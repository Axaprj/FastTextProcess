using Axaprj.FastTextProcess;
using Axaprj.Textc.Vect.Attributes;
using Axaprj.WordToVecDB.Enums;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Axaprj.Textc.Vect.Test
{
    /// <summary>Unit Of Measure</summary>
    enum UOM
    {
        kW,
        Nm,
        L,
        L_100Km,
        g_Km
    }
    /// <summary>Vehicles Features</summary>
    enum VFeat
    {
        [ReplaceTC(
        SyntaxPattern = ":Word(EU5) :Word?(-) :VWord(compliant)", Lng = LangLabel.en)]
        EU5compliant,
        [ReplaceNum(
            SyntaxPattern =
            "num:Decimal :Word?(-) :VWord?(litre) :VWord?(turbo) :Word?(-) :VWord(diesel) :VWord(engine)",
            UOM = UOM.L, Lng = LangLabel.en)]
        DIESEL_ENG,
        [ReplaceNum(
            SyntaxPattern =
            "num:Decimal :Word(CO2) :VWord?(emission)",
            UOM = UOM.g_Km, Lng = LangLabel.en)]
        CO2,
        [ReplaceNum(
            SyntaxPatterns = new string[] {
            "num:Decimal :VWord(liters) :VWord(per) :Word(100) :VWord(kilometers)",
            "num:Decimal :Word(L) :Word(/) :Word(100) :Word(km)"
            },
            UOM = UOM.L_100Km, Lng = LangLabel.en)]
        FuelConsumption
    }

    public class ReplaceDemoTest : TestBase
    {
        string TestPH1 =
            $"The EU5-compliant, 130kW/430Nm 3.0-litre turbo-diesel engine introduced in the MY16.5 MU-X(and MY17 D-MAX)"
            + $" continues on with the option of six-speed manual or six-speed automatic transmissions."
            + $"The lowest combined fuel consumption for the range is 7.9L/100km for LS-U and LS-T 4x4 autos(209 CO2 emissions),"
            + $" and peaks at 8.1 liter per 100 km.";

        public ReplaceDemoTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void VehiclesFeaturesTest()
        {
            Log(TestPH1);
            var ptxt = RegExUtil.RegExClean(TestPH1);
            ptxt = RegExUtil.Prepare(ptxt);
            Log(ptxt);
            var ctx = CreateContext();
            var cur = new VTextCursor(ptxt, ctx);
            cur.ProcessReplace<VFeat>(CancellationToken.None);
            Log(cur.ToString());
        }

        VRequestContext CreateContext()
        {
            AssertFileExists(W2VDictEN);
            return new VRequestContext()
            {
                W2VDictFile = W2VDictEN,
                LangLabel = LangLabel.en,
                MinCosine = 0.6f
            };
        }
    }
}
