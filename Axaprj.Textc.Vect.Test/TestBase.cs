using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Axaprj.Textc.Vect.Test
{
    public class TestBase
    {
        readonly ITestOutputHelper _output;

        public TestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        protected void Log(string msg)
        {
            msg = DateTime.Now.ToShortTimeString() + ": " + msg;
            _output.WriteLine(msg);
        }

        protected void AssertFileExists(string fpath, string descr = null)
        {
            fpath = Path.GetFullPath(fpath);
            Assert.True(File.Exists(fpath), $"'{fpath}' is not exist ({descr})");
        }

        protected void AssertFileNotExists(string fpath, string descr = null)
        {
            fpath = Path.GetFullPath(fpath);
            Assert.False(File.Exists(fpath), $"'{fpath}' is exist ({descr})");
        }
    }
}







