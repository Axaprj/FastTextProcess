using Axaprj.WordToVecDB;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;

namespace Axaprj.Textc.Vect
{
    public class VRequestContext : Axaprj.Textc.Vect.RequestContext, IVRequestContext
    {
        public string W2VDictFile { get; set; }

        public LangLabel LangLabel { get; set; }

        public float MinCosine { get; set; }

        public Dict FindVectByWord(string word)
        {
            var serv = VectorsService.Instance(W2VDictFile);
            return serv.FindByWord(word, LangLabel);
        }
    }
}
