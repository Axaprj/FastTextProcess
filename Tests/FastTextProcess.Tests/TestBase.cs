using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FastTextProcess.Tests
{
    public class TestBase
    {
        readonly ITestOutputHelper _output;

        public TestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            return testCases.ToList();  // Run them in discovery order
        }

        protected void WriteConsole(string msg)
        {
            msg = DateTime.Now.ToShortTimeString() + ": " + msg;
            if (_output != null)
                _output.WriteLine(msg);
        }
    }
}







