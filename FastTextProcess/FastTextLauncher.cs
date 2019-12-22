using Axaprj.WordToVecDB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Axaprj.FastTextProcess
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
        public static FastTextLauncher CreateW2V(string path_exe, string path_model) =>
            new FastTextLauncher(path_exe, path_model, CMD_VECT);
        /// <summary> Processor Factory: language detector </summary>
        public static FastTextLauncher CreateLangDetect(string path_exe, string path_model) =>
            new FastTextLauncher(path_exe, path_model, CMD_PREDICT, "-");
    }
    /// <summary>
    /// FastText process launcher
    /// TODO: errors handling
    /// </summary>
    public class FastTextLauncher : IDisposable
    {
        readonly BlockingCollection<ITextSource> QueueIn =
             new BlockingCollection<ITextSource>();
        public readonly ConcurrentBag<string> BagErrors =
             new ConcurrentBag<string>();

        Process FTProc;
        Task taskFTOut;

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

        public void CompleteAdding()
        {
            QueueIn.CompleteAdding();
            taskFTOut.Wait();
        }

        public void Push(ITextSource txt_src) => QueueIn.Add(txt_src);

        public void Push(string txt) => Push(new TextSourceStub(txt));

        public void RunByLineAsync(Action<ITextSource, string> actHandleResult)
        {
            taskFTOut = Task.Run(() =>
            {
                FTProc.Start();
                using (var writer = new StreamWriter(FTProc.StandardInput.BaseStream, Encoding.UTF8))
                {
                    foreach (var txt_src in QueueIn.GetConsumingEnumerable())
                    {
                        var task_in = Task.Run(() =>
                        {
                            writer.WriteLine(txt_src.GetText());
                            writer.Flush();
                        });
                        task_in.Wait();
                        string res_ln = FTProc.StandardOutput.ReadLine();
                        actHandleResult(txt_src, res_ln);
                    }
                }
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
            }
            finally
            {
                QueueIn.Dispose();
                if (FTProc.HasExited)
                    FTProc.Close();
                else
                    FTProc.Kill();
            }
        }
    }
}
