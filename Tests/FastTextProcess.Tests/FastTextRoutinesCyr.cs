using Axaprj.FastTextProcess;
using Axaprj.FastTextProcess.Preprocessor;
using Axaprj.WordToVecDB;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Xunit;
using Xunit.Abstractions;

namespace FastTextProcess.Tests
{
    /// <summary>
    /// Ru Uk En texts aligned vectors processor
    /// </summary>
    public class FastTextRoutinesCyr : FastTextRoutines
    {
        string DBF_W2V_RUK { get { return DataOutPath("w2v_ruk.db"); } }
        string DBF_RUK_Proc { get { return DataOutPath("RUK_proc.db"); } }

        public FastTextRoutinesCyr(ITestOutputHelper output) : base(output) { }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Clean Processing Results")]
        public void ProcResultCleanRuk()
        {
            try
            {
                ProcResultClean(proc_db_fn: DBF_RUK_Proc, dbf_w2v_fn: DBF_W2V_RUK);
                SubProcInsertPredefinedMacro(DBF_W2V_RUK);
            }
            catch (Exception ex) { Log(ex.Message); }
        }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Load PreTrained FastText Database")]
        public void ProcCreateDbRuk()
        {
            ProcCreateDb(DBF_W2V_RUK);
            var cln_proc = new ServiceRoutines();
            bool infilter(Dict w2v)
            {
                var cln_w = cln_proc.GetLettersOnly(w2v.Word, w2v.Lang);
                var reject = string.IsNullOrEmpty(cln_w);
                if (reject)
                    Log($"Skip {w2v}");
                return !reject;
            }
            ProcAppendDb("wiki.ru.align.vec", DBF_W2V_RUK, LangLabel.ru
                , with_insert_or_replace: true, fn_infilter_predicat: infilter);
            ProcAppendDb("wiki.uk.align.vec", DBF_W2V_RUK, LangLabel.uk
                , with_insert_or_replace: true, fn_infilter_predicat: infilter);
            ProcAppendDb("wiki.en.align.vec", DBF_W2V_RUK, LangLabel.en
                , with_insert_or_replace: true, fn_infilter_predicat: infilter);
        }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Append Data (Processing Full)")]
        public void ProcRukBuildFull()
        {
            var conn_str = ConfRoot.GetSection("DataCyrConnStr").Value;
            ProcRukFull(conn_str, "cs", "");
            SubProcFillEmptyVectDictRND(DBF_W2V_RUK, LangLabel.NA);
            SubProcFillEmptyVectDictRND(DBF_W2V_RUK, LangLabel.ru);
            SubProcFillEmptyVectDictRND(DBF_W2V_RUK, LangLabel.uk);
            SubProcFillEmptyVectDictRND(DBF_W2V_RUK, LangLabel.en);
            SubProcBuildResultDict(DBF_RUK_Proc, DBF_W2V_RUK);
            //SubProcInsertPredefinedMacro(DBF_W2V_RUK);
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
                preproc.RunAsync((txt_src, pp_item) =>
                {
                    if (pp_item.Lang == LangLabel.NA)
                        Log($"LANG_DETECT_ERROR>>> {pp_item.Text}");
                    //else
                    //Log($"{pp_item.Lang}>>> {pp_item.Text}"); 
                }
                    );
                Log($"TEST Process samples ...");
                foreach (var keyValue in GetSrcItems(conn_str))
                {
                    preproc.Push(keyValue.Value);
                }
            }
            Log($"TEST Done");
        }

        /// <summary>
        /// Texts data source. Rewrite to connect another source.
        /// </summary>
        /// <returns>yield key-value pairs</returns>
        IEnumerable<KeyValuePair<string, string>> GetSrcItems(string conn_str)
        {
            using (var cn = new SQLiteConnection(conn_str))
            {
                cn.Open();
                var sql = ConfRoot.GetSection("DataCyrSelectSQL").Value;
                var cmd = new SQLiteCommand(sql, cn);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return KeyValuePair.Create(rd.GetString(0), rd.GetString(1));
                }
            }
        }
    }
}
