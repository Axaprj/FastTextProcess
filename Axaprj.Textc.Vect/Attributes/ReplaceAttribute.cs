using Axaprj.WordToVecDB.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.Textc.Vect.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReplaceAttribute : Attribute
    {
        public ReplaceAttribute()
        {
            Lng = LangLabel.NA;
        }
        public LangLabel Lng { get; set; }
        public virtual string GetMacro(object obj, string argVal = null)
        {
            if (obj == null)
                return string.Empty;
            else
            {
                var arg = (string.IsNullOrWhiteSpace(argVal) ? string.Empty : "(" + argVal + ")");
                return (obj.GetType().IsEnum ? obj.GetType().Name + ".": string.Empty)
                    + obj.ToString() + arg;
            }
        }
    }
}
