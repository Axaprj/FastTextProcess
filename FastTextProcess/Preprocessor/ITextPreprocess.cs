using Axaprj.WordToVecDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.FastTextProcess.Preprocessor
{
    public interface ITextPreprocess
    {
        void RunAsync(Action<ITextSource, PreprocessItem> actResultHandler);

        void Push(ITextSource txt_src);
                
        void CompleteAdding();
    }
}
