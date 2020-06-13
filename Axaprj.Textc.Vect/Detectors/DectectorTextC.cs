using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Textc;
using Takenet.Textc.Csdl;
using Takenet.Textc.Processors;

namespace Axaprj.Textc.Vect.Detectors
{
    /// <summary>
    /// Token detector, Takenet.Textc lexemes based
    /// </summary>
    public class DectectorTextC : IOutputProcessor
    {
        /// <summary>Initializes a new instance</summary>
        public DectectorTextC() : this(new SyntaxParser()) { }
        /// <summary>Initializes a new instance</summary>
        public DectectorTextC(ISyntaxParser syntaxParser)
        {
            SyntaxParser = syntaxParser ?? throw new ArgumentNullException(nameof(syntaxParser));
            CommandProcessors = new List<ICommandProcessor>();
        }

        public ICollection<ICommandProcessor> CommandProcessors { get; }

        protected ISyntaxParser SyntaxParser { get; }

        public void AddProcessor(string syntaxPattern, Delegate fn = null)
        {
            var syntax = CsdlParser.Parse(syntaxPattern);
            fn = fn ?? (Func<Task<string>>)(() => Task.FromResult(string.Empty));
            // Now create the command processors, to bind the methods to the syntaxes
            var cmd_proc = new DelegateCommandProcessor(
                fn,
                true,
                this,
                syntax
                );
            CommandProcessors.Add(cmd_proc);
        }

        Task IOutputProcessor.ProcessOutputAsync(object output, IRequestContext context, CancellationToken cancellationToken)
        {
            return Task.Run(() => { });
        }

        //public bool TryDetect(ITextCursor textCursor, out IEnumerable<Expression> parsedExprs, CancellationToken cancellationToken)
        //{
        //    if (textCursor == null)
        //        throw new ArgumentNullException(nameof(textCursor));
        //    parsedExprs = ParseInput(textCursor, cancellationToken, stop_on_first: false);
        //    return parsedExprs.Any();
        //}

        public bool TryDetect(ITextCursor textCursor, out Expression parsedExpr, CancellationToken cancellationToken)
        {
            if (textCursor == null)
                throw new ArgumentNullException(nameof(textCursor));
            var exprs = ParseInput(textCursor, cancellationToken, stop_on_first: true);
            parsedExpr = exprs.FirstOrDefault();
            return parsedExpr != null;
        }

        IEnumerable<Expression> ParseInput(ITextCursor textCursor, CancellationToken cancellationToken, bool stop_on_first)
        {
            IRequestContext context = textCursor.Context;
            var res = new List<Expression>();
            foreach (var commandProcessor in CommandProcessors.ToList())
            {
                // Gets all the syntaxes that are of the same culture of the context or are culture invariant
                var syntaxes = commandProcessor.Syntaxes.Where(
                        s => s.Culture.Equals(context.Culture) || s.Culture.Equals(CultureInfo.InvariantCulture));

                foreach (var syntax in syntaxes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    textCursor.RightToLeftParsing = syntax.RightToLeftParsing;
                    textCursor.Reset();

                    if (SyntaxParser.TryParse(textCursor, syntax, context, out Expression expression))
                    {
                        res.Add(expression);
                        if (stop_on_first)
                            return res;
                    }
                }
            }
            return res;
        }
    }
}