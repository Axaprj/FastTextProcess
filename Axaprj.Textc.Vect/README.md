# Axaprj.Textc.Vect
Takenet.Textc extension. 
- uses own [Takenet.Textc .Net Core port](https://github.com/Axaprj/textc-csharp/tree/port2core)
- a part of [FastTextProcess](https://github.com/Axaprj/FastTextProcess) project.
 
## contents
 - VWord - ValueToken based on distance metrics of embedded vectors

## code
VS 2017, .NET Core, SQLite

## VWord() textc usage example
 Uses En Ru Uk aligned vectors DB (FastTextRoutinesCyr.cs::ProcCreateDbRuk)
":VWord(consumption)" matched to following (FastTextProcess.Tests\DictDbTests.cs::TestV2W1consumption)
```
In English
cos(consumption,consumptive)=0.7271223 cos(consumption,overconsumption)=0.8072221 cos(consumption,consumptions)=0.9393129 cos(consumption,underconsumption)=0.7711332 cos(consumption,consumptives)=0.7006535 cos(consumption,production/consumption)=0.7023017 cos(consumption,income–consumption)=0.7680753 cos(consumption,deconsumption)=0.7769312 cos(consumption,consumption,)=0.8818358 cos(consumption,‘consumption)=0.8400897 cos(consumption,nonconsumption)=0.8223644 cos(consumption,consumption/)=0.7580206 cos(consumption,autoconsumption)=0.7710906 cos(consumption,hyperconsumption)=0.7327829 cos(consumption,#consumption)=0.838056
In Russian
cos(consumption,потребления)=0.4010898 cos(consumption,потребление)=0.4859301 cos(consumption,потреблению)=0.3793698 cos(consumption,потреблять)=0.3513061 cos(consumption,потребление —)=0.3628894 cos(consumption,потребления,)=0.3594068
```
## author
Axaprj for https://propertyindicators.github.io/
