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
            TReplEnum enum_val, IVReplaceTextCursor textCursor, CancellationToken cancellation) =>
            GetDetector().TryReplace<TReplEnum>(enum_val, textCursor, cancellation);
    }
}
