using FastTextProcess.Preprocessor;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FastTextProcess.Tests
{
    public class FastTextLauncherTests : TestBase
    {
        public FastTextLauncherTests(ITestOutputHelper output) : base(output) { }
        [Fact]
        public void TestCreateLangDetector()
        {
            using (var lang_detector = CreateLangDetector())
            {
                var preproc = new CommonEnCyr(lang_detector);
                preproc.RunAsync((txt_src, pp_item)
                    => Log($"{pp_item.Lang}>>> {pp_item.Text}"));
                Log($"Process samples ...");
                foreach (string txt in GetSrcItems())
                {
                    preproc.Push(txt);
                }
            }
            Log($"Done");
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
