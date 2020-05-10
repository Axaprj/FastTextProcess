using Axaprj.Textc.Vect.Attributes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Textc;

namespace Axaprj.Textc.Vect.Detectors
{
    public class ReplaceDetector: DectectorTextC
    {

        public ReplaceDetector(ReplaceTextCAttribute replace_attr)
        {
            foreach(var sp in replace_attr.SyntaxPatterns)
            {
                AddProcessor(sp);
            }
        }

        public int TryReplace(IVTextCursor textCursor, CancellationToken cancellation)
        {
            //TryDetect(ITextCursor textCursor, out IEnumerable<Expression> parsedExpr, IRequestContext context, CancellationToken cancellationToken)
            return 0;            
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
