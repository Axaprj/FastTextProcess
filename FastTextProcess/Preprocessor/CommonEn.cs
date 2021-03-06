﻿using Axaprj.WordToVecDB;
using Axaprj.WordToVecDB.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Axaprj.FastTextProcess.Preprocessor
{
    /// <summary>
    /// Common En text preprocessor
    /// </summary>
    public class CommonEn : ITextPreprocess
    {
        readonly Regex rexClnCommonEn = new Regex("[^A-Za-z0-9(),.!?\'`\"]", RegexOptions.Compiled);
        readonly Regex rexVe = new Regex("(?<lett>[A-Za-z]) ' ve", RegexOptions.Compiled);
        readonly Regex rexRe = new Regex("(?<lett>[A-Za-z]) ' re", RegexOptions.Compiled);
        readonly Regex rexD = new Regex("(?<lett>[A-Za-z]) ' d", RegexOptions.Compiled);
        readonly Regex rexLL = new Regex("(?<lett>[A-Za-z]) ' ll", RegexOptions.Compiled);
        readonly Regex rexS = new Regex("(?<lett>[A-Za-z]) ' s", RegexOptions.Compiled);

        readonly Regex rexClnSpaces = new Regex("\\s{2,}", RegexOptions.Compiled);

        Action<ITextSource, PreprocessItem> _resultHandler;

        public virtual void CompleteAdding()
        {
            // do nothing here
        }

        public virtual void Push(ITextSource txt_src)
        {
            var ctxt = CleanCommon(txt_src.GetText(), rexClnCommonEn);
            txt_src.SetText(ctxt);
            _resultHandler(txt_src, new PreprocessItem(ctxt, LangLabel.en));
        }

        public virtual void RunAsync(Action<ITextSource, PreprocessItem> actResultHandler)
        {
            _resultHandler = actResultHandler;
        }

        /// <summary>
        /// Tokenization/string cleaning for all datasets except for SST.
        /// Original taken from https://github.com/yoonkim/CNN_sentence/blob/master/process_data.py
        /// </summary>
        protected string CleanCommon(string str, Regex clean_rex)
        {
            //str = str.ToLower();
            str = clean_rex.Replace(str, " ");
            str = str
                //.Replace(" ' s", " 's")
                //.Replace(" ' ve", " 've")
                // .Replace("n't", " n't") // not in fasttext dict
                //.Replace(" ' re", " 're")
                //.Replace(" ' d", " 'd")
                //.Replace(" ' ll", " 'll")
                .Replace("'", " ' ") // add
                .Replace("\"", " \" ") // add
                .Replace("`", " ' ") // add
                .Replace(".", " . ") // add
                .Replace(",", " , ")
                .Replace("!", " ! ")
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Replace("?", " ? ");
            str = str.Replace("n ' t", "n't");
            str = rexVe.Replace(str, "${lett} 've");
            str = rexRe.Replace(str, "${lett} 're");
            str = rexD.Replace(str, "${lett} 'd");
            str = rexLL.Replace(str, "${lett} 'll");
            str = rexS.Replace(str, "${lett} 's");
            str = rexClnSpaces.Replace(str, " ");
            return str.Trim();
        }
          
    }
}
