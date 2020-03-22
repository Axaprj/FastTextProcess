using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    public interface ISlidingRequestContext: IRequestContext
    {
        string TextSlice { get; set; }
        string TextProcess { get; set; }
    }
}
