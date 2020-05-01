using System;
using System.Threading.Tasks;

namespace Axaprj.Textc.Vect.Attributes
{
    /// <summary>Measured Values replace attribute (use 'num' for value and 'uom' for unit )</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReplaceUnitNumAttribute : ReplaceTextCAttribute
    {
        public override Delegate CreateFnResultTask()
            => (Func<decimal, string, Task<string>>)
                    ((num, uom) => Task.FromResult($"{num}{StringUtil.UnmarkMacro(uom)}"));
    }
    /// <summary>Measured Values replace attribute (use 'num' for value and UOM property for unit )</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReplaceNumAttribute : ReplaceTextCAttribute
    {
        public object UOM { get; set; }
        public override Delegate CreateFnResultTask()
            => (Func<decimal, Task<string>>)
                    ((num) => Task.FromResult($"{num}{StringUtil.MakeMacro(UOM, this)}"));
    }
    /// <summary>TextC replace attribute (no values)</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReplaceTCAttribute : ReplaceTextCAttribute
    {
        public override Delegate CreateFnResultTask()
            => (Func<Task<string>>)
                    (() => Task.FromResult(string.Empty));
    }

}
