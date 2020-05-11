using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;
using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    /// <summary>
    /// Textc.Vect Request Context
    /// </summary>
    public interface IVRequestContext : IRequestContext
    {
        /// <summary>Current request context language (W2V)</summary>
        LangLabel LangLabel { get; set; }

        /// <summary>Current request context minimal W2V metric to match</summary>
        float MinCosine { get; set; }

        /// <summary>
        /// Get W2V dictionary record
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        Dict FindVectByWord(string word);
    }
}
