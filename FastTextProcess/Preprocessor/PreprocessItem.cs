using Axaprj.WordToVecDB.Enums;
using System;

namespace Axaprj.FastTextProcess.Preprocessor
{
    /// <summary>
    /// Text Preprocessing result item
    /// </summary>
    public class PreprocessItem
    {
        public readonly string Text;
        public string[] Words { get { return Text.Split(' '); } }
        public readonly LangLabel Lang;
        public PreprocessItem(string text, string ft_lang_label)
            : this(text, LangLabel.NA)
        {
            //Enum.TryParse(ft_lang_label, out Lang);
            LangLabelExt.TryParseFastTextLabel(ft_lang_label, out Lang);
        }
        public PreprocessItem(string text, LangLabel lang)
        {
            Text = text;
            Lang = lang;
        }
    }
}
