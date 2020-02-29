using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    public interface ISlidingRequestContext: IRequestContext
    {
        string MatchedTextSlice { get; set; }
        bool IsMatched { get; }
    }
}
