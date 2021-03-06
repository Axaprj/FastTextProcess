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
    /// (NOT USED NOW) SlidingTextProcessor
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

        public async Task ProcessSlidingAsync(string inputText
            , IRequestContext context
            , CancellationToken cancellationToken
            , int startPos = 0, int stopPos = int.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(inputText))
                throw new ArgumentException("The input string must have a value", nameof(inputText));
            inputText = inputText.Trim(SplitChar);
            await Task.Run(() =>
            {
                for (int inx = startPos
                    ; (inx >= 0) && (inx <= stopPos)
                    ; inx = inputText.IndexOf(SplitChar, inx + 1))
                {
                    var text_slice = inx == 0 ? inputText : inputText.Substring(inx + 1);
                    try
                    {
                        ProcessAsync(text_slice, context, cancellationToken).Wait();
                        return;
                    }
                    catch (AggregateException agex)
                    { // not found here
                        agex.Handle((x) => x is MatchNotFoundException);
                    }
                    catch(Exception)
                    { // for debug
                        throw;
                    }

                }
                throw new MatchNotFoundException(inputText);
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}