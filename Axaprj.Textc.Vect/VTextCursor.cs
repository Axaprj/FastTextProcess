using System;
using System.Text;
using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    /// <summary>
    /// Defines a cursor with start-stop tokens range.
    /// </summary>
    public class VTextCursor : IVTextCursor
    {
        readonly char TOKEN_SEPARATOR;

        private string[] _tokens;
        private int _leftPos;
        private int _leftPosCheckpoint;
        private int _rightPos;
        private int _rightPosCheckpoint;

        int _posStart;
        int _posStop;

        protected int PosStart { get => _posStart; }
        protected int PosStop { get => _posStop; }

        /// <summary>Gets the <see cref="IVRequestContext"/> context via <see cref="Context"/></summary>
        public IVRequestContext VContext => (IVRequestContext)Context;
        /// <summary>Gets the context where the cursor was created.</summary>
        public IRequestContext Context { get; }

        protected string[] Tokens { get => _tokens; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VTextCursor" /> class.
        /// </summary>
        /// <param name="inputText">Preprocessed input text tokens.</param>
        /// <param name="context">The context.</param>
        /// <param name="tokenSeparator">The token separator</param>
        /// <exception cref="System.ArgumentNullException">inputText</exception>
        public VTextCursor(string inputText, IVRequestContext context, char tokenSeparator = ' ')
        {
            TOKEN_SEPARATOR = tokenSeparator;
            SetTokens(inputText);
            Context = context;
            Reset();
        }

        protected void SetTokens(string inputText, int posStart = -1, int posStop = -1)
        {
            if (inputText == null)
                throw new ArgumentNullException(nameof(inputText));
            SetTokens(inputText.Split(TOKEN_SEPARATOR), posStart, posStop);
        }

        protected void SetTokens(string[] tokens, int posStart = -1, int posStop = -1)
        {
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            SetRangePos(posStart, posStop);
        }

        protected void SetRangePos(int posStart = -1, int posStop = -1)
        {
            _posStart = posStart < 0 ? 0 : posStart;
            _posStop = posStop < 0 ? _tokens.Length - 1 : posStop;
        }

        protected void IncRangePos(int posStartDelta = 0, int posStopDelta = 0)
        {
            _posStart += posStartDelta;
            _posStop += posStopDelta;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            for (var i = _leftPos; i <= _rightPos; i++)
            {
                stringBuilder.Append(_tokens[i]);

                if (i + 1 <= _rightPos)
                {
                    stringBuilder.Append(TOKEN_SEPARATOR);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Extracts the current token and advances the cursor.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all remaining text from the cursor.
        /// </summary>
        /// <returns></returns>
        public string All()
        {
            var text = ToString();
            if (RightToLeftParsing)
            {
                _rightPos = _leftPos - 1;
            }
            else
            {
                _leftPos = _rightPos + 1;
            }

            return text;
        }

        /// <summary>
        /// Saves the current cursor position to allow further rollback.
        /// </summary>
        public void SavePosition()
        {
            _leftPosCheckpoint = _leftPos;
            _rightPosCheckpoint = _rightPos;
        }

        /// <summary>
        /// Rollbacks the cursor to the last saved position.
        /// </summary>
        public void RollbackPosition()
        {
            _leftPos = _leftPosCheckpoint;
            _rightPos = _rightPosCheckpoint;
        }

        /// <summary>
        /// Gets a preview of the next available token without advancing the cursor position.
        /// </summary>
        /// <returns></returns>
        public string Peek()
        {
            string token = null;

            if (!IsEmpty)
            {
                var pos = RightToLeftParsing ? _rightPos : _leftPos;
                token = _tokens[pos];
            }

            return token;
        }

        /// <summary>
        /// Gets the current parse direction.
        /// </summary>
        public bool RightToLeftParsing { get; set; }

        /// <summary>
        /// Inverts the cursor parse direction.
        /// </summary>
        public void InvertParsing()
        {
            RightToLeftParsing = !RightToLeftParsing;
        }

        /// <summary>
        /// Returns to the initial position.
        /// </summary>
        public void Reset()
        {
            _leftPos = _leftPosCheckpoint = _posStart;
            _rightPos = _rightPosCheckpoint = _posStop;
        }

        /// <summary>
        /// Indicates if the cursor is empty.
        /// </summary>
        public bool IsEmpty => _leftPos > _rightPos;
    }
}
