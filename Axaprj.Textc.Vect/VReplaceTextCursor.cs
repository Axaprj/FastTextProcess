using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    /// <summary>
    /// Defines a cursor with tokens replacement capability.
    /// </summary>
    public class VReplaceTextCursor : VTextCursor, IVReplaceTextCursor
    {
        List<string> _processed;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="inputTokens">The input text tokens.</param>
        /// <param name="context">The context.</param>
        /// <param name="tokenSeparator">The token separator</param>
        /// <exception cref="System.ArgumentNullException">inputText</exception>
        public VReplaceTextCursor(string inputText, IVRequestContext context, char tokenSeparator = ' ')
            : base(inputText, context, tokenSeparator)
        {
        }

        List<string> ProcessedList
        {
            get => _processed = _processed ?? new List<string>(capacity: Tokens.Length);
        }

        public bool GoToNextToken()
        {
            bool isok = false;
            if (isok = Tokens.Length > PosStart)
            {
                Reset();
                ProcessedList.Add(Tokens[PosStart]);
                IncRangePos(posStartDelta: 1);
            }
            return isok;
        }

        public void GoForwardWithReplacement(string replacement_text, string remaining_text)
        {
            Reset();
            ProcessedList.Add(replacement_text);
            SetTokens(remaining_text);
        }

        public void SetupProcessedToInput()
        {
            if (_processed != null)
            {
                SetTokens(_processed.ToArray());
                _processed.Clear();
            }
            Reset();
        }
    }

}
