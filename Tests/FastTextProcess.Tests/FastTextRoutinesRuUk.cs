using Axaprj.FastTextProcess;
using Axaprj.FastTextProcess.Preprocessor;
using Axaprj.WordToVecDB;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;
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
    /// Ru Ua Fasttext model based processing (non aligned vectors)
    /// </summary>
    public class FastTextRoutinesRuUk : FastTextRoutinesModel
    {
        protected override string DBF_W2V => DataOutPath("w2v_ru_uk.db");
        protected override string DBF_RESULT => DataOutPath("ru_uk_proc.db");
        LangLabel _lang = LangLabel.NA;
        protected override LangLabel LANG => _lang;

        public FastTextRoutinesRuUk(ITestOutputHelper output) : base(output) { }

        [Fact]
        [Trait("Task", "RU UK")]
        [Trait("Process", "Load PreTrained FastText Database")]
        public void ProcCreateRuUkDb()
        {
            ProcCreateDb(DBF_W2V);
            var cln_proc = new ServiceRoutines();
            bool infilter(Dict w2v)
            {
                var cln_w = cln_proc.GetLettersOnly(w2v.Word, w2v.Lang);
                var reject = string.IsNullOrEmpty(cln_w);
                if (reject)
                    Log($"Skip {w2v}");
                return !reject;
            }
            _lang = LangLabel.ru;
            ProcAppendDb(FTF_VECTOR, DBF_W2V, LANG
                , with_insert_or_replace: true, fn_infilter_predicat: infilter);
            _lang = LangLabel.uk;
            ProcAppendDb(FTF_VECTOR, DBF_W2V, LANG
                , with_insert_or_replace: true, fn_infilter_predicat: infilter);
            SubProcInsertPredefinedMacro(DBF_W2V);
        }

        [Fact]
        [Trait("Task", "RU UK")]
        [Trait("Process", "Append Data (Items source processing)")]
        public void ProcRuUkFull()
        {
            Log("Process Samples ...");
            var conn_str = ConfRoot.GetSection("DataCyrConnStr").Value;
            ProcSrcItems(conn_str, "cs", "");
            _lang = LangLabel.ru;
            SubProcFillEmptyVectDict(FTF_MODEL, DBF_W2V, LANG);
            _lang = LangLabel.uk;
            SubProcFillEmptyVectDict(FTF_MODEL, DBF_W2V, LANG);
            _lang = LangLabel.NA;
            SubProcFillEmptyVectDictRND(DBF_W2V, LANG);
            //
            SubProcBuildResultDict(DBF_RESULT, DBF_W2V);
            Log("Done (ProcRuUkFull)");
        }

        void ProcSrcItems(string conn_str, string proc_info, string src_id_pref)
        {
            using (var lang_detector = CreateLangDetector())
            {
                using (var proc = new TextProcessor(
                    DBF_W2V, DBF_RESULT, new CommonEnCyr(lang_detector)))
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
