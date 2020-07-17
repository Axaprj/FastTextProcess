using Axaprj.WordToVecDB;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;
using System;

namespace Axaprj.Textc.Vect
{
    public class VRequestContext : Takenet.Textc.RequestContext, IVRequestContext
    {
        public VRequestContext()
        {
            DbgLog = (msg) => { };
        }

        public string W2VDictFile { get; set; }

        public LangLabel LangLabel { get; set; }

        public float MinCosine { get; set; }

        public Action<string> DbgLog { get; set; }

        public Dict FindVectByWord(string word)
        {
            var serv = VectorsService.Instance(W2VDictFile);
            var vect = serv.FindByWord(word, LangLabel);
            if (vect == null)
                DbgLog($"Vector for '{word}' not found");
            return vect;
        }

    }
}
