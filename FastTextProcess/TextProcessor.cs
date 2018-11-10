using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FastTextProcess
{
    public class TextProcessor : IDisposable
    {
        public readonly BlockingCollection<string> QueuePreprocess = new BlockingCollection<string>();
        readonly BlockingCollection<string[]> QueueStore = new BlockingCollection<string[]>();

        readonly TextPreprocess Preprocess = new TextPreprocess();
        Task _taskPreprocess;

        public TextProcessor()
        {
            _taskPreprocess = Task.Run(() =>
            {
                Parallel.ForEach(
                    QueuePreprocess.GetConsumingEnumerable()
                    , txt => QueueStore.Add(Preprocess.Process(txt))
                );
                QueueStore.CompleteAdding();
            }
            );
        }

        public void WaitForFinalize()
        {
            QueuePreprocess.CompleteAdding();
            _taskPreprocess.Wait();
        }

        public void Dispose()
        {
            WaitForFinalize();
            QueuePreprocess.Dispose();
            QueueStore.Dispose();
        }
    }
}
