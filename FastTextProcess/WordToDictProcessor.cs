using System;
using System.Collections.Generic;
using FastTextProcess.Context;

namespace FastTextProcess
{
    public class WordToDictProcessor : IDisposable
    {
        readonly FastTextProcessDB ProcessDB;

        public WordToDictProcessor(string dbf_w2v)
        {
            ProcessDB = new FastTextProcessDB(dbf_w2v);
        }

        public List<long[]> Process(List<string[]> words_list)
        {
            var dict = ProcessDB.Dict(DictDbSet.DictKind.Main);
            var dict_addins = ProcessDB.Dict(DictDbSet.DictKind.Addin);
            var res = new List<long[]>(words_list.Count);
            foreach (var words in words_list)
            {
                res.Add(Process(dict, dict_addins, words));
            }
            return res;
        }

        public long[] Process(string[] words)
        {
            var dict = ProcessDB.Dict(DictDbSet.DictKind.Main);
            var dict_addins = ProcessDB.Dict(DictDbSet.DictKind.Addin);
            return Process(dict, dict_addins, words);
        }

        static long[] Process(DictDbSet dict, DictDbSet dict_addins, string[] words)
        {
            var res_ids = new long[words.Length];
            for (int inx = 0; inx < words.Length; inx++)
            {
                long? cur_id = dict_addins.FindIdByWord(words[inx]);
                if (!cur_id.HasValue)
                    cur_id = dict.FindIdByWord(words[inx]);
                res_ids[inx] = cur_id.HasValue ? cur_id.Value : 0;
            }
            return res_ids;
        }

        public void Dispose()
        {
            ProcessDB.Dispose();
        }
    }
}
