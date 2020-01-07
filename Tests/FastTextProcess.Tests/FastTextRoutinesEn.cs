using Axaprj.FastTextProcess;
using Axaprj.WordToVecDB.Enums;
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
    /// AclImdb English processing
    /// </summary>
    public class FastTextRoutinesEn : FastTextRoutinesModel
    {
        protected override string DBF_W2V => DataOutPath("w2v_en.db");
        protected override string DBF_RESULT => DataOutPath("AclImdb_proc.db");
        protected override LangLabel LANG => LangLabel.en;

        public FastTextRoutinesEn(ITestOutputHelper output) : base(output) { }

        [Fact]
        [Trait("Task", "AclImdb")]
        [Trait("Process", "Append Train Data (Processing Full)")]
        public void ProcAclImdbTrain()
        {
            Log("Process Train Samples ...");
            var path = DataArcPath("aclImdb/train/neg/");
            ProcAclImdbFull(data_dir: path, proc_info: "1 0", src_id_pref: "train/neg/");
            path = DataArcPath("aclImdb/train/pos/");
            ProcAclImdbFull(data_dir: path, proc_info: "0 1", src_id_pref: "train/pos/");
            SubProcFillEmptyVectDict(FTF_MODEL, DBF_W2V, LANG);
            SubProcBuildResultDict(DBF_RESULT, DBF_W2V);
            Log("Done (ProcAclImdbTrain)");
        }

        [Fact]
        [Trait("Task", "AclImdb")]
        [Trait("Process", "Append Tests Data (Processing Full)")]
        public void ProcAclImdbTest()
        {
            Log("Process Test Samples ...");
            var path = DataArcPath("aclImdb/test/neg/");
            ProcAclImdbFull(data_dir: path, proc_info: "1 0", src_id_pref: "test/neg/");
            path = DataArcPath("aclImdb/test/pos/");
            ProcAclImdbFull(data_dir: path, proc_info: "0 1", src_id_pref: "test/pos/");
            SubProcFillEmptyVectDict(FTF_MODEL, DBF_W2V, LANG);
            SubProcBuildResultDict(DBF_RESULT, DBF_W2V);
            Log("Done (ProcAclImdbTest)");
        }

        void ProcAclImdbFull(string data_dir, string proc_info, string src_id_pref)
        {
            SubProcInsertPredefinedMacro(DBF_W2V);
            using (var proc = new TextProcessor(
                DBF_W2V, DBF_RESULT, new Axaprj.FastTextProcess.Preprocessor.CommonEn()))
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
                            , src_id: src_id_pref + file.Name
                            , proc_info: proc_info);
                    }
                }
                );
            }
            Log($"Done ({src_id_pref})");
        }
    }
}
