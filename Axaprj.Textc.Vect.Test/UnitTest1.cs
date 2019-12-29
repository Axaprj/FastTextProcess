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
    public class UnitTest1: TestBase
    {
        public UnitTest1(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Test1()
        {
            Func<int, int, Task<int>> sum_fn = (a, b) => Task.FromResult(a + b);
            var a_sum = "{ 'ValidValues': ['sum'], 'Lang': 'en', MaxCosine: '0.6'}";
            var text_proc = CreateTextProcessor($"operation+:VWord({a_sum}) a:Integer :Word?(and) b:Integer", sum_fn);
            string inputText = "sum 5 3";
            var context = new RequestContext();
            var task = text_proc.ProcessAsync(inputText, context, CancellationToken.None);
            task.Wait();
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
