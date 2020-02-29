using Axaprj.WordToVecDB;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    public class VRequestContext : RequestContext, ISlidingRequestContext
    {
        const string VNAME_MatchedTextSlice = "VRequestContext.MatchedTextSlice";
        public string W2VDictFile { get; set; }

        public LangLabel LangLabel { get; set; }

        public float MinCosine { get; set; }

        public Dict FindVectByWord(string word)
        {
            var serv = VectorsService.Instance(W2VDictFile);
            return serv.FindByWord(word, LangLabel);
        }

        readonly object MatchedTextSliceLock = new object();
        public string MatchedTextSlice
        {
            get
            {
                lock (MatchedTextSliceLock)
                {
                    return (string)GetVariable(VNAME_MatchedTextSlice);
                }
            }
            set
            {
                lock (MatchedTextSliceLock)
                {

                    if(GetVariable(VNAME_MatchedTextSlice) == null)
                        SetVariable(VNAME_MatchedTextSlice, value);
                }
            }
        }

        public bool IsMatched {
            get {
                lock (MatchedTextSliceLock)
                {
                    return GetVariable(VNAME_MatchedTextSlice) != null;
                }
            }
        }
    }
}
