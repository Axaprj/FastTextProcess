using Takenet.Textc;

namespace Axaprj.Textc.Vect
{
    /// <summary>
    /// Textc.Vect Text Cursor
    /// </summary>
    public interface IVTextCursor : ITextCursor
    {
        /// <summary>Textc.Vect Request Context</summary>
        IVRequestContext VContext { get; }
    }
    /// <summary>
    /// Cursor with tokens replacement capability
    /// </summary>
    public interface IVReplaceTextCursor : IVTextCursor
    {
        /// <summary>
        /// Move current token to processed and setup start to next token
        /// </summary>
        /// <returns>false on the end of stream</returns>
        bool GoToNextToken();
        /// <summary>
        /// Set replacement to processed and setup start to the following token
        /// </summary>
        void GoForwardWithReplacement(string replacement_text, string remaining_text);
        /// <summary>
        /// Setup processed to input tokens
        /// </summary>
        void SetupProcessedToInput();
    }
}
