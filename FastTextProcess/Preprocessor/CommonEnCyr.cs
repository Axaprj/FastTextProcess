using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FastTextProcess.Preprocessor
{
    public class CommonEnCyr: CommonEn
    {
        Regex rexClnCommonEnCyr = new Regex("[^A-Za-zА-Яа-яІіЇїЄєҐґЁё0-9(),.!?\'`\"]", RegexOptions.Compiled);

        public string[] ProcessWords(string txt)
        {
            txt = ProcessText(txt);
            var warr = txt.Split(' ');
            return warr;
        }

        public string ProcessText(string txt)
        {
            txt = CleanCommonEn(txt, rexClnCommonEnCyr);
            return txt;
        }

    }
}
