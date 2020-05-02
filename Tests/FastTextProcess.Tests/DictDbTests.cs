using Axaprj.WordToVecDB;
using Axaprj.WordToVecDB.Context;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;
using System;
using System.Data.SQLite;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FastTextProcess.Tests
{
    public class DictDbTests : TestBase
    {
        string DBF_W2V_RUK { get { return DataOutPath("w2v_ruk.db"); } }
        string DBF_RUK_Proc { get { return DataOutPath("RUK_proc.db"); } }

        public DictDbTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void TestCreateDictInsert()
        {
            var dbf = "w2v_test.db";
            FastTextProcessDB.CreateDB(dbf);
            var w2v = new Dict { Word = "test", Vect = new byte[] { 1, 2, 3 } };
            using (var dbx = new FastTextProcessDB(dbf))
            {

                var id1 = InsertDict(dbx, w2v, DictDbSet.DictKind.Main);
                w2v.Word = "Test";
                Assert.True(id1 > 0);
                w2v.Word = "Test";
                InsertDict(dbx, w2v, DictDbSet.DictKind.Main);
                Assert.True(w2v.Id > id1);
            }
            Log("done");
        }

        long InsertDict(FastTextProcessDB dbx, Dict w2v, DictDbSet.DictKind kind)
        {
            var w2v_tbl = dbx.Dict(DictDbSet.DictKind.Main);
            w2v_tbl.Insert(w2v);
            return w2v.Id;
        }

        [Fact]
        public void TestCreateEmbedInsert()
        {
            var dbf = "w2v_test.db";
            FastTextProcessDB.CreateDB(dbf);

            using (var dbx = new FastTextProcessDB(dbf))
            {
                var w2v = new Dict { Word = "test", Vect = new byte[] { 1, 2, 3 } };
                var id1 = InsertDict(dbx, w2v, DictDbSet.DictKind.Main);
                var ed = new EmbedDict { Inx = 0, DictId = id1, Freq = 1 };
                dbx.EmbedDict().Insert(ed);
            }
            using (var dbx = new FastTextProcessDB(dbf, foreign_keys: false))
            {
                var ed = new EmbedDict { Inx = 1, DictId = 999, Freq = 1 };
                dbx.EmbedDict().Insert(ed);
            }
            try
            {
                using (var dbx = new FastTextProcessDB(dbf))
                {
                    var ed = new EmbedDict { Inx = 1, DictId = 998, Freq = 1 };
                    dbx.EmbedDict().Insert(ed);
                }
                throw new InvalidOperationException("FK check failed");
            }
            catch (SQLiteException ex)
            {
                Log("FK checked:" + ex.Message);
            }
            Log("done");
        }
        [Fact]
        public void TestVectRangeRUK()
        {
            AssertFileExists(DBF_W2V_RUK, "Ru-Uk w2v DB");

            CalcMinMax(DBF_W2V_RUK, LangLabel.en);
            CalcMinMax(DBF_W2V_RUK, LangLabel.ru);
            CalcMinMax(DBF_W2V_RUK, LangLabel.uk);
            Log("done");
        }

        void CalcMinMax(string w2v_db_fn, LangLabel lang)
        {
            using (var dbx = new FastTextProcessDB(w2v_db_fn))
            {
                var dict_db = dbx.Dict(DictDbSet.DictKind.Main);
                var w2v_all = dict_db.GetAll(lang);
                var fmin = float.MaxValue;
                var fmax = float.MinValue;
                long cnt = 0;
                foreach (var w2v in w2v_all)
                {
                    foreach (var vv in Dict.GetVectFloat(w2v.Vect))
                    {
                        fmin = Math.Min(fmin, vv);
                        fmax = Math.Max(fmax, vv);
                    }
                    cnt++;
                }
                Log($"Lang={lang}: Count={cnt}; Min={fmin}; Max={fmax};");
            }
        }

        [Fact]
        public void TestCosineRUK()
        {
            AssertFileExists(DBF_W2V_RUK, "Ru-Uk w2v DB");

            var vs = VectorsService.Instance(DBF_W2V_RUK);
            var w1u = vs.FindByWord("шкарпетки", LangLabel.uk);
            var w1r = vs.FindByWord("носки", LangLabel.ru);
            var w1e = vs.FindByWord("socks", LangLabel.en);
            var w2u = vs.FindByWord("краватка", LangLabel.uk);
            var w2r = vs.FindByWord("галстук", LangLabel.ru);
            var w2e = vs.FindByWord("necktie", LangLabel.en);

            Log($"cos({w1u.Word}, {w1r.Word}) = {w1u.GetCosine(w1r)}");
            Log($"cos({w2u.Word}, {w2r.Word}) = {w2u.GetCosine(w2r)}");

            Log($"cos({w1u.Word}, {w1e.Word}) = {w1u.GetCosine(w1e)}");
            Log($"cos({w1r.Word}, {w1e.Word}) = {w1r.GetCosine(w1e)}");

            Log($"cos({w1u.Word}, {w2u.Word}) = {w1u.GetCosine(w2u)}");
            Log($"cos({w1r.Word}, {w2r.Word}) = {w1r.GetCosine(w2r)}");
            Log($"cos({w1e.Word}, {w2e.Word}) = {w1e.GetCosine(w2e)}");
            Log("done");
        }

        [Fact]
        public void TestCosineRUK2()
        {
            AssertFileExists(DBF_W2V_RUK, "Ru-Uk w2v DB");

            using (var dbx = new FastTextProcessDB(DBF_W2V_RUK))
            {
                var dict_db = dbx.Dict(DictDbSet.DictKind.Main);
                var sum_w2v_en = dict_db.FindByWord("sum", LangLabel.en);
                var sum_w2v_ru = dict_db.FindByWord("сумма", LangLabel.ru);
                PrintPair(sum_w2v_en, sum_w2v_ru);
                foreach (var w2v in dict_db.GetAll(LangLabel.ru))
                {
                    PrintPair(w2v, sum_w2v_en, distance_min: 0.3f);
                    PrintPair(w2v, sum_w2v_ru, distance_min: 0.6f);
                }
                foreach (var w2v in dict_db.GetAll(LangLabel.en))
                    PrintPair(w2v, sum_w2v_en, distance_min: 0.6f);
            }
            Log("done");
        }

        [Fact]
        public void TestV2W1consumption()
        {
            AssertFileExists(DBF_W2V_RUK, "En-Ru-Uk w2v DB");

            var vs = VectorsService.Instance(DBF_W2V_RUK);
            var w1 = vs.FindByWord("consumption", LangLabel.en);
            using (var dbx = new FastTextProcessDB(DBF_W2V_RUK))
            {
                var dict_db = dbx.Dict(DictDbSet.DictKind.Main);
                var w2v_en_all = dict_db.GetAll(LangLabel.en);
                Parallel.ForEach(w2v_en_all, (w2v) => 
                    PrintPair(w1, w2v, distance_min: 0.7f));
                //foreach (var w2v in w2v_en_all)
                //    PrintPair(w1, w2v, distance_min: 0.6f);
                var w2v_ru_all = dict_db.GetAll(LangLabel.ru);
                Parallel.ForEach(w2v_ru_all, (w2v) =>
                    PrintPair(w1, w2v, distance_min: 0.35f));
                //foreach (var w2v in w2v_ru_all)
                //    PrintPair(w1, w2v, distance_min: 0.35f);
            }
            Log("done");
        }

        void PrintPair(Dict w2v1, Dict w2v2, float distance_min = -1, bool log_err = true)
        {
            try
            {
                var cc = w2v1.GetCosine(w2v2);
                if (cc > distance_min)
                    Log($"cos({w2v1.Word},{w2v2.Word})={cc}");
            }
            catch (Exception ex)
            {
                if (log_err)
                    Log($"ERR ({w2v1.Word}, {w2v2.Word}): {ex.Message}");
            }
        }
    }
}
