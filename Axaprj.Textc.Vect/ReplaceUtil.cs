using Axaprj.Textc.Vect.Attributes;
using Axaprj.WordToVecDB.Enums;
using System;
using System.Linq;
using System.Threading;
using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    public static class ReplaceUtil
    {

        public static void ProcessReplace<TReplEnum>(this IVTextCursor textCursor, CancellationToken cancellation)
        {
            ProcessIterator<TReplEnum>(textCursor, (txt_cursor, enum_val, attr) =>
                {
                    if (attr is ReplaceTextAttribute)
                    {
                        var ra = (ReplaceTextAttribute)attr;
                        //txt = txt.ReplaceMacroText(ra.Txt, StringUtil.GetReplaceMacro(enum_val, ra));
                    }
                    else if (attr is ReplaceTextCAttribute)
                    {
                        var ra = (ReplaceTextCAttribute)attr;
                        ra.Detector.TryReplace(txt_cursor, cancellation);
                        //txt = context.TextProcess;
                    }
                    else
                        throw new InvalidOperationException(
                            $"Unknown replace attribute '{attr}' in '{typeof(TReplEnum)}'");
                });
        }

        static void ProcessIterator<TReplEnum>(IVTextCursor textCursor, Action<IVTextCursor, TReplEnum, ReplaceAttribute> act_replace)
        {
            LangLabel lang = textCursor.VContext.LangLabel;
            var enum_vals = (TReplEnum[])Enum.GetValues(typeof(TReplEnum));
            foreach (TReplEnum enumVal in enum_vals)
            {
                var replace_attrs = GetTextAttrib<TReplEnum, ReplaceAttribute>(enumVal)
                    .Where(ra => ra.Lng == lang);
                foreach (var attr in replace_attrs)
                {
                    act_replace(textCursor, enumVal, attr);
                }
            }
        }

        static TAttrib[] GetTextAttrib<TReplEnum, TAttrib>(TReplEnum enumVal) where TAttrib : Attribute
        {
            var memInfo = typeof(TReplEnum).GetMember(enumVal.ToString());
            if (memInfo.Length > 0)
            {
                var attributes = memInfo[0].GetCustomAttributes(typeof(TAttrib), false);
                if (attributes.Length > 0)
                    return (TAttrib[])attributes;
            }
            return new TAttrib[] { };
        }

        public static string GetReplaceMacro<TReplEnum>(TReplEnum val_enum)
            => GetReplaceMacro<TReplEnum>(val_enum, LangLabel.NA, val_arg: null);

        public static string GetReplaceMacro<TReplEnum>(TReplEnum val_enum, LangLabel lang, string val_arg)
        {
            try
            {
                var attr = GetTextAttrib<TReplEnum, ReplaceAttribute>(val_enum).First(v => v.Lng == lang);
                return StringUtil.GetReplaceMacro(val_enum, attr, val_arg);
            }
            catch
            { // for debug
                throw;
            }
        }

    }
}
