using FastTextProcess.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Preprocessor
{
    /// <summary>
    /// Text Preprocessing result item
    /// </summary>
    public class PreprocessItem
    {
        public readonly string Text;
        public readonly string[] Words;
        public readonly FTLangLabel Lang;
        public PreprocessItem(string text, string[] words, string ft_lang_label)
            :this(text , words, FTLangLabel.NotSpecified)
        {
            Enum.TryParse(ft_lang_label, out Lang);
        }
        public PreprocessItem(string text, string[] words, FTLangLabel lang)
        {
            Text = text;
            Words = words;
            Lang = lang;
        }
    }
}
