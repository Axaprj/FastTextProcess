using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.WordToVecDB
{
    public interface ITextSource
    {
        string GetText();
        void SetText(string txt);
    }

}
