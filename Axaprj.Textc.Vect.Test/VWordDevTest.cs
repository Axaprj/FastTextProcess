using Axaprj.WordToVecDB.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Textc;
using Takenet.Textc.Csdl;
using Takenet.Textc.PreProcessors;
using Takenet.Textc.Processors;
using Takenet.Textc.Splitters;
using Xunit;
using Xunit.Abstractions;

namespace Axaprj.Textc.Vect.Test
{
    public class VWordDevTest : TestBase
    {
        public VWordDevTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void SumVWordTest()
        {
            Func<int, int, Task<int>> sum_fn = (a, b) => Task.FromResult(a + b);
            //var a_sum = "{ValidValues: ['sum'], MaxCosine: 0.5}";
            var a_sum = "sum";
            var text_proc = CreateTextProcessor<int>($"operation+:VWord({a_sum}) a:Integer :Word?(and) b:Integer", sum_fn);
            var context = CreateContext();
            string inputText = "sum 5 3";
            var task = text_proc.ProcessAsync(inputText, context, CancellationToken.None);
            task.Wait();
            inputText = "sums 5 4";
            task = text_proc.ProcessAsync(inputText, context, CancellationToken.None);
            task.Wait();
            try
            {
                inputText = "summary 5 5";
                task = text_proc.ProcessAsync(inputText, context, CancellationToken.None);
                task.Wait();
                throw new InvalidOperationException($"True Negative '{inputText}'");
            }
            catch (AggregateException agex)
            {
                agex.Handle((x) => x is MatchNotFoundException);
            }
        }

        [Fact]
        public void SumVWordSlideTest()
        {
            Func<int, int, Task<int>> sum_fn = (a, b) => Task.FromResult(a + b);
            //var a_sum = "{ValidValues: ['sum'], MaxCosine: 0.5}";
            var a_sum = "sum";
            var context = CreateContext();
            var text_proc = CreateTextProcessor<int>($"operation+:VWord({a_sum}) a:Integer :Word?(and) b:Integer", sum_fn);
            string inputText = "please make for me sum 5 3 operation";
            var task1 = text_proc.ProcessSlidingAsync(inputText, context, CancellationToken.None);
            task1.Wait();
            Assert.True(context.IsMatched);
            Log($"MatchedTextSlice: '{context.MatchedTextSlice}'");
            try
            {
                context.Clear();
                inputText = "please make for me summary 5 2 operation";
                var task2 = text_proc.ProcessSlidingAsync(inputText, context, CancellationToken.None);
                task2.Wait();
                throw new InvalidOperationException($"True Negative '{inputText}'");
            }
            catch (AggregateException agex)
            {
                agex.Handle((x) => x is MatchNotFoundException);
            }
        }


        VRequestContext CreateContext()
        {
            AssertFileExists(W2VDictEN);
            return new VRequestContext
            {
                W2VDictFile = W2VDictEN,
                LangLabel = LangLabel.en,
                MinCosine = 0.6f
            };
        }

        SlidingTextProcessor CreateTextProcessor<TRes>(string syntaxPattern, Delegate fn)
        {
            var out_proc = new DelegateOutputProcessor<TRes>(
                (o, ctx) =>
                Log($"Result: {o}; op: {ctx.GetVariable("operation")}"));
            var syntax = CsdlParser.Parse(syntaxPattern);
            // Now create the command processors, to bind the methods to the syntaxes
            var cmd_proc = new DelegateCommandProcessor(
                fn,
                true,
                out_proc,
                syntax
                );
            // Finally, create the text processor and register all command processors
            //var text_proc = new TextProcessor(new PunctuationTextSplitter());
            var text_proc = new SlidingTextProcessor();
            text_proc.CommandProcessors.Add(cmd_proc);
            text_proc.TextPreprocessors.Add(new TrimTextPreprocessor());

            return text_proc;
        }

    }
}
