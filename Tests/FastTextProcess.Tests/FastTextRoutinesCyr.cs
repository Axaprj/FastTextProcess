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
    public class FastTextRoutinesCyr : FastTextRoutines
    {
        const string DBF_W2V_RU = "w2v_ru.db";
        const string DBF_W2V_UK = "w2v_uk.db";
        const string DBF_RUK_Proc = "RUK_proc.db";
        public FastTextRoutinesCyr(ITestOutputHelper output) : base(output) { }

        [Fact]
        [Trait("Task", "RUK")]
        [Trait("Process", "Clean Processing Results")]
        public void ProcAclImdbResultClean()
        {
            ProcResultClean(DBF_W2V_RU, DBF_RUK_Proc);
            ProcResultClean(DBF_W2V_UK, DBF_RUK_Proc);
        }

        void ProcResultClean(string res_db_file, string proc_file)
        {
            if (File.Exists(res_db_file))
            {
                File.Delete(res_db_file);
                Log($"'{res_db_file}' deleted");
            }
            AssertFileExists(proc_file, "word2vect DB");
            using (var dbx = new FastTextProcessDB(proc_file))
            {
                dbx.EmbedDict().DeleteAll();
                dbx.Dict(DictDbSet.DictKind.Addin).DeleteAll();
            }
        }
    
    }
}
