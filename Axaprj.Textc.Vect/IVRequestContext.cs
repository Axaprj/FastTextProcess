using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;
using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    public interface IVRequestContext : IRequestContext
    {
        LangLabel LangLabel { get; set; }

        float MinCosine { get; set; }

        Dict FindVectByWord(string word);
    }
}
