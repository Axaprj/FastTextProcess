# Axaprj.Textc.Vect
(in the deep development phase now)
Natural Language features extraction framework (Takenet.Textc based)
- uses own [Takenet.Textc .Net Core port](https://github.com/Axaprj/textc-csharp/tree/port2core)
- a part of [FastTextProcess](https://github.com/Axaprj/FastTextProcess) project.
 
## contents
 - VWord - ValueToken based on distance metrics of embedded vectors
 - MacroReplace - Easy usable and extendable NLP features extractor. 

## code
VS 2017, VS 2019,.NET Core, SQLite

## VWord() textc extension usage explanation
- Uses En Ru Uk aligned vectors DB [FastTextRoutinesCyr::ProcCreateDbRuk](https://github.com/Axaprj/FastTextProcess/blob/master/Tests/FastTextProcess.Tests/FastTextRoutinesCyr.cs)
- **:VWord(consumption)** matched to following [DictDbTests::TestV2W1consumption](https://github.com/Axaprj/FastTextProcess/blob/master/Tests/FastTextProcess.Tests/DictDbTests.cs)

In English
```
cos(consumption,consumptive)=0.7271223 cos(consumption,overconsumption)=0.8072221 cos(consumption,consumptions)=0.9393129 cos(consumption,underconsumption)=0.7711332 cos(consumption,consumptives)=0.7006535 cos(consumption,production/consumption)=0.7023017 cos(consumption,income–consumption)=0.7680753 cos(consumption,deconsumption)=0.7769312 cos(consumption,consumption,)=0.8818358 cos(consumption,‘consumption)=0.8400897 cos(consumption,nonconsumption)=0.8223644 cos(consumption,consumption/)=0.7580206 cos(consumption,autoconsumption)=0.7710906 cos(consumption,hyperconsumption)=0.7327829 cos(consumption,#consumption)=0.838056
```
In Russian
```
cos(consumption,потребления)=0.4010898 cos(consumption,потребление)=0.4859301 cos(consumption,потреблению)=0.3793698 cos(consumption,потреблять)=0.3513061 cos(consumption,потребление —)=0.3628894 cos(consumption,потребления,)=0.3594068
```
In Ukrainian
```
cos(consumption,споживання)=0.4414848 cos(consumption,споживати)=0.3519385 cos(consumption,калорійність)=0.3616202
```
## MacroReplace example
- Uses En vectors DB [FastTextProcess.Tests.AclImdbDemo.ProcCreateDbTest](https://github.com/Axaprj/FastTextProcess/blob/master/Tests/FastTextProcess.Tests/AclImdbDemo.cs)
- Example	[Axaprj.Textc.Vect.Test.ReplaceDemoTest.VehiclesFeaturesTest](https://github.com/Axaprj/FastTextProcess/blob/master/Axaprj.Textc.Vect.Test/ReplaceDemoTest.cs)
```
Input:
The EU5-compliant, 130kW/430Nm 3.0-litre turbo-diesel engine introduced in the MY16.5 MU-X(and MY17 D-MAX) continues on with the option of six-speed manual or six-speed automatic transmissions.
The lowest combined fuel consumption for the range is 7.9L/100km for LS-U and LS-T 4x4 autos(209 CO2 emissions), 
and peaks at 8.1 liter per 100 km.
Output:
The <%VFeat.EU5compliant%> , 130 kW / 430 Nm <%VFeat.DIESEL_ENG(3.0, UOM.L)%> introduced in the MY16 . 5 MU - X ( and MY17 D - MAX ) continues on with the option of six - speed manual or six - speed automatic transmissions . 
The lowest combined fuel consumption for the range is <%VFeat.FuelConsumption(7.9, UOM.L_100Km)%> for LS - U and LS - T 4 x4 autos ( <%VFeat.CO2(209, UOM.g_Km)%> ) , 
and peaks at <%VFeat.FuelConsumption(8.1, UOM.L_100Km)%> .
```
Rules set
```csharp
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
```
## revision history
 - 2020 May 16: v1.0.2.1 VReplaceTextCursor refactoring
 - 2020 May 11: v1.0.2.0 MacroReplace implementation refactoring
 - 2020 May 02: v1.0.1.1 MacroReplace implementation
 - 2019 Dec 22: v0.0.1  VWord implementation
 
## authors
[Axaprj](https://github.com/Axaprj), [Igor Alexeev](mailto:axaprj2000@yahoo.com) 

You are welcome to [Property Indicators Lab](https://propertyindicators.github.io/)! 
We know how to use it in real projects.
For any questions, please contact us at email propertyindicators@gmail.com.
