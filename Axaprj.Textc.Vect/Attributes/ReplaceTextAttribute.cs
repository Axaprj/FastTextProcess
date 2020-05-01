using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.Textc.Vect.Attributes
{
    /// <summary>Simple text replace</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReplaceTextAttribute : ReplaceAttribute
    {
        public string Txt { get; set; }
    }
}
