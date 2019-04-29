using FastTextProcess.Preprocessor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FastTextProcess
{
    /// <summary>
    /// FastText command arguments, helper, factory
    /// </summary>
    public static class FTCmd
    {
        /// <summary> print word vectors given a trained model </summary>
        internal const string CMD_VECT = "print-word-vectors";
        /// <summary> predict most likely labels </summary>
        internal const string CMD_PREDICT = "predict";
        /// <summary> predict most likely labels with probabilities </summary>
        internal const string CMD_PREDICT_PROB = "predict-prob";
        /// <summary> Processor Factory: word to vector </summary>
        public static FastTextLauncher<Entities.Dict> CreateW2V(string path_exe, string path_model) =>
            new FastTextLauncher<Entities.Dict>(path_exe, path_model, CMD_VECT);
        /// <summary> Processor Factory: language detector </summary>
        public static FastTextLauncher<PreprocessItem> CreateLangDetect(string path_exe, string path_model) =>
            new FastTextLauncher<PreprocessItem>(path_exe, path_model, CMD_PREDICT, "-");
    }
    /// <summary>
    /// FastText process launcher
    /// TODO: errors handling
    /// </summary>
    public class FastTextLauncher<TResult> : IDisposable
    {
        readonly BlockingCollection<TResult> QueueOut =
             new BlockingCollection<TResult>();
        readonly BlockingCollection<string> QueueIn =
             new BlockingCollection<string>();
        public readonly ConcurrentBag<string> BagErrors =
             new ConcurrentBag<string>();

        Process FTProc;
        //Task taskFTIn;
        Task taskFTOut;
        Task taskFTRes;

        internal FastTextLauncher(string path_exe, string path_model, string cmd, string additional_args = "")
        {
            FTProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path_exe,
                    Arguments = $" {cmd} \"{path_model}\" {additional_args}",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            FTProc.ErrorDataReceived += new DataReceivedEventHandler(
                (_, e) => BagErrors.Add(e.Data));
        }

        public void Push(string txt) => QueueIn.Add(txt);

        public void Push(string[] txts)
        {
            foreach (var txt in txts)
                QueueIn.Add(txt);
        }

        //public void RunAsync(Action<TResult> actHanleResult, Func<string, TResult> fnPostProcess)
        //{
        //    taskFTOut = Task.Run(() =>
        //    {
        //        FTProc.Start();
        //        taskFTIn = Task.Run(() =>
        //        {
        //            using (var writer = new StreamWriter(FTProc.StandardInput.BaseStream, Encoding.UTF8))
        //            {
        //                foreach (var txt in QueueIn.GetConsumingEnumerable())
        //                    writer.WriteLine(txt);
        //            }
        //            //FTProc.StandardInput.Close();
        //        });
        //        taskFTRes = Task.Run(() =>
        //        {
        //            foreach (var res_out in QueueOut.GetConsumingEnumerable())
        //                actHanleResult(res_out);
        //        });
        //        string ln;
        //        while ((ln = FTProc.StandardOutput.ReadLine()) != null)
        //            QueueOut.Add(fnPostProcess(ln));
        //        QueueOut.CompleteAdding();
        //        FTProc.WaitForExit();
        //    });
        //}

        public void RunByLineAsync(Action<TResult> actHanleResult, Func<string, string, TResult> fnPostProcess)
        {
                taskFTOut = Task.Run(() =>
                {
                    FTProc.Start();
                    taskFTRes = Task.Run(() =>
                    {
                        foreach (var res_out in QueueOut.GetConsumingEnumerable())
                            actHanleResult(res_out);
                    });
                    using (var writer = new StreamWriter(FTProc.StandardInput.BaseStream, Encoding.UTF8))
                    {
                        foreach (var txt in QueueIn.GetConsumingEnumerable())
                        {
                            var task_in = Task.Run(() =>
                            {
                                writer.WriteLine(txt);
                                writer.Flush();
                            });
                            task_in.Wait();
                            string res_ln = FTProc.StandardOutput.ReadLine();
                            QueueOut.Add(fnPostProcess(txt, res_ln));
                        }
                    }
                    QueueOut.CompleteAdding();
                    FTProc.WaitForExit();
                });
        }

        void IDisposable.Dispose()
        {
                QueueIn.CompleteAdding();
                try
                {
                    if (taskFTOut != null)
                        taskFTOut.Wait();
                    //if (taskFTIn != null)
                    //    taskFTIn.Wait();
                    if (taskFTRes != null)
                        taskFTRes.Wait();
                }
                finally
                {
                    QueueIn.Dispose();
                    QueueOut.Dispose();
                    if (FTProc.HasExited)
                        FTProc.Close();
                    else
                        FTProc.Kill();
                }
            }
        }
    }
