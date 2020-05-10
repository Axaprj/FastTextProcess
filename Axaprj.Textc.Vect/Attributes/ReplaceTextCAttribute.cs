using Axaprj.Textc.Vect.Detectors;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Axaprj.Textc.Vect.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public abstract class ReplaceTextCAttribute : ReplaceAttribute
    {
        List<string> _patternsList;
        ReplaceDetector _detector;

        public string SyntaxPattern
        {
            get => _patternsList[0];
            set => SyntaxPatterns = new string[] { value };
        }
        public string[] SyntaxPatterns
        {
            get => _patternsList.ToArray();
            set => _patternsList = new List<string>(value);
        }

        public virtual string GetArgument(Takenet.Textc.Expression expr)
            => string.Empty;

        protected ReplaceDetector GetDetector()
            => _detector = _detector ?? new ReplaceDetector(this);

        public int TryReplace<TReplEnum>(
            TReplEnum enum_val, IVTextCursor textCursor, CancellationToken cancellation) =>
            GetDetector().TryReplace<TReplEnum>(enum_val, textCursor, cancellation);

        protected object GetTokenValue(string name, Takenet.Textc.Expression expr)
        {
            foreach(var token in expr.Tokens)
            {
                if (token != null)
                {
                    if (token.Type.Name == name)
                        return token.Value;
                }
            }
            return string.Empty;
        }
    }
}
