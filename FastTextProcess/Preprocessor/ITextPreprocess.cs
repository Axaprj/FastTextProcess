using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Preprocessor
{
    public interface ITextPreprocess
    {
        void ProcessWords(ITextSource txt_src);

        Action<ITextSource, PreprocessItem> HandleResult { get; set;  }
    }
}
