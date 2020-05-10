using Axaprj.Textc.Vect.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Textc;

namespace Axaprj.Textc.Vect.Detectors
{
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

        public int TryReplace<TReplEnum>(TReplEnum enum_val, IVTextCursor textCursor, CancellationToken cancellation)
        {
            int cnt = 0;
            textCursor.Reset();
            while (!textCursor.IsEmpty)
            {
                if (TryDetect(textCursor, out Expression expr, cancellation))
                {
                    var str_value = ReplaceAttr.GetArgument(expr);
                    var macro = StringUtil.GetReplaceMacro(enum_val, ReplaceAttr, str_value);
                    var token_cnt = expr.Tokens.Count(t => t != null);
                    textCursor.ProcessReplace(token_cnt, macro);
                }
                else
                    textCursor.ProcessNext();
            }
            textCursor.ResetProcessedToInput();
            return cnt;
        }
        //protected override Task OnMatchedAsync(object output, string remaining_text, VRequestContext context, CancellationToken cancellationToken)
        //{
        //    return Task.Run(() => {
        //        var str_value = output?.ToString();
        //        var macro = StringUtil.GetReplaceMacro(context.ReplaceEnum, context.ReplaceAttrib, str_value);
        //        context.ReplaceByEnum(macro, remaining_text, out int prev_txt_len);
        //        TryDetect(context, cancellationToken, startPos: prev_txt_len);
        //        // Log($"<<{output}>> <<{StringUtil.GetPhrase(ctx.TextSlice)}>>"); 
        //    });  //  {ctx.TextSlice}
        //}


    }
}
