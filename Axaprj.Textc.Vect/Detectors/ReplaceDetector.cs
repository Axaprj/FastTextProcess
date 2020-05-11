using Axaprj.Textc.Vect.Attributes;
using System.Linq;
using System.Threading;
using Takenet.Textc;

namespace Axaprj.Textc.Vect.Detectors
{
    /// <summary>
    /// Token detector with MacroReplace functionality
    /// </summary>
    public class ReplaceDetector : DectectorTextC
    {
        readonly ReplaceTextCAttribute ReplaceAttr;

        public ReplaceDetector(ReplaceTextCAttribute replace_attr)
        {
            ReplaceAttr = replace_attr;
            foreach (var sp in replace_attr.SyntaxPatterns)
            {
                AddProcessor(sp);
            }
        }

        public int TryReplace<TReplEnum>(TReplEnum enum_val, IVReplaceTextCursor textCursor, CancellationToken cancellation)
        {
            int cnt = 0;
            textCursor.Reset();
            while (!textCursor.IsEmpty)
            {
                if (TryDetect(textCursor, out Expression expr, cancellation))
                {
                    var str_value = ReplaceAttr.GetArgument(expr);
                    var macro = StringUtil.GetReplaceMacro(enum_val, ReplaceAttr, str_value);
                    textCursor.GoForwardWithReplacement(expr.DetectedTokenCount(), macro);
                }
                else
                    textCursor.GoToNextToken();
            }
            textCursor.SetupProcessedToInput();
            return cnt;
        }
    }
}
