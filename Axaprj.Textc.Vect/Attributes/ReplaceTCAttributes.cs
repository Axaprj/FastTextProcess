using System;
using Takenet.Textc;

namespace Axaprj.Textc.Vect.Attributes
{
    /// <summary>Measured Values replace attribute (use 'num' for value and UOM property for unit )</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReplaceNumAttribute : ReplaceTextCAttribute
    {
        public ReplaceNumAttribute()
        {
            NumVarName = "num";
        }
        
        /// <summary>Fixed argument of the macro</summary>
        public object UOM { get; set; }

        /// <summary>TextC variable name</summary>
        public string NumVarName { get; set; }

        public override string GetArgument(Expression expr)
            => $"{expr.GetTokenValue(NumVarName)}{StringUtil.MakeMacro(UOM, this)}";
    }

    /// <summary>TextC replace attribute (no values)</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReplaceTCAttribute : ReplaceTextCAttribute
    {
    }

}
