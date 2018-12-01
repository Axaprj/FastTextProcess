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
    public class FastTextRoutines : TestBase
    {
        const string DBF_W2V_EN = "w2v_en.db";
        const string DBF_AclImdb = "AclImdb_proc.db";
        public FastTextRoutines(ITestOutputHelper output) : base(output) { }

        [Fact]
        [Trait("Task", "EN Common")]
        [Trait("Process", "Load PreTrained FastText Database")]
        public void ProcCreateDbEn()
        {
            var fvec = DataArcPath("cc.en.300.vec");
            AssertFileExists(fvec, "FastText file of vectors");

            AssertFileNotExists(DBF_W2V_EN, "word2vect En-Common DB");
            FastTextProcessDB.CreateDB(DBF_W2V_EN);

            using (var dbx = new FastTextProcessDB(DBF_W2V_EN, foreign_keys: false))
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

        #region ProcAclImdb
        [Fact]
        [Trait("Task", "AclImdb")]
        [Trait("Process", "Append Train Data (Processing Full)")]
        public void ProcAclImdbTrain()
        {
            SubProcAclImdbInsertPredefinedMacro();
            using (var proc = new TextProcessor(
                DBF_W2V_EN, DBF_AclImdb, new Preprocessor.CommonEn()))
            {
                Log("Process Negative samples ...");
                var path = DataArcPath("aclImdb/train/neg/");
                ProcAclImdbDir(proc, path, proc_info: "1 0");
                Log("Process Positive samples ...");
                path = DataArcPath("aclImdb/train/pos/");
                ProcAclImdbDir(proc, path, proc_info: "0 1");
            }
            SubProcFillEmptyVectDictEn();
            SubProcAclImdbDictEn();
            Log("Done");
        }

        [Fact]
        [Trait("Task", "AclImdb")]
        [Trait("Process", "Append Tests Data (Processing Full)")]
        public void ProcAclImdbTest()
        {
            var path = DataArcPath("aclImdb/test/neg/");
            ProcAclImdbFull(data_dir: path, proc_info: "1 0", src_id_pref:"test/neg");
            path = DataArcPath("aclImdb/test/pos/");
            ProcAclImdbFull(data_dir: path, proc_info: "0 1", src_id_pref: "test/pos");
            Log("Done (ProcAclImdbTest)");
        }

        void ProcAclImdbFull(string data_dir, string proc_info, string src_id_pref)
        {
            SubProcAclImdbInsertPredefinedMacro();
            using (var proc = new TextProcessor(
                DBF_W2V_EN, DBF_AclImdb, new Preprocessor.CommonEn()))
            {
                Log($"Process samples '{src_id_pref}' ...");
                var dir_path = DataArcPath(data_dir);
                var dir = new DirectoryInfo(dir_path);
                Assert.True(dir.Exists,
                    $"source folder does not exist: '{dir_path}'");
                var files = dir.GetFiles("*.txt").AsParallel();
                Parallel.ForEach(files, (file) =>
                {
                    using (var strm = file.OpenText())
                    {
                        proc.Process(strm.ReadToEnd()
                            , src_id: src_id_pref + "/" + file.Name
                            , proc_info: proc_info);
                    }
                }
                );
            }
            SubProcFillEmptyVectDictEn();
            SubProcAclImdbDictEn();
            Log($"Done ({src_id_pref})");
        }

        void ProcAclImdbDir(TextProcessor proc, string path, string proc_info)
        {
            var dir = new DirectoryInfo(path);
            Assert.True(dir.Exists,
                $"source folder does not exist: '{path}'");
            var files = dir.GetFiles("*.txt").AsParallel();
            Parallel.ForEach(files, (file) =>
            {
                using (var strm = file.OpenText())
                {
                    proc.Process(strm.ReadToEnd()
                        , src_id: proc_info + "/" + file.Name
                        , proc_info: proc_info);
                }
            }
            );
        }
        #endregion

        [Fact]
        [Trait("Task", "AclImdb")]
        [Trait("Process", "Clean Processing Results")]
        public void ProcAclImdbResultClean()
        {
            if (File.Exists(DBF_AclImdb))
            {
                File.Delete(DBF_AclImdb);
                Log($"'{DBF_AclImdb}' deleted");
            }
            AssertFileExists(DBF_W2V_EN, "word2vect En-Common DB");
            using (var dbx = new FastTextProcessDB(DBF_W2V_EN))
            {
                dbx.EmbedDict().DeleteAll();
                dbx.Dict(DictDbSet.DictKind.Addin).DeleteAll();
            }
        }

        [Fact]
        [Trait("Task", "EN Common")]
        [Trait("SubProcess", "Fill Empty Add-in Dictionary Vectors")]
        public void SubProcFillEmptyVectDictEn()
        {
            using (var dbx = new FastTextProcessDB(DBF_W2V_EN))
            {
                var words = dbx.Dict(DictDbSet.DictKind.Addin).GetWordsWithEmptyVect();
                if (words.Any())
                {
                    var fmod = DataArcPath("cc.en.300.bin");
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

        [Fact]
        [Trait("Task", "AclImdb")]
        [Trait("SubProcess", "Build Result Dictionary")]
        public void SubProcAclImdbDictEn()
        {
            using (var dbx_src = new FastTextProcessDB(DBF_W2V_EN))
            {
                using (var dbx_dst = new FastTextResultDB(DBF_AclImdb))
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

        [Fact]
        [Trait("Task", "EN Common")]
        [Trait("SubProcess", "Insert Predefined Vectors")]
        public void SubProcAclImdbInsertPredefinedMacro()
        {
            using (var dbx_src = new FastTextProcessDB(DBF_W2V_EN))
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
