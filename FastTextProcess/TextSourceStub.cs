using System;
using System.Collections.Generic;
using System.Text;
using Axaprj.WordToVecDB;

namespace Axaprj.FastTextProcess
{
    internal class TextSourceStub : ITextSource
    {
        string Text;
        public TextSourceStub(string txt)
        {
            Text = txt;
        }
        public string GetText() => Text;
        public void SetText(string txt) { Text = txt; }
    }
}
