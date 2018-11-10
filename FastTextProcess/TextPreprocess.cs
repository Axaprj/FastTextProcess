using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FastTextProcess
{
    public class TextPreprocess
    {
        Regex rexClnCommonEn = new Regex("[^A-Za-z0-9(),.!?\'`\"]", RegexOptions.Compiled);
        Regex rexClnSpaces = new Regex("\\s{2,}", RegexOptions.Compiled);

        public string[] Process(string txt)
        {
            txt = CleanCommonEn(txt);
            var warr = txt.Split(' ');
            return warr;
        }

        /// <summary>
        /// Tokenization/string cleaning for all datasets except for SST.
        /// Original taken from https://github.com/yoonkim/CNN_sentence/blob/master/process_data.py
        /// </summary>
        string CleanCommonEn(string str)
        {
            str = rexClnCommonEn.Replace(str, " ");
            str = str
                .Replace("'s", " 's")
                .Replace("'ve", " 've")
                .Replace("n't", " n't")
                .Replace("'re", " 're")
                .Replace("'d", " 'd")
                .Replace("'ll", " 'll")
                .Replace(",", " , ")
                .Replace(".", " . ") // add
                .Replace("!", " ! ")
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Replace("?", " ? ");
            str = rexClnSpaces.Replace(str, " ");
            return str.Trim().ToLower();
        }
    }
}
