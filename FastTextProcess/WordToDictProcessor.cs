using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Axaprj.WordToVecDB.Context;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;

namespace Axaprj.FastTextProcess
{
    public class WordToDictProcessor : IDisposable
    {
        readonly FastTextProcessDB ProcessDB;

        struct NewItem
        {
            public long Inx;
            public long Freq;
            public long? DictId;
            public DictDbSet.DictKind DictKind;
        }
        long? _curEmbedInx = null;
        readonly object SetOfNewLock = new object();
        Dictionary<LangLabel, Dictionary<string, NewItem>> _setOfNew;


        IEnumerable<LangLabel> GetSetOfNewLangs()
        {
            lock (SetOfNewLock)
            {
                if (_setOfNew == null)
                    return null;
                else 
                    return _setOfNew.Keys;
            }
        }

        Dictionary<string, NewItem> GetSetOfNew(LangLabel lang)
        {
            lock (SetOfNewLock)
            {
                _setOfNew = _setOfNew
                    ?? new Dictionary<LangLabel, Dictionary<string, NewItem>>();

                if (_setOfNew.ContainsKey(lang))
                    return _setOfNew[lang];
                else
                {
                    var res = new Dictionary<string, NewItem>();
                    _setOfNew[lang] = res;
                    return res;
                }
            }
        }
        struct ExistingItem
        {
            public long FreqAdd;
        }
        readonly object SetOfDirtyLock = new object();
        readonly Dictionary<long, ExistingItem> SetOfDirty = new Dictionary<long, ExistingItem>();

        public WordToDictProcessor(string dbf_w2v)
        {
            ProcessDB = new FastTextProcessDB(dbf_w2v);//, foreign_keys:false);
        }

        public long[] WordsToInxsForParallel(string[] words, LangLabel lang)
        {
            var dict = ProcessDB.Dict(DictDbSet.DictKind.Main);
            var dict_addins = ProcessDB.Dict(DictDbSet.DictKind.Addin);
            var embed = ProcessDB.EmbedDict();
            var embed_inxs = new long[words.Length];
            for (int inx = 0; inx < words.Length; inx++)
            {
                embed_inxs[inx] = WordToInx(words[inx], lang, dict, dict_addins, embed);
            }
            return embed_inxs;
        }

        long WordToInx(string word, LangLabel lang, DictDbSet dict, DictDbSet dict_addins, EmbedDictDbSet embed)
        {
            long embed_inx;
            long? cur_id = dict.FindIdByWord(word, lang);
            if (cur_id.HasValue)
            {
                embed_inx = GetOrAddEmbed(embed, cur_id, DictDbSet.DictKind.Main, word, lang);
            }
            else
            {
                cur_id = dict_addins.FindIdByWord(word, lang);
                embed_inx = GetOrAddEmbed(embed, cur_id, DictDbSet.DictKind.Addin, word, lang);
            }
            return embed_inx;
        }

        long GetOrAddEmbed(EmbedDictDbSet embed
            , long? dict_id, DictDbSet.DictKind dict_kind, string word, LangLabel lang)
        {
            long? inx = null;
            if (dict_id.HasValue)
                inx = embed.FindInxById(dict_id.Value, dict_kind);

            if (inx.HasValue)
            {
                var item = new ExistingItem { FreqAdd = 0 };
                lock (SetOfDirtyLock)
                {
                    if (SetOfDirty.ContainsKey(inx.Value))
                        item = SetOfDirty[inx.Value];
                    item.FreqAdd++;
                    SetOfDirty[inx.Value] = item;
                }
            }
            else
            {
                var item = new NewItem { DictKind = dict_kind, DictId = dict_id, Freq = 0 };
                lock (SetOfNewLock)
                {
                    var set_of_new = GetSetOfNew(lang);
                    if (set_of_new.ContainsKey(word))
                        item = set_of_new[word];
                    else
                        item.Inx = GetNextEmbedInx(embed);
                    inx = item.Inx;
                    item.Freq++;
                    set_of_new[word] = item;
                }
            }
            return inx.Value;
        }

        long GetNextEmbedInx(EmbedDictDbSet embed)
        {
            if (!_curEmbedInx.HasValue)
            {
                _curEmbedInx = embed.SelectInxMax();
                if (!_curEmbedInx.HasValue)
                    _curEmbedInx = -1;
            }
            _curEmbedInx = _curEmbedInx.Value + 1;
            return _curEmbedInx.Value;
        }

        public void StoreEmbed()
        {
            var trans = ProcessDB.BeginTransaction();
            try
            {
                var embed = ProcessDB.EmbedDict();
                foreach (long inx in SetOfDirty.Keys)
                {
                    var item = SetOfDirty[inx];
                    embed.IncrementFreq(inx: inx, add_freq: item.FreqAdd);
                }
                var dict_addins = ProcessDB.Dict(DictDbSet.DictKind.Addin);
                var langs = GetSetOfNewLangs();
                if (langs != null)
                {
                    foreach (var lang in langs)
                    {
                        var set_of_new = GetSetOfNew(lang);
                        foreach (string word in set_of_new.Keys)
                        {
                            var item = set_of_new[word];
                            if (!item.DictId.HasValue)
                            {
                                Dict w2v = new Dict { Word = word, Lang = lang };
                                dict_addins.Insert(w2v);
                                item.DictId = w2v.Id;
                            }
                            var ed = new EmbedDict
                            {
                                Inx = item.Inx,
                                DictId = item.DictKind ==
                                    DictDbSet.DictKind.Main ? item.DictId : null,
                                DictAddinsId = item.DictKind ==
                                    DictDbSet.DictKind.Addin ? item.DictId : null,
                                Freq = item.Freq
                            };
                            embed.Insert(ed);
                        }
                    }
                }
                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        public void Dispose()
        {
            ProcessDB.Dispose();
        }

    }
}
