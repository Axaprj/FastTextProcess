using FastTextProcess.Context;
using FastTextProcess.Entities;
using System;
using Xunit;
using Xunit.Abstractions;

namespace FastTextProcess.Tests
{
    public class WordToVectDbTests : TestBase
    {
        public WordToVectDbTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void TestCreateInsert()
        {
            var dbf = "w2v_test.db";
            FastTextProcessDB.CreateDB(dbf);
            var w2v = new Word2Vect { Word = "test", Vect = new byte[] { 1, 2, 3 } };
            using (var dbx = new FastTextProcessDB(dbf))
            {
                var w2v_tbl = dbx.WordToVect(WordToVectDbSet.DictDb.Main);
                w2v_tbl.Insert(w2v);
                var id1 = w2v.Id;
                Assert.True(id1 > 0);
                w2v.Word = "Test";
                w2v_tbl.Insert(w2v);
                Assert.True(w2v.Id > id1);
            }
            WriteConsole("done");
        }
    }
}
