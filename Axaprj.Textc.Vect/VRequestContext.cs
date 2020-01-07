using Axaprj.WordToVecDB.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    public class VRequestContext : RequestContext
    {
        const string CtxVarW2VDictFile = "__W2VDictFile";
        const string CtxVarW2VLangLabel = "__W2VLangLabel";
        const string CtxVarW2VMinCosine = "__W2VMinCosine";

        public string W2VDictFile
        {
            get { return (string) GetVariable(CtxVarW2VDictFile); }
            set { SetVariable(CtxVarW2VDictFile, value);  }
        }

        public LangLabel LangLabel
        {
            get { return (LangLabel)GetVariable(CtxVarW2VLangLabel); }
            set { SetVariable(CtxVarW2VLangLabel, value); }
        }

        public float MinCosine
        {
            get { return (float)GetVariable(CtxVarW2VMinCosine); }
            set { SetVariable(CtxVarW2VMinCosine, value); }
        }

    }
}
