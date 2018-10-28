using FastTextProcess.Context;
using FastTextProcess.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FastTextProcess.Tests
{
    public class WordToVectDbRoutines
    {
        ITestOutputHelper _output;
        public WordToVectDbRoutines(ITestOutputHelper output)
        {
            _output = output;
        }

        protected void WriteConsole(string msg)
        {
            _output.WriteLine(DateTime.Now.ToShortTimeString() + ":" + msg);
        }

        [Fact]
        public void procCreateDbEn()
        {
            var fvec = "./../../../../../../data.arc/cc.en.300.vec";
            AssertFileExists(fvec, "FastText file of vectors");

            var dbf = "w2v_en.db";
            WordToVectDb.CreateDB(dbf);
            WordToVectDb.ControlWordsIndex(dbf, is_enabled: false);
            using (var dbx = new WordToVectDb(dbf))
            {
                var trans = dbx.BeginTransaction();
                using (var sr = new StreamReader(fvec))
                {
                    // header
                    var line = sr.ReadLine();
                    var harr = line.Split(' ');
                    Assert.Equal(2, harr.Length);
                    WriteConsole($"'{fvec}': {harr[0]} - samples count, {harr[1]} - sample dim.");
                    // data
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;
                        var w2v = Word2Vect.Create(line);
                        dbx.Insert(w2v);
                    }
                }
                trans.Commit();
            }
            WriteConsole("ControlWordsIndex create...");
            WordToVectDb.ControlWordsIndex(dbf, is_enabled: true);
        }

        void AssertFileExists(string fpath, string descr = null)
        {
            fpath = Path.GetFullPath(fpath);
            Assert.True(File.Exists(fpath), $"'{fpath}' is not exist ({descr})");
        }
    }
}
