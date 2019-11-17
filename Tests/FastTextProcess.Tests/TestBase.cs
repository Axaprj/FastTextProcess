using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FastTextProcess.Tests
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

        protected IConfigurationRoot ConfRoot
        {
            get
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("FastTextProcess.Tests.json", optional: false, reloadOnChange: false)
                    .Build();
                return config;
            }
        }

        protected string DataArcPath(string file_name)
        {
            var dir = ConfRoot.GetSection("DataArcDir").Value;
            return Path.GetFullPath(Path.Combine(dir, file_name));
        }

        protected string DataOutPath(string file_name)
        {
            var dir = ConfRoot.GetSection("DataOutDir").Value;
            return Path.GetFullPath(Path.Combine(dir, file_name));
        }

        protected string FastTextBin
        {
            get
            {
                var bin = ConfRoot.GetSection("FastTextBin").Value;
                return Path.GetFullPath(bin);
            }
        }
    }
}







