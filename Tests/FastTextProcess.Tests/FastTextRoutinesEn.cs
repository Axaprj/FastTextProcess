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
    public class FastTextRoutinesEn : FastTextRoutines
    {
        string DBF_W2V_EN { get { return DataOutPath("w2v_en.db"); } }
        string DBF_AclImdb { get { return DataOutPath("AclImdb_proc.db"); } }

        public FastTextRoutinesEn(ITestOutputHelper output) : base(output) { }

        [Fact]
        [Trait("Task", "EN Common")]
        [Trait("Process", "Load PreTrained FastText Database")]
        public void ProcCreateDbEn()
        {
            ProcCreateDb("cc.en.300.vec", DBF_W2V_EN, FTLangLabel.__label__en);
        }

        #region AclImdb processing tasks
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
            SubProcFillEmptyVectDictEn();
            SubProcAclImdbResultDictEn();
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
            SubProcFillEmptyVectDictEn();
            SubProcAclImdbResultDictEn();
            Log("Done (ProcAclImdbTest)");
        }

        void ProcAclImdbFull(string data_dir, string proc_info, string src_id_pref)
        {
            SubProcAclImdbInsertPredefinedMacro();
            using (var proc = new TextProcessor(
                DBF_W2V_EN, DBF_AclImdb, new Axaprj.FastTextProcess.Preprocessor.CommonEn()))
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
        #endregion

        [Fact]
        [Trait("Task", "AclImdb")]
        [Trait("Process", "Clean Processing Results")]
        public void ProcAclImdbResultClean()
        {
            ProcResultClean(DBF_AclImdb, DBF_W2V_EN);
        }

        [Fact]
        [Trait("Task", "EN Common")]
        [Trait("SubProcess", "Fill Empty Add-in Dictionary Vectors (via FastTest)")]
        public void SubProcFillEmptyVectDictEn()
        {
            SubProcFillEmptyVectDict("cc.en.300.bin", DBF_W2V_EN, FTLangLabel.__label__en);
        }

        [Fact]
        [Trait("Task", "AclImdb")]
        [Trait("SubProcess", "Build Result Dictionary")]
        public void SubProcAclImdbResultDictEn()
        {
            SubProcBuildResultDict(DBF_AclImdb, DBF_W2V_EN);
        }

        [Fact]
        [Trait("Task", "EN Common")]
        [Trait("SubProcess", "Insert Predefined Vectors")]
        public void SubProcAclImdbInsertPredefinedMacro()
        {
            SubProcInsertPredefinedMacro(DBF_W2V_EN);
        }
    }
}
