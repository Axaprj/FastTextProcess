using Axaprj.WordToVecDB.Context;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.WordToVecDB
{
    using TCache = Dictionary<LangLabel, WeakReference<ConcurrentDictionary<string, Dict>>>;
    /// <summary>
    /// Vector values access Facade Service.
    /// TAG: thread_safe, cache, performance
    /// </summary>
    public class VectorsService
    {
        static readonly object InstanceLock = new object();
        static VectorsService _instance;
        public static VectorsService Instance(string db_file)
        {
            lock (InstanceLock)
            {
                if (_instance == null ||
                    !_instance._db_file.Equals(db_file))
                {
                    _instance = new VectorsService(db_file);
                }
                return _instance;
            }
        }

        readonly string _db_file;
        protected VectorsService(string db_file) { _db_file = db_file; }

        readonly object VectCacheLock = new object();
        TCache _vectCache;
        ConcurrentDictionary<string, Dict> GetVectDict(LangLabel lang)
        {
            lock (VectCacheLock)
            {
                if (_vectCache == null)
                    _vectCache = new TCache();
                if (_vectCache.ContainsKey(lang))
                {
                    if (_vectCache[lang].TryGetTarget(out ConcurrentDictionary<string, Dict> dict))
                        return dict;
                }
                var new_dict = new ConcurrentDictionary<string, Dict>();
                _vectCache[lang] = new WeakReference<ConcurrentDictionary<string, Dict>>(new_dict);
                return new_dict;
            }
        }

        public Dict FindByWord(string word, LangLabel lang)
        {
            Dict w2v;
            var vect_dict = GetVectDict(lang);
            if (vect_dict.ContainsKey(word))
            {
                if (!vect_dict.TryGetValue(word, out w2v))
                    throw new InvalidOperationException(
                        $"FindByWord('{word}', {lang}) cache get error");
            }
            else
            {
                w2v = DbFindByWord(word, lang);
                if (!vect_dict.TryAdd(word, w2v))
                    throw new InvalidOperationException(
                        $"FindByWord('{word}', {lang}) cache add error");
            }
            return w2v;
        }

        Dict DbFindByWord(string word, LangLabel lang)
        {
            using (var dbx = new FastTextProcessDB(_db_file))
            {
                var dict_db = dbx.Dict(DictDbSet.DictKind.Main);
                var w2v = dict_db.FindByWord(word, lang);
                if (w2v == null)
                {
                    dict_db = dbx.Dict(DictDbSet.DictKind.Addin);
                    w2v = dict_db.FindByWord(word, lang);
                }
                return w2v;
            }
        }
    }
}
