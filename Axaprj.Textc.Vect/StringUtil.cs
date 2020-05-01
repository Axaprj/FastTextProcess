using Axaprj.Textc.Vect.Attributes;
using System.Text;

namespace Axaprj.Textc.Vect
{
    public static class StringUtil
    {
        const string MACRO_BEGIN = "<%";
        const string MACRO_END = "%>";
        const string SP = " ";

        static int GetPhraseLength(string src, char separator, int start_inx = 0)
        {
            if (!string.IsNullOrWhiteSpace(src))
            {
                start_inx = src.IndexOf(separator);
                while (start_inx >= 0 && InIntoMacro(src, start_inx))
                {
                    start_inx = src.IndexOf(separator, start_inx + 1);
                }
                return start_inx > 0 ? start_inx : src.Length;
            }
            return 0;
        }

        static bool InIntoMacro(string src, int inx)
        {
            var einx = src.IndexOf(MACRO_END, inx);
            if (einx >= 0)
            {
                var binx = src.IndexOf(MACRO_BEGIN, inx);
                return binx < 0 || binx > einx;
            }
            return false;
        }

        public static string UnmarkMacro(string macro)
        {
            if (string.IsNullOrWhiteSpace(macro))
                return string.Empty;
            macro = macro.Trim();
            if (macro.StartsWith(MACRO_BEGIN))
                macro = macro.Substring(MACRO_BEGIN.Length);
            if (macro.EndsWith(MACRO_END))
                macro = macro.Substring(0, macro.Length - MACRO_END.Length);
            return string.IsNullOrWhiteSpace(macro)
                ? string.Empty : ", " + macro;
        }

        public static string MakeMacro(object obj, ReplaceAttribute attr, string val_arg = null)
        {
            var macro = attr.GetMacro(obj, val_arg);
            return string.IsNullOrWhiteSpace(macro)
                ? string.Empty : ", " + macro;
        }

        public static string GetReplaceMacro(object val_enum, ReplaceAttribute attr, string val_arg = null)
            => MACRO_BEGIN + attr.GetMacro(val_enum, val_arg) + MACRO_END;

        public static string ReplaceMacroText(this string txt, string old_item, string new_item)
        {
            old_item = old_item.Trim();
            new_item = new_item.Trim();
            txt = txt.Replace(SP + old_item + SP, SP + new_item + SP);
            if (txt.StartsWith(old_item + SP))
                txt = new_item + SP + txt.Substring(old_item.Length + 1);
            if (txt.EndsWith(SP + old_item))
                txt = txt.Substring(0, txt.Length - old_item.Length - 1) + SP + new_item;
            return txt;
        }

        public static string InsertReplaceMacro(string prev_txt, string macro_value, string remaining_text)
        {
            var sbld = new StringBuilder(prev_txt.TrimEnd());
            sbld.Append(SP);
            sbld.Append(macro_value);
            sbld.Append(SP);
            sbld.Append(remaining_text.TrimStart());
            return sbld.ToString();
        }
    }
}
