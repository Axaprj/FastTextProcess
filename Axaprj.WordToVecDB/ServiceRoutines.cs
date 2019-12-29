using Axaprj.WordToVecDB.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Axaprj.WordToVecDB
{
    public class ServiceRoutines
    {
        /// <summary>
        /// Cyrillic letters only
        /// </summary>
        readonly Regex rexClnWordCyr = new Regex("[^А-Яа-яІіЇїЄєҐґЁё]", RegexOptions.Compiled);
        /// <summary>
        /// English letters only
        /// </summary>
        readonly Regex rexClnWordEn = new Regex("[^A-Za-z]", RegexOptions.Compiled);
        /// <summary>
        /// get only letter characters
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public string GetLettersOnly(string word, FTLangLabel lang)
        {
            if (string.IsNullOrWhiteSpace(word))
                return string.Empty;
            Regex rex = null;
            switch (lang)
            {
                case FTLangLabel.__label__en:
                    rex = rexClnWordEn;
                    break;
                case FTLangLabel.__label__ru:
                case FTLangLabel.__label__uk:
                    rex = rexClnWordCyr;
                    break;
            }
            if (rex == null)
                return word;
            var str = rex.Replace(word, string.Empty);
            return str.Trim();
        }
    }
}
