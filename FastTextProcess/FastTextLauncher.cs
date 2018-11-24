using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace FastTextProcess
{
    /// <summary>
    /// FastText process launcher
    /// TODO: errors handling
    /// </summary>
    public class FastTextLauncher : IDisposable
    {
        readonly BlockingCollection<Entities.Dict> QueueOut =
             new BlockingCollection<Entities.Dict>();
        readonly BlockingCollection<string> QueueIn =
             new BlockingCollection<string>();
        public readonly ConcurrentBag<string> BagErrors =
             new ConcurrentBag<string>();

        Process FTProc;
        Task taskFTIn;
        Task taskFTOut;
        Task taskFTRes;
        const string CMD_VECT = "print-word-vectors";

        public FastTextLauncher(string path_exe, string path_model)
        {
            FTProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path_exe,
                    Arguments = $" {CMD_VECT} \"{path_model}\"",
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

        public void Push(string word) => QueueIn.Add(word);

        public void RunAsync(Action<Entities.Dict> actHanleResult)
        {
            taskFTOut = Task.Run(() =>
            {
                FTProc.Start();
                taskFTIn = Task.Run(() =>
                {
                    foreach (var word in QueueIn.GetConsumingEnumerable())
                        FTProc.StandardInput.WriteLine(word);
                    FTProc.StandardInput.Close();
                });
                taskFTRes = Task.Run(() =>
                {
                    foreach (var v2w in QueueOut.GetConsumingEnumerable())
                        actHanleResult(v2w);
                });
                string ln;
                while ((ln = FTProc.StandardOutput.ReadLine()) != null)
                    QueueOut.Add(Entities.Dict.Create(ln));
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
                if (taskFTIn != null)
                    taskFTIn.Wait();
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
