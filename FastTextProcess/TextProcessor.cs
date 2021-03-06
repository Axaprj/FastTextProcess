﻿using Axaprj.WordToVecDB.Context;
using Axaprj.WordToVecDB.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Axaprj.FastTextProcess
{
    public class TextProcessor : IDisposable
    {
        /// <summary>
        /// PostProcess handler (optional)
        /// </summary>
        public Func<ProcessItem, ProcessItem> fnPostProcess;

        readonly BlockingCollection<ProcessItem> QueueProcess;
        readonly Task taskPreprocess;

        readonly BlockingCollection<ProcessItem> QueueWordToDict;
        readonly Task taskWordToDict;

        readonly BlockingCollection<ProcessItem> QueueStoreResult;
        readonly Task taskStoreResult;

        readonly CancellationTokenSource CancelTokenSrc;

        public TextProcessor(string dbf_w2v, string dbf_res
            , Preprocessor.ITextPreprocess preprocessor
            , int boundedCapacity = 10000)
        {
            QueueProcess = new BlockingCollection<ProcessItem>(boundedCapacity);
            QueueWordToDict = new BlockingCollection<ProcessItem>(boundedCapacity);
            QueueStoreResult = new BlockingCollection<ProcessItem>(boundedCapacity);
            CancelTokenSrc = new CancellationTokenSource();
            var cancel_token = CancelTokenSrc.Token;
            taskPreprocess = Task.Run(() =>
            {
                try
                {
                    preprocessor.RunAsync((txt_src, prep_itm) =>
                        {
                            var itm = (ProcessItem)txt_src;
                            itm.Preprocessed = prep_itm.Words;
                            itm.Lang = prep_itm.Lang;
                            QueueWordToDict.Add(itm, cancel_token);
                        }
                    );
                    Parallel.ForEach(
                       QueueProcess.GetConsumingEnumerable(cancel_token)
                       , (itm) => preprocessor.Push(itm)
                    );
                    preprocessor.CompleteAdding();
                    QueueWordToDict.CompleteAdding();
                }
                catch
                {
                    CancelTokenSrc.Cancel();
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
                            , (itm) =>
                            {
                                itm.Embedded = wordToDict.WordsToInxsForParallel(itm.Preprocessed, itm.Lang);
                                QueueStoreResult.Add(itm);
                            }
                        );
                        wordToDict.StoreEmbed();
                        QueueStoreResult.CompleteAdding();
                    }
                }
                catch
                {
                    CancelTokenSrc.Cancel();
                    throw;
                }
            }, cancel_token
            );
            taskStoreResult = Task.Run(() =>
            {
                FastTextResultDB.CreateIfNotExistsDB(dbf_res);
                using (var res_dbx = new FastTextResultDB(dbf_res))
                {
                    var tran = res_dbx.BeginTransaction();
                    try
                    {
                        foreach (var itm in QueueStoreResult.GetConsumingEnumerable(cancel_token))
                        {
                            var store_itm = fnPostProcess == null ? itm : fnPostProcess(itm);
                            res_dbx.StoreProcessItem(store_itm);
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        CancelTokenSrc.Cancel();
                        throw;
                    }
                }
            }, cancel_token
            );
        }

        public void Process(string src, string src_id, string proc_info)
        {
            if (CancelTokenSrc.IsCancellationRequested)
                throw new InvalidOperationException(
                    $"Processing was canceled. Terminated before process '{src_id}' source.");
            var item = new ProcessItem
            {
                Src = src,
                SrcOriginalId = src_id,
                SrcProcInfo = proc_info
            };
            QueueProcess.Add(item);
        }

        void WaitForFinalize()
        {
            var agg_ex = new List<Exception>();
            try
            {
                QueueProcess.CompleteAdding();
                taskPreprocess.Wait();
            }
            catch (Exception ex) { agg_ex.Add(ex); }
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
