using System;
using System.Threading.Tasks;
using Takenet.Textc;

namespace Axaprj.Textc.Vect.Attributes
{
    /// <summary>Measured Values replace attribute (use 'num' for value and UOM property for unit )</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReplaceNumAttribute : ReplaceTextCAttribute
    {
        public object UOM { get; set; }
        public override string GetArgument(Expression expr)
            => $"{GetTokenValue("num", expr)}{StringUtil.MakeMacro(UOM, this)}";
    }
    /// <summary>TextC replace attribute (no values)</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReplaceTCAttribute : ReplaceTextCAttribute
    {
    }

}
