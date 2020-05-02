using Xunit;
using Xunit.Abstractions;

namespace Axaprj.Textc.Vect.Test
{
    public class ReplaceDevTest : TestBase
    {
        string PH1 = 
            $"The EU5-compliant, 130kW/430Nm 3.0-litre turbo-diesel engine introduced in the MY16.5 MU-X(and MY17 D-MAX)" 
            + $" continues on with the option of six-speed manual or six-speed automatic transmissions.";
        string PH2 =
            $"The lowest combined fuel consumption for the range is 7.9L/100km for LS-U and LS-T 4x4 autos(209 CO2 emissions),"
            + $" and peaks at 8.1L/100km(for all LS-T and LS-U 4x2 and LS-M 4x4). ";
        string PH3 = $" CO2 output for the latter models has been measured at 214g/km for auto versions.";

        public ReplaceDevTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void SumVWordTest()
        {

        }

        //VRequestContext CreateContext()
        //{
        //    AssertFileExists(W2VDictEN);
        //    return new VRequestContext
        //    {
        //        W2VDictFile = W2VDictEN,
        //        LangLabel = LangLabel.en,
        //        MinCosine = 0.6f
        //    };
        //}
    }
}
