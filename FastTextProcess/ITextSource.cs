using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess
{
    public interface ITextSource
    {
        string GetText();
        void SetText(string txt);
    }

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
