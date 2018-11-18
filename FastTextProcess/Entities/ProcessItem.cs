using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Entities
{
    internal class ProcessItem
    {
        internal const string TblnSrc = "Src";
        internal const string FldnSrcProcInfo = "ProcInfo";
        internal const string FldnSrcOriginalId = "OriginalId";
        internal const string FldnSrcDbgInfo = "DbgInfo";
        internal const string FldnSrcDictInxsStr = "DictInxsStr";

        public string Src;
        public string SrcOriginalId;
        public string SrcProcInfo;
        public string[] Preprocessed;
        public long[] Embedded;

        public string GetEmbeddedInxsStr() =>
            string.Join(" ", Embedded);


        public string GetPreprocessedStr() =>
            string.Join(" ", Preprocessed);

        public override string ToString() =>
             $"FastTextProcess::ProcessItem:'{SrcOriginalId}'; info:'{SrcProcInfo}';";
    }
}
