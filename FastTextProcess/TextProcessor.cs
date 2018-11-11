﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FastTextProcess
{
    public class TextProcessor : IDisposable
    {
        public readonly BlockingCollection<string> QueueProcess;
        readonly Task taskPreprocess;

        readonly BlockingCollection<string[]> QueueWordToDict;
        readonly Task taskWordToDict;

        readonly BlockingCollection<long[]> QueueStoreResult;
        readonly Task taskStoreResult;

        public TextProcessor(string dbf_w2v, int boundedCapacity = 10000)
        {
            QueueProcess = new BlockingCollection<string>(boundedCapacity);
            QueueWordToDict = new BlockingCollection<string[]>(boundedCapacity);
            QueueStoreResult = new BlockingCollection<long[]>(boundedCapacity);
            var cancel_token_src = new CancellationTokenSource();
            var cancel_token = cancel_token_src.Token;

            taskPreprocess = Task.Run(() =>
            {
                try
                {
                    var preproc = new TextPreprocess();
                    Parallel.ForEach(
                        QueueProcess.GetConsumingEnumerable(cancel_token)
                        , (txt) => QueueWordToDict.Add(preproc.Process(txt), cancel_token)
                    );
                    QueueWordToDict.CompleteAdding();
                }
                catch
                {
                    cancel_token_src.Cancel();
                    throw;
                }
            }, cancel_token
            );
            taskWordToDict = Task.Run(() =>
            {
                try
                {
                    using (var wordToDict = new WordToDictProcessor(dbf_w2v))
                    {
                        #region experimental (unused)
                        //int thread_cnt_max = Environment.ProcessorCount;
                        //var tasks = new BlockingCollection<Task>(thread_cnt_max);
                        //while (!QueueWordToDict.IsCompleted)
                        //{
                        //    var proc_buff = new List<string[]>();
                        //    string[] words;
                        //    while (QueueWordToDict.TryTake(out words))
                        //    {
                        //        proc_buff.Add(words);
                        //    }
                        //    if (proc_buff.Count == 0)
                        //        continue;
                        //    var t = Task.Run(() => wordToDict.Process(proc_buff));
                        //    while (!tasks.TryAdd(t))
                        //    {
                        //        var tarr = tasks.ToArray();
                        //        var inx = Task.WaitAny(tarr);
                        //        if (tarr[inx].IsFaulted)
                        //            throw tarr[inx].Exception;
                        //        if (!tasks.TryTake(out tarr[inx]))
                        //            throw new InvalidOperationException();
                        //    }
                        //}
                        //Task.WaitAll(tasks.ToArray());
                        #endregion
                        var opt = new ParallelOptions { CancellationToken = cancel_token };
                        Parallel.ForEach(
                            QueueWordToDict.GetConsumingEnumerable(cancel_token)
                            , opt
                            , (words) => wordToDict.WordsToInxsForParallel(words)
                        );
                        wordToDict.StoreEmbed();
                    }
                }
                catch
                {
                    cancel_token_src.Cancel();
                    throw;
                }
            }, cancel_token
            );
            taskStoreResult = Task.Run(() =>
            {
                try
                {

                }
                catch
                {
                    cancel_token_src.Cancel();
                    throw;
                }
            }, cancel_token
            );
        }

        void WaitForFinalize()
        {
            QueueProcess.CompleteAdding();
            var agg_ex = new List<Exception>();
            try
            {
                taskWordToDict.Wait();
            }
            catch (Exception ex) { agg_ex.Add(ex); }
            try
            {
                taskStoreResult.Wait();
            }
            catch (Exception ex) { agg_ex.Add(ex); }
            try
            {
                taskPreprocess.Wait();
            }
            catch (Exception ex) { agg_ex.Add(ex); }

            if (agg_ex.Count > 0)
                throw new AggregateException(agg_ex);
        }

        public void Dispose()
        {
            try
            {
                WaitForFinalize();
            }
            finally
            {
                QueueProcess.Dispose();
                QueueStoreResult.Dispose();
                QueueWordToDict.Dispose();
            }
        }
    }
}
