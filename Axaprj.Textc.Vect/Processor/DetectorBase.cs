using System;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Textc;
using Takenet.Textc.Csdl;
using Takenet.Textc.PreProcessors;
using Takenet.Textc.Processors;

namespace Axaprj.Textc.Vect.Processor
{
    public abstract class DetectorBase<TContext> : IOutputExpressionProcessor
        where TContext : ISlidingRequestContext
    {
        protected Action<string> Log = (msg) => { };
        protected readonly SlidingTextProcessor Processor;

        public virtual void SetupLog(Action<string> log) => Log = log;

        protected abstract Task OnMatchedAsync(object output, string remaining_text, TContext context, CancellationToken cancellationToken);
        protected abstract Task OnNotMatchedAsync(TContext context, CancellationToken cancellationToken);

        protected DetectorBase()
        {
            Processor = new SlidingTextProcessor();
            Processor.TextPreprocessors.Add(new TrimTextPreprocessor());
        }

        protected void AddProcessor(string syntaxPattern, Delegate fn)
        {
            var syntax = CsdlParser.Parse(syntaxPattern);
            // Now create the command processors, to bind the methods to the syntaxes
            var cmd_proc = new DelegateCommandProcessor(
                fn,
                true,
                this,
                syntax
                );
            // Finally, create the text processor and register all command processors
            Processor.CommandProcessors.Add(cmd_proc);
        }

        public bool TryDetect(TContext context, CancellationToken cancellation
            , int startPos = 0, int stopPos = int.MaxValue)
        {
            try
            {
                string text = context.TextProcess;
                Processor.ProcessSlidingAsync(
                    text, context, cancellation, startPos, stopPos).Wait();
                return true;
            }
            catch (AggregateException aex)
            {
                aex.Handle((e) => e is MatchNotFoundException);
            }
            OnNotMatchedAsync(context, cancellation);
            return false;
        }

        Task IOutputProcessor.ProcessOutputAsync(object output, IRequestContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Not Used (backward-compatibility stub)");
        }

        async Task IOutputExpressionProcessor.ProcessOutputAsync(object output, Expression expression, CancellationToken cancellationToken)
        {
            var ctx = (TContext)expression.Context;
            await OnMatchedAsync(output, expression.RemainingText, ctx, cancellationToken);
        }

    }
}
