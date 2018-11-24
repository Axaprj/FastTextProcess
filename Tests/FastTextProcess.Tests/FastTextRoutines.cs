using FastTextProcess.Context;
using FastTextProcess.Entities;
using FastTextProcess.Tests.Properties;
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
        public void ProcCreateDbEn()
        {
            var fvec = Path.Combine(Resources.DataArcDir, "cc.en.300.vec");
            AssertFileExists(fvec, "FastText file of vectors");

            AssertFileNotExists(DBF_W2V_EN, "word2vect En common DB");
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

        [Fact]
        public void SubProcFillEmptyVectDictEn()
        {
            using (var dbx = new FastTextProcessDB(DBF_W2V_EN))
            {
                var words = dbx.Dict(DictDbSet.DictKind.Addin).GetWordsWithEmptyVect();
                if (words.Any())
                {
                    var fmod = Path.Combine(Resources.DataArcDir, "cc.en.300.bin");
                    AssertFileExists(fmod, "FastText model file");
                    var fexe = Resources.FastTextBin;
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
        public void ProcAclImdb()
        {
            using (var proc = new TextProcessor(
                DBF_W2V_EN, DBF_AclImdb, new Preprocessor.CommonEn()))
            {
                Log("Process Negative samples ...");
                var path = Path.GetFullPath(
                    Path.Combine(Resources.DataArcDir, "aclImdb/train/neg/"));
                ProcAclImdbDir(proc, path, proc_info: "neg");
                Log("Process Positive samples ...");
                path = Path.GetFullPath(
                    Path.Combine(Resources.DataArcDir, "aclImdb/train/pos/"));
                ProcAclImdbDir(proc, path, proc_info: "pos");
            }
            SubProcFillEmptyVectDictEn();
            SubProcAclImdbDictEn();
            Log("Done");
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

        [Fact]
        public void ProcAclImdbResultClean()
        {
            if (File.Exists(DBF_AclImdb))
            {
                File.Delete(DBF_AclImdb);
                Log($"'{DBF_AclImdb}' deleted");
            }
            AssertFileExists(DBF_W2V_EN, "word2vect En common DB");
            using (var dbx = new FastTextProcessDB(DBF_W2V_EN))
            {
                dbx.EmbedDict().DeleteAll();
                dbx.Dict(DictDbSet.DictKind.Addin).DeleteAll();
            }
        }
    }
}
