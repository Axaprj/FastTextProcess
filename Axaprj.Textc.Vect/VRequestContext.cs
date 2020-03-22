using Axaprj.WordToVecDB;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;

namespace Axaprj.Textc.Vect
{
    public class VRequestContext : Axaprj.Textc.Vect.RequestContext, ISlidingRequestContext
    {
        const string VNAME_TextSlice = "VRequestContext.TextSlice";
        const string VNAME_TextProcess = "VRequestContext.TextProcess";
        public string W2VDictFile { get; set; }

        public LangLabel LangLabel { get; set; }

        public float MinCosine { get; set; }

        public Dict FindVectByWord(string word)
        {
            var serv = VectorsService.Instance(W2VDictFile);
            return serv.FindByWord(word, LangLabel);
        }

        public string TextSlice
        {
            get
            {
                return (string)GetVariable(VNAME_TextSlice);
            }
            set
            {
               SetVariable(VNAME_TextSlice, value);
            }
        }

        public string TextProcess
        {
            get
            {
                return (string)GetVariable(VNAME_TextProcess);
            }
            set
            {
                SetVariable(VNAME_TextProcess, value);
            }
        }

    }
}
