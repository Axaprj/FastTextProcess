using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.WordToVecDB.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LangInfoAttribute : Attribute
    {
        /// <summary>FastText Language labels</summary>
        public string FTLabel { get; set; }
    }

    /// <summary> Language identifiers </summary>
    public enum LangLabel
    {
        /// <summary> Not specified/unknown </summary>
        NA = 0,
        /// <summary> English </summary>
        [LangInfo(FTLabel = "__label__en")]
        en,
        /// <summary> Russian </summary>
        [LangInfo(FTLabel = "__label__ru")]
        ru,
        /// <summary> Ukrainian </summary>
        [LangInfo(FTLabel = "__label__uk")]
        uk
    }

    public static class LangLabelExt
    {
        public static string GetStdLangLabel(this LangLabel enumVal)
        {
            return Enum.GetName(typeof(LangLabel), enumVal);
        }

        public static string GetFastTextLabel(this LangLabel enumVal)
        {
            var memInfo = typeof(LangLabel).GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(LangInfoAttribute), false);
            var attr = (attributes.Length > 0) ? (LangInfoAttribute)attributes[0] : null;
            return attr == null ? "" : attr.FTLabel;
        }

        public static bool TryParseFastTextLabel(string ft_label, out LangLabel parsedVal)
        {
            parsedVal = LangLabel.NA;
            if (!string.IsNullOrEmpty(ft_label))
            {
                var val = ft_label.Trim();
                foreach (LangLabel lbl in (LangLabel[])Enum.GetValues(typeof(LangLabel)))
                {
                    if (val.Equals(lbl.GetFastTextLabel()))
                    {
                        parsedVal = lbl;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
