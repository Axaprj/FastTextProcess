﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Textc;
using Takenet.Textc.PreProcessors;
using Takenet.Textc.Processors;
using Takenet.Textc.Scorers;

namespace Axaprj.Textc.Vect
{
    /// <summary>
    /// SlidingTextProcessor
    /// </summary>
    public class SlidingTextProcessor : TextProcessor
    {
        const char SplitChar = ' ';

        public SlidingTextProcessor()
            : base(new SyntaxParser(), new RatioExpressionScorer()) { }

        public SlidingTextProcessor(ITextSplitter textSplitter)
            : base(new SyntaxParser(), new RatioExpressionScorer(), textSplitter) { }

        public SlidingTextProcessor(ISyntaxParser syntaxParser, IExpressionScorer expressionScorer, ITextSplitter textSplitter = null)
            : base(syntaxParser, expressionScorer, textSplitter) { }

        public async Task ProcessSlidingAsync(string inputText, IRequestContext context, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(inputText))
                throw new ArgumentException("The input string must have a value", nameof(inputText));
            inputText = inputText.Trim(SplitChar);
            var tasks = new List<Task<bool>>();
            for (int inx = 0; inx >= 0; inx = inputText.IndexOf(SplitChar, inx + 1))
            {
                var proc_text = inx == 0 ? inputText : inputText.Substring(inx + 1);
                tasks.Add(Task.Run(async () => 
                    {
                        try
                        {
                            await ProcessAsync(proc_text, context, cancellationToken);
                            return true;
                        }
                        catch (MatchNotFoundException)
                        {
                            return false;
                        }
                    })
                );
            }
            await Task.WhenAll(tasks);
            if(!tasks.Any(t=>t.Result))
                throw new MatchNotFoundException(inputText);
        }

    }
}