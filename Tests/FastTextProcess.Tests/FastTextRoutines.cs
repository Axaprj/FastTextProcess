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
            var fvec = Resources.DataArcDir + "cc.en.300.vec";
            AssertFileExists(fvec, "FastText file of vectors");

            AssertFileNotExists(DBF_W2V_EN, "db word2vect");
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
        public void ProcAclImdb()
        {
            using (var proc = new TextProcessor(
                DBF_W2V_EN, DBF_AclImdb, new Preprocessor.CommonEn()))
            {
                var path = Path.GetFullPath(
                    Path.Combine(Resources.DataArcDir, "aclImdb/train/neg/"));
                var dir = new DirectoryInfo(path);
                Assert.True(dir.Exists,
                    $"source folder does not exist: '{path}'");
                var files = dir.GetFiles("*.txt").AsParallel();
                Parallel.ForEach(files, (file) =>
                    {
                        using (var strm = file.OpenText())
                        {
                            proc.Process(strm.ReadToEnd(), src_id: file.Name, proc_info: "neg");
                        }
                    }
                );
            }
        }
    }
}
