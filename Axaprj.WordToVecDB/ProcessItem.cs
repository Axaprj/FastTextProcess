using Axaprj.WordToVecDB.Enums;

namespace Axaprj.WordToVecDB.Entities
{
    public class ProcessItem : ITextSource
    {
        internal const string TblnSrc = "Src";
        internal const string FldnSrcProcInfo = "ProcInfo";
        internal const string FldnSrcOriginalId = "OriginalId";
        internal const string FldnSrcDbgInfo = "DbgInfo";
        internal const string FldnSrcDictInxsStr = "DictInxsStr";

        internal const string TblnDict = "Dict";
        internal const string FldnDictInx = "Inx";
        internal const string FldnDictVectStr = "VectStr";

        public string Src;
        string ITextSource.GetText() => Src;
        void ITextSource.SetText(string txt) { Src = txt; }

        public string SrcOriginalId;
        public LangLabel Lang;
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
