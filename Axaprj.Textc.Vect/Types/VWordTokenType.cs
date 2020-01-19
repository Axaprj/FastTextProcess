using Axaprj.WordToVecDB;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Takenet.Textc;
using Takenet.Textc.Metadata;
using Takenet.Textc.Types;

namespace Axaprj.Textc.Vect.Types
{
    /// <summary>
    /// ValueToken based on distance metrics of embedded vectors
    /// </summary>
    [TokenType(ShortName = "VWord")]
    public class VWordTokenType : ValueTokenTypeBase<string>
    {
        const float NONE_Cosine = -99;

        public VWordTokenType(string name, bool isContextual, bool isOptional, bool invertParsing)
            : base(name, isContextual, isOptional, invertParsing)
        {
            MinCosine = NONE_Cosine;
        }

        [TokenTypeProperty]
        public float MinCosine { get; set; }

        protected VRequestContext GetVRequestCtx(IRequestContext context)
        {
            var vctx = context as VRequestContext;
            if (vctx == null)
                throw new InvalidOperationException($"{typeof(VRequestContext)} is required");
            return vctx;
        }

        protected override bool Compare(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        protected override bool HasMatch(string value, IRequestContext context, out string bestMatch)
        {
            var hasMatch = false;
            bestMatch = null;

            var validValues = GetValidValues(context);

            if (validValues.Any(v => Compare(v, value)))
            {
                bestMatch = validValues.First(v => Compare(v, value));
                hasMatch = true;
            }
            else
            {
                var ctx = GetVRequestCtx(context);
                var v_dict = ctx.FindVectByWord(value);
                if (v_dict != null)
                {
                    var vv_dicts = validValues
                        .Select(vv => ctx.FindVectByWord(vv))
                        .Where(d => d != null);
                    var min_cosine = MinCosine == NONE_Cosine
                        ? ctx.MinCosine : MinCosine;
                    Dict nearest_value = v_dict.GetNearest(vv_dicts, min_cosine);
                    hasMatch = nearest_value != null;
                    if (hasMatch)
                        bestMatch = nearest_value.Word;
                }
            }
            return hasMatch;
        }
    }
}
