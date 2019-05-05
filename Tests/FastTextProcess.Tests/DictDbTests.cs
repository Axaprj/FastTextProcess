using FastTextProcess.Context;
using FastTextProcess.Entities;
using System;
using System.Data;
using System.Data.SQLite;
using Xunit;
using Xunit.Abstractions;

namespace FastTextProcess.Tests
{
    public class DictDbTests : TestBase
    {
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
        public void TestCosineRUK()
        {
            var DBF_W2V_RUK = "w2v_ruk.db";
            AssertFileExists(DBF_W2V_RUK, "Ru-Uk w2v DB");
            
            using (var dbx = new FastTextProcessDB(DBF_W2V_RUK))
            {
                var dict_db = dbx.Dict(DictDbSet.DictKind.Main);
                var w1u = dict_db.FindByWord("шкарпетки", Enums.FTLangLabel.__label__uk);
                var w1r = dict_db.FindByWord("носки", Enums.FTLangLabel.__label__ru);
                var w2u = dict_db.FindByWord("краватка", Enums.FTLangLabel.__label__uk);
                var w2r = dict_db.FindByWord("галстук", Enums.FTLangLabel.__label__ru);
                Log($"cos({w1u.Word}, {w1r.Word}) = {w1u.GetCosine(w1r)}");
                Log($"cos({w2u.Word}, {w2r.Word}) = {w2u.GetCosine(w2r)}");
                Log($"cos({w1u.Word}, {w2u.Word}) = {w1u.GetCosine(w2u)}");
                Log($"cos({w1r.Word}, {w2r.Word}) = {w1r.GetCosine(w2r)}");
            }
            Log("done");
        }
    }
}
