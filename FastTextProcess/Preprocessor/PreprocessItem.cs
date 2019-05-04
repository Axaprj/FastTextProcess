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
        public string[] Words { get { return Text.Split(' '); } }
        public readonly FTLangLabel Lang;
        public PreprocessItem(string text, string ft_lang_label)
            : this(text, FTLangLabel.NotSpecified)
        {
            Enum.TryParse(ft_lang_label, out Lang);
        }
        public PreprocessItem(string text, FTLangLabel lang)
        {
            Text = text;
            Lang = lang;
        }
    }
}
