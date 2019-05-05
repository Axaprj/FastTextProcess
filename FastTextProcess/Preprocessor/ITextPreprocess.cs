using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Preprocessor
{
    public interface ITextPreprocess
    {
        void Push(ITextSource txt_src);

        void RunAsync(Action<ITextSource, PreprocessItem> actResultHandler);
    }
}
