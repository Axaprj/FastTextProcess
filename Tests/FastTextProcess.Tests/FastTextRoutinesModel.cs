using Axaprj.FastTextProcess;
using Axaprj.WordToVecDB.Enums;
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
    /// FastText processing with model base 
    /// </summary>
    public abstract class FastTextRoutinesModel : FastTextRoutines
    {
        /// <summary> Word Vectors dictionary DB file path </summary>
        protected abstract string DBF_W2V { get; }
        /// <summary> Processing Word2Vectors result DB file path </summary>
        protected abstract string DBF_RESULT{ get; }
        /// <summary> Current Processing language label </summary>
        protected abstract LangLabel LANG { get; }
        /// <summary> Fasttext .VEC file name</summary>
        protected virtual string FTF_VECTOR => $"cc.{LANG.GetStdLangLabel()}.300.vec";
        /// <summary> Fasttext .BIN file name</summary>
        protected virtual string FTF_MODEL => $"cc.{LANG.GetStdLangLabel()}.300.bin";

        public FastTextRoutinesModel(ITestOutputHelper output) : base(output) { }

        [Fact]
        [Trait("Task", "Common")]
        [Trait("Process", "Load PreTrained FastText Database")]
        public void ProcCreateDbTest()
        {
            ProcCreateDb(DBF_W2V);
            ProcAppendDb(FTF_VECTOR, DBF_W2V, LANG);
        }

        [Fact]
        [Trait("Task", "Common")]
        [Trait("Process", "Clean Processing Results")]
        public void ProcResultCleanTest()
        {
            ProcResultClean(DBF_RESULT, DBF_W2V);
        }

        [Fact]
        [Trait("Task", "Common")]
        [Trait("SubProcess", "Fill Empty Add-in Dictionary Vectors (via FastTest)")]
        public void SubProcFillEmptyVectDictTest()
        {
            SubProcFillEmptyVectDict(FTF_MODEL, DBF_W2V, LANG);
        }

        [Fact]
        [Trait("Task", "Common")]
        [Trait("SubProcess", "Build Result Dictionary")]
        public void SubProcBuildResultDictTest()
        {
            SubProcBuildResultDict(DBF_RESULT, DBF_W2V);
        }

        [Fact]
        [Trait("Task", "Common")]
        [Trait("SubProcess", "Insert Predefined Vectors")]
        public void SubProcInsertPredefinedMacroTest()
        {
            SubProcInsertPredefinedMacro(DBF_W2V);
        }
    }
}
