using Axaprj.Textc.Vect.Attributes;
using Axaprj.WordToVecDB.Enums;
using System;
using System.Linq;
using System.Threading;

namespace Axaprj.Textc.Vect
{
    public static class ReplaceUtil
    {

        public static void ProcessRepl<TEnum>(this VRequestContext context, CancellationToken cancellation)
        {
            context.TextProcess = Preprocess<TEnum>(context.TextProcess, context.LangLabel,
                (txt, enum_val, attr) =>
                {
                    if (attr is ReplaceTextAttribute)
                    {
                        var ra = (ReplaceTextAttribute)attr;
                        txt = txt.ReplaceMacroText(ra.Txt, StringUtil.GetReplaceMacro(enum_val, ra));
                        return txt;
                    }
                    else if (attr is ReplaceTextCAttribute)
                    {
                        var ra = (ReplaceTextCAttribute)attr;
                        context.ReplaceAttrib = ra;
                        context.ReplaceEnum = enum_val;
                        ra.Detector.TryDetect(context, cancellation);
                        txt = context.TextProcess;
                        return txt;
                    }
                    else
                        throw new InvalidOperationException(
                            $"Unknown replace attribute '{attr}' in '{typeof(TEnum)}'");
                });
        }

        static string Preprocess<TReplEnum>(string src, LangLabel lang, Func<string, TReplEnum, ReplaceAttribute, string> fn_replace)
        {
            var res = src;
            if (!string.IsNullOrWhiteSpace(src))
            {
                var enum_vals = (TReplEnum[])Enum.GetValues(typeof(TReplEnum));
                foreach (TReplEnum enumVal in enum_vals)
                {
                    var replace_attrs = GetTextAttrib<TReplEnum, ReplaceAttribute>(enumVal)
                        .Where(ra => ra.Lng == lang);
                    foreach (var attr in replace_attrs)
                    {
                        res = fn_replace(res, enumVal, attr);
                    }
                }
            }
            return res;
        }

        static TAttrib[] GetTextAttrib<TReplEnum, TAttrib>(TReplEnum enumVal) where TAttrib: Attribute
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
