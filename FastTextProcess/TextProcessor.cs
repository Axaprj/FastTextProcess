using System;
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

        public TextProcessor(string dbf_w2v, int boundedCapacity = 10000)
        {
            QueueProcess = new BlockingCollection<string>(boundedCapacity);
            QueueWordToDict = new BlockingCollection<string[]>(boundedCapacity);

            var preprocess_token_src = new CancellationTokenSource();
            var preprocess_token = preprocess_token_src.Token;

            taskPreprocess = Task.Run(() =>
                {
                    try
                    {
                        var preproc = new TextPreprocess();
                        Parallel.ForEach(QueueProcess.GetConsumingEnumerable(preprocess_token)
                            , (txt) =>
                            {
                                if (preprocess_token.IsCancellationRequested)
                                    preprocess_token.ThrowIfCancellationRequested();
                                QueueWordToDict.Add(preproc.Process(txt), preprocess_token);
                            }
                        );
                        QueueWordToDict.CompleteAdding();
                    }
                    catch
                    {
                        QueueProcess.Dispose();
                    }
                }, preprocess_token
            );
            taskWordToDict = Task.Run(() =>
                {
                    try
                    {
                        using (var wordToDict = new WordToDictProcessor(dbf_w2v))
                        {
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

                            Parallel.ForEach(QueueWordToDict.GetConsumingEnumerable()
                                , (words) => wordToDict.WordsToInxs(words)
                            );
                        }
                    }
                    catch
                    {
                        preprocess_token_src.Cancel();
                        throw;
                    }
                }
            );
        }

        void WaitForFinalize()
        {
            try
            {
                QueueProcess.CompleteAdding();
            }
            catch (ObjectDisposedException) { /* terminated abnormally*/ }
            taskWordToDict.Wait();
            taskPreprocess.Wait();
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
                QueueWordToDict.Dispose();
            }
        }
    }
}
