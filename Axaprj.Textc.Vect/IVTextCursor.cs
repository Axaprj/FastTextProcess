using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    public interface IVTextCursor : ITextCursor
    {
        IVRequestContext VContext { get; }
    }
}
