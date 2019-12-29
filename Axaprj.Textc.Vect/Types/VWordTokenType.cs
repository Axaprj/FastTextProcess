using System;
using System.Collections.Generic;
using System.Text;
using Takenet.Textc.Metadata;
using Takenet.Textc.Types;

namespace Axaprj.Textc.Vect.Types
{
    [TokenType(ShortName = "VWord")]
    public class VWordTokenType : ValueTokenTypeBase<string>
    {
        public VWordTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
        }

        [TokenTypeProperty]
        public string Lang { get; internal set; }

        [TokenTypeProperty]
        public  float MaxCosine { get; internal set; }


        protected override bool Compare(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }
    }
}
