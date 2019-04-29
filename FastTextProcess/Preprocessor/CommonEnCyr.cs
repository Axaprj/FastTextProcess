using FastTextProcess.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FastTextProcess.Preprocessor
{
    /// <summary>
    /// Common En, Ru, Uk (Cyrillic) text preprocessor
    /// </summary>
    public class CommonEnCyr : CommonEn
    {
        Regex rexClnCommonEnCyr = new Regex("[^A-Za-zА-Яа-яІіЇїЄєҐґЁё0-9(),.!?\'`\"]", RegexOptions.Compiled);

        public override PreprocessItem ProcessWords(string txt)
        {
            var ctxt = CleanCommonEn(txt, rexClnCommonEnCyr);
            return new PreprocessItem(txt, ctxt.Split(' '), GetLang(ctxt));
        }
        protected override FTLangLabel GetLang(string txt)
        {
            return FTLangLabel.__label__en;
        }
    }
}
