using FastTextProcess.Context;
using FastTextProcess.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FastTextProcess.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class FastTextRoutines : TestBase
    {
        public FastTextRoutines(ITestOutputHelper output) : base(output) { }
        /// <summary>
        /// Create word to vector db 
        /// </summary>
        /// <param name="ft_vec_fn">FastText vectors filename</param>
        /// <param name="dbf_w2v_fn">DB word to vector filename</param>
        protected void ProcCreateDb(string ft_vec_fn, string dbf_w2v_fn)
        {
            var fvec = DataArcPath(ft_vec_fn);
            AssertFileExists(fvec, "FastText file of vectors");

            AssertFileNotExists(dbf_w2v_fn, "word2vect DB");
            FastTextProcessDB.CreateDB(dbf_w2v_fn);

            using (var dbx = new FastTextProcessDB(dbf_w2v_fn, foreign_keys: false))
            {
                var w2v_tbl = dbx.Dict(DictDbSet.DictKind.Main);
                var trans = dbx.BeginTransaction();
                w2v_tbl.ControlWordsIndex(is_enabled: false);
                using (var sr = new StreamReader(fvec))
                {
                    // header
                    var line = sr.ReadLine();
                    var harr = line.Split(' ');
                    Assert.Equal(2, harr.Length);
                    Log($"'{fvec}': {harr[0]} - samples count, {harr[1]} - sample dim.");
                    // data
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;
                        var w2v = Dict.Create(line);
                        w2v_tbl.Insert(w2v);
                    }
                }
                Log("ControlWordsIndex create...");
                w2v_tbl.ControlWordsIndex(is_enabled: true);
                Log("Done");
                trans.Commit();
            }
        }
        /// <summary>
        /// Cleanup result DB
        /// </summary>
        /// <param name="proc_db_fn">Result Db filename</param>
        /// <param name="dbf_w2v_fn">DB word to vector filename</param>
        protected void ProcResultClean(string proc_db_fn, string dbf_w2v_fn)
        {
            if (File.Exists(proc_db_fn))
            {
                File.Delete(proc_db_fn);
                Log($"'{proc_db_fn}' deleted");
            }
            AssertFileExists(dbf_w2v_fn, "word2vect DB");
            using (var dbx = new FastTextProcessDB(dbf_w2v_fn))
            {
                dbx.EmbedDict().DeleteAll();
                dbx.Dict(DictDbSet.DictKind.Addin).DeleteAll();
            }
        }
        /// <summary>
        /// Fill Empty Add-in Dictionary Vectors
        /// </summary>
        /// <param name="ft_bin_fn">FastText bin model filename</param>
        /// <param name="dbf_w2v_fn">DB word to vector filename</param>
        protected void SubProcFillEmptyVectDict(string ft_bin_fn, string dbf_w2v_fn)
        {
            using (var dbx = new FastTextProcessDB(dbf_w2v_fn))
            {
                var words = dbx.Dict(DictDbSet.DictKind.Addin).GetWordsWithEmptyVect();
                if (words.Any())
                {
                    var fmod = DataArcPath(ft_bin_fn);
                    AssertFileExists(fmod, "FastText model file");
                    var fexe = FastTextBin;
                    AssertFileExists(fexe, "FastText executable");
                    var trans = dbx.BeginTransaction();
                    try
                    {
                        var dict = dbx.Dict(DictDbSet.DictKind.Addin);
                        using (var ftl = new FastTextLauncher(fexe, fmod))
                        {
                            ftl.RunAsync((w2v) => dict.UpdateVectOfWord(w2v));
                            foreach (var w in words)
                                ftl.Push(w);
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// Build result DB dictionary
        /// </summary>
        /// <param name="proc_db_fn">Result Db filename</param>
        /// <param name="dbf_w2v_fn">DB word to vector filename</param>
        protected void SubProcBuildResultDict(string proc_db_fn, string dbf_w2v_fn)
        {
            using (var dbx_src = new FastTextProcessDB(dbf_w2v_fn))
            {
                using (var dbx_dst = new FastTextResultDB(proc_db_fn))
                {
                    var tran = dbx_dst.BeginTransaction();
                    try
                    {
                        var inx_old = dbx_dst.GetDictInxMax();
                        long inx_check = inx_old.HasValue ? inx_old.Value + 1 : 0;
                        dbx_src.ProcessEmbedJoins((itm) =>
                        {
                            Assert.Equal(inx_check, itm.Inx);
                            dbx_dst.StoreDictItem(itm);
                            inx_check++;
                        }, from_inx: inx_check);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// Insert Predefined Macro vectors
        /// </summary>
        /// <param name="dbf_w2v_fn">DB word to vector filename</param>
        protected void SubProcInsertPredefinedMacro(string dbf_w2v_fn)
        {
            using (var dbx_src = new FastTextProcessDB(dbf_w2v_fn))
            {
                var vect_empty = Dict.CreateEmpty();
                //var vect_fl = Dict.GetVectFloat(vect_empty.Vect);
                var dict = dbx_src.Dict(DictDbSet.DictKind.Addin);
                long? vect_empty_id = dict.FindIdByWord(vect_empty.Word);
                if (vect_empty_id.HasValue)
                    vect_empty.Id = vect_empty_id.Value;
                else
                    dict.Insert(vect_empty);
                var embed_dict = dbx_src.EmbedDict();
                long? ed_inx = embed_dict.FindInxById(vect_empty.Id, DictDbSet.DictKind.Addin);
                if (ed_inx.HasValue)
                    Assert.True(ed_inx.Value == 0,
                        $"'{vect_empty.Word}' dictionary index should be Zero");
                else
                {
                    var ed = new EmbedDict { Inx = 0, DictAddinsId = vect_empty.Id };
                    embed_dict.Insert(ed);
                }
            }
        }
    }
}
