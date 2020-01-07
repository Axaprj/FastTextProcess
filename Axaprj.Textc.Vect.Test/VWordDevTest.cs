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
            var text_proc = CreateTextProcessor($"operation+:VWord({a_sum}) a:Integer :Word?(and) b:Integer", sum_fn);
            AssertFileExists(W2VDictEN);
            var context = new VRequestContext
            {
                W2VDictFile = W2VDictEN,
                LangLabel = LangLabel.en,
                MinCosine = 0.6f
            };
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

        ITextProcessor CreateTextProcessor(string syntaxPattern, Delegate fn)
        {
            var out_proc = new DelegateOutputProcessor<int>(
                (o, ctx) =>
                Log($"Result: {o}"));
            var syntax = CsdlParser.Parse(syntaxPattern);
            // Now create the command processors, to bind the methods to the syntaxes
            var cmd_proc = new DelegateCommandProcessor(
                fn,
                true,
                out_proc,
                syntax
                );
            // Finally, create the text processor and register all command processors
            var text_proc = new TextProcessor(new PunctuationTextSplitter());
            text_proc.CommandProcessors.Add(cmd_proc);
            text_proc.TextPreprocessors.Add(new TrimTextPreprocessor());

            return text_proc;
        }

    }
}
