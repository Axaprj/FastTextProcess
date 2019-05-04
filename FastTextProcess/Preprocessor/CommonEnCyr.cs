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
        readonly Regex rexClnCommonEnCyr = new Regex("[^A-Za-zА-Яа-яІіЇїЄєҐґЁё0-9(),.!?\'`\"]", RegexOptions.Compiled);
        readonly FastTextLauncher _ftLauncher;

        public CommonEnCyr(FastTextLauncher ft_launcher)
        {
            _ftLauncher = ft_launcher;
            _ftLauncher.RunByLineAsync(
                (txt_src, txt_lbl) => HandleResult(txt_src, new PreprocessItem(txt_src.GetText(), txt_lbl))
                );
        }

        public void ProcessWords(string txt) => ProcessWords(new TextSourceStub(txt));

        public override void ProcessWords(ITextSource txt_src)
        {
            var ctxt = CleanCommonEn(txt_src.GetText(), rexClnCommonEnCyr);
            txt_src.SetText(ctxt);
            _ftLauncher.Push(txt_src);
        }
    }
}
