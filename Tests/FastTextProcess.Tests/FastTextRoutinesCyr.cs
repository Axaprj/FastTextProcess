using FastTextProcess.Context;
using FastTextProcess.Entities;
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
    /// Ru Uk texts processor
    /// </summary>
    public class FastTextRoutinesCyr : FastTextRoutines
    {
        const string DBF_W2V_RU = "w2v_ru.db";
        const string DBF_W2V_UK = "w2v_uk.db";
        const string DBF_RUK_Proc = "RUK_proc.db";
        public FastTextRoutinesCyr(ITestOutputHelper output) : base(output) { }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Clean Processing Results")]
        public void ProcResultCleanRuk()
        {
            try { ProcResultClean(DBF_W2V_RU, DBF_RUK_Proc); }
            catch (Exception ex) { Log(ex.Message); }
            try { ProcResultClean(DBF_W2V_UK, DBF_RUK_Proc); }
            catch (Exception ex) { Log(ex.Message); }
        }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Load PreTrained FastText Database")]
        public void ProcCreateDbRuk()
        {
            ProcCreateDb("wiki.ru.vec", DBF_W2V_RU, with_insert_or_replace: true);
            SubProcInsertPredefinedMacro(DBF_W2V_RU);
            ProcCreateDb("wiki.uk.vec", DBF_W2V_UK, with_insert_or_replace: true);
            SubProcInsertPredefinedMacro(DBF_W2V_UK);
        }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Append Data (Processing Full)")]
        public void ProcRukBuildFull()
        {
            ProcRukFull("", "", "");
        }

        void ProcRukFull(string data_dir, string proc_info, string src_id_pref)
        {
            using (var lang_detector = CreateLangDetector())
            {
                var preproc = new CommonEnCyr();
                Log($"Process samples '{src_id_pref}' ...");
                lang_detector.RunByLineAsync(
                      (item) => Log(item.Lang.ToString() + ">>> " + item.Text)
                    , (txt, txt_lbl) => new LangDetectItem(txt, txt_lbl));
                foreach (string txt in GetSrcItems())
                {
                    var words = preproc.ProcessWords(txt);
                    var proc_txt = string.Join(" ", words);
                    lang_detector.Push(proc_txt);
                }
            }
            Log($"Done ({src_id_pref})");
        }

        FastTextLauncher<LangDetectItem> CreateLangDetector()
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
        IEnumerable<string> GetSrcItems()
        {
            var conn_str = ConfRoot.GetSection("DataCyrConnStr").Value;
            using (var cn = new SQLiteConnection(conn_str))
            {
                cn.Open();
                var sql = ConfRoot.GetSection("DataCyrSelectSQL").Value;
                var cmd = new SQLiteCommand(sql, cn);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        yield return rd.GetString(0);
                }
            }
        }
    }
}
