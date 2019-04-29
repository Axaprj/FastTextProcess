using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Preprocessor
{
    public interface ITextPreprocess
    {
        /// <summary>
        /// Process text to words array
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        PreprocessItem ProcessWords(string txt);
    }
}
