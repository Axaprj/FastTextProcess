using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Preprocessor
{
    public interface ITextPreprocess
    {
        string[] Process(string txt);
    }
}
