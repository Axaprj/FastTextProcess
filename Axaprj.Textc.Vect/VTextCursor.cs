using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    /// <summary>
    /// Defines a cursor that extracts a token for each word in a text.
    /// </summary>
    public sealed class VTextCursor : IVTextCursor
    {
        public readonly char TOKEN_SEPARATOR = ' ';

        private string[] _tokens;
        private int _leftPos;
        private int _leftPosCheckpoint;
        private int _rightPos;
        private int _rightPosCheckpoint;

        List<string> _processed;
        int _leftPosProcessed = 0;
        int LeftPos
        {
            get => _leftPos + _leftPosProcessed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VTextCursor" /> class.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="context">The context.</param>
        /// <param name="tokenSeparator">The token separator</param>
        /// <exception cref="System.ArgumentNullException">inputText</exception>
        public VTextCursor(string inputText, IVRequestContext context, char tokenSeparator=' ')
        {
            TOKEN_SEPARATOR = tokenSeparator;
            if (inputText == null)
                throw new ArgumentNullException(nameof(inputText));
            _tokens = inputText.Split(' ');
            Context = context;
            Reset();
        }

        public IVRequestContext VContext => (IVRequestContext) Context;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            for (var i = LeftPos; i <= _rightPos; i++)
            {
                stringBuilder.Append(_tokens[i]);

                if (i + 1 <= _rightPos)
                {
                    stringBuilder.Append(TOKEN_SEPARATOR);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>Gets the context where the cursor was created.</summary>
        public IRequestContext Context { get; }

        /// <summary>Extracts the current token and advances the cursor.</summary>
        public string Next()
        {
            var token = Peek();

            if (!string.IsNullOrEmpty(token))
            {
                if (RightToLeftParsing)
                {
                    _rightPos--;
                }
                else
                {
                    _leftPos++;
                }
            }

            return token;
        }
        /// <summary>Gets all remaining text from the cursor.</summary>
        public string All()
        {
            var text = ToString();
            if (RightToLeftParsing)
            {
                _rightPos = LeftPos - 1;
            }
            else
            {
                _leftPos = _rightPos + 1;
            }

            return text;
        }
        /// <summary>Saves the current cursor position to allow further rollback.</summary>
        public void SavePosition()
        {
            _leftPosCheckpoint = LeftPos;
            _rightPosCheckpoint = _rightPos;
        }
        /// <summary>Rollbacks the cursor to the last saved position.</summary>
        public void RollbackPosition()
        {
            _leftPos = _leftPosCheckpoint;
            _rightPos = _rightPosCheckpoint;
        }
        /// <summary>Gets a preview of the next available token without advancing the cursor position.</summary>
        public string Peek()
        {
            string token = null;

            if (!IsEmpty)
            {
                var pos = RightToLeftParsing ? _rightPos : LeftPos;
                token = _tokens[pos];
            }

            return token;
        }
        /// <summary>Gets the current parse direction.</summary>
        public bool RightToLeftParsing { get; set; }
        /// <summary>Inverts the cursor parse direction.</summary>
        public void InvertParsing()
        {
            RightToLeftParsing = !RightToLeftParsing;
        }
        /// <summary>Returns to the initial position.</summary>
        public void Reset()
        {
            _leftPos = _leftPosCheckpoint = 0;
            _rightPos = _rightPosCheckpoint = _tokens.Length - 1;
        }

        List<string> Processed()
            => _processed = _processed ?? new List<string>(capacity: _tokens.Length);
        

        public void ProcessNext()
        {
            Reset();
            Processed().Add(_tokens[LeftPos]);
            _leftPosProcessed++;
        }

        public void ProcessReplace(int token_count, string new_text)
        {
            Reset();
            Processed().Add(new_text);
            _leftPosProcessed += token_count;
        }

        public void ResetProcessedToInput()
        {
            _leftPosProcessed = 0;
            if(_processed != null)
            {
                _tokens = _processed.ToArray();
                _processed = null;
            }
            Reset();
        }

        /// <summary>Indicates if the cursor is empty.</summary>
        public bool IsEmpty => LeftPos > _rightPos;

    }

}
