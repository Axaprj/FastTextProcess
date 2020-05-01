using Axaprj.Textc.Vect.Attributes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Axaprj.Textc.Vect.Processor
{
    public class ReplaceDetector: DetectorBase<VRequestContext> 
    {

        public ReplaceDetector(ReplaceTextCAttribute replace_attr)
        {
            foreach(var sp in replace_attr.SyntaxPatterns)
            {
                AddProcessor(sp, replace_attr.CreateFnResultTask());
            }
        }

        public override void SetupLog(Action<string> log)
        {
            base.SetupLog(log);
        }

        protected override Task OnMatchedAsync(object output, string remaining_text, VRequestContext context, CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                var str_value = output?.ToString();
                var macro = StringUtil.GetReplaceMacro(context.ReplaceEnum, context.ReplaceAttrib, str_value);
                context.ReplaceByEnum(macro, remaining_text, out int prev_txt_len);
                TryDetect(context, cancellationToken, startPos: prev_txt_len);
                // Log($"<<{output}>> <<{StringUtil.GetPhrase(ctx.TextSlice)}>>"); 
            });  //  {ctx.TextSlice}
        }

        protected override Task OnNotMatchedAsync(VRequestContext context, CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                Log($"Replacement not found");
            });
        }
    }
}
