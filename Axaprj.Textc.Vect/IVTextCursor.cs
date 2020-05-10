using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    public interface IVTextCursor : ITextCursor
    {
        IVRequestContext VContext { get; }

        void ProcessNext();

        void ProcessReplace(int token_count, string new_text);

        void ResetProcessedToInput();
    }
}
