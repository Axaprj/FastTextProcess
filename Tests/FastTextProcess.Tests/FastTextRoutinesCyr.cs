using FastTextProcess.Context;
using FastTextProcess.Entities;
using System;
using System.Collections.Generic;
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
            ProcResultClean(DBF_W2V_RU, DBF_RUK_Proc);
            ProcResultClean(DBF_W2V_UK, DBF_RUK_Proc);
        }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Load PreTrained FastText Database")]
        public void ProcCreateDbRuk()
        {
            ProcCreateDb("wiki.ru.vec", DBF_W2V_RU);
            ProcCreateDb("wiki.uk.vec", DBF_W2V_UK);
        }
    }
}
