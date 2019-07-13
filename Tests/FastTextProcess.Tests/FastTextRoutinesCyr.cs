using FastTextProcess.Context;
using FastTextProcess.Entities;
using FastTextProcess.Enums;
using FastTextProcess.Preprocessor;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FastTextProcess.Tests
{
    /// <summary>
    /// Ru Uk En texts processor
    /// </summary>
    public class FastTextRoutinesCyr : FastTextRoutines
    {
        const string DBF_W2V_RUK = "w2v_ruk.db";
        const string DBF_RUK_Proc = "RUK_proc.db";
        public FastTextRoutinesCyr(ITestOutputHelper output) : base(output) { }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Clean Processing Results")]
        public void ProcResultCleanRuk()
        {
            try { ProcResultClean(DBF_W2V_RUK, DBF_RUK_Proc); }
            catch (Exception ex) { Log(ex.Message); }
        }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Load PreTrained FastText Database")]
        public void ProcCreateDbRuk()
        {
            ProcCreateDb("wiki.ru.align.vec", DBF_W2V_RUK, FTLangLabel.__label__ru, with_insert_or_replace: true);
            ProcAppendDb("wiki.uk.align.vec", DBF_W2V_RUK, FTLangLabel.__label__uk, with_insert_or_replace: true);
            ProcAppendDb("wiki.en.align.vec", DBF_W2V_RUK, FTLangLabel.__label__en, with_insert_or_replace: true);
            SubProcInsertPredefinedMacro(DBF_W2V_RUK);
        }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Append Data (Processing Full)")]
        public void ProcRukBuildFull()
        {
            var conn_str = ConfRoot.GetSection("DataCyrConnStr").Value;
            ProcRukFull(conn_str, "cs", "");
            SubProcFillEmptyVectDictRND(DBF_W2V_RUK, FTLangLabel.__label__ru);
            SubProcFillEmptyVectDictRND(DBF_W2V_RUK, FTLangLabel.__label__uk);
            SubProcFillEmptyVectDictRND(DBF_W2V_RUK, FTLangLabel.__label__en);
            SubProcBuildResultDict(DBF_RUK_Proc, DBF_W2V_RUK);
            SubProcInsertPredefinedMacro(DBF_W2V_RUK);
        }

        void ProcRukFull(string conn_str, string proc_info, string src_id_pref)
        {

            using (var lang_detector = CreateLangDetector())
            {
                using (var proc = new TextProcessor(
                    DBF_W2V_RUK, DBF_RUK_Proc, new CommonEnCyr(lang_detector)))
                {
                    Log($"Process samples '{proc_info}' ...");
                    foreach (var keyValue in GetSrcItems(conn_str))
                    {
                        proc.Process(keyValue.Value, src_id: src_id_pref + keyValue.Key
                                , proc_info: proc_info);
                    }
                }
            }
            Log($"Done ({proc_info})");
        }

        [Fact]
        [Trait("Task", "RUK Lang Detector test")]
        [Trait("Process", "LangDetector/DataSource test")]
        public void TestLangDetector()
        {
            var conn_str = ConfRoot.GetSection("DataCyrConnStr").Value;
            using (var lang_detector = CreateLangDetector())
            {
                var preproc = new CommonEnCyr(lang_detector);
                preproc.RunAsync((txt_src, pp_item)
                    => Log($"{pp_item.Lang}>>> {pp_item.Text}"));
                Log($"TEST Process samples ...");
                foreach (var keyValue in GetSrcItems(conn_str))
                {
                    preproc.Push(keyValue.Value);
                }
            }
            Log($"TEST Done");
        }

        FastTextLauncher CreateLangDetector()
        {
            var fmod = DataArcPath("lid.176.bin");
            AssertFileExists(fmod, "FastText model file");
            var fexe = FastTextBin;
            AssertFileExists(fexe, "FastText executable");
            var lang_detector = FTCmd.CreateLangDetect(fexe, fmod);
            return lang_detector;
        }

        /// <summary>
        /// Texts data source. Rewrite to connect another source.
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<long, string>> GetSrcItems(string conn_str)
        {
            using (var cn = new SQLiteConnection(conn_str))
            {
                cn.Open();
                var sql = ConfRoot.GetSection("DataCyrSelectSQL").Value;
                var cmd = new SQLiteCommand(sql, cn);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return KeyValuePair.Create(rd.GetInt64(0), rd.GetString(1));
                }
            }
        }
    }
}
