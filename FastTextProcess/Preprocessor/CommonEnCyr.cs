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
        }

        public void Push(string txt) => Push(new TextSourceStub(txt));

        public override void Push(ITextSource txt_src)
        {
            var ctxt = CleanCommonEn(txt_src.GetText(), rexClnCommonEnCyr);
            txt_src.SetText(ctxt);
            _ftLauncher.Push(txt_src);
        }

        public override void RunAsync(Action<ITextSource, PreprocessItem> actResultHandler)
        {
            _ftLauncher.RunByLineAsync(
                (txt_src, txt_lbl) => actResultHandler(txt_src, new PreprocessItem(txt_src.GetText(), txt_lbl))
                );
        }

    }
}
