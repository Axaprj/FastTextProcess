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

        /// <summary>Stored SrcDictInxsStr</summary>
        public string GetEmbeddedInxsStr() => string.Join(" ", Embedded);
        /// <summary>Preprocessed source text</summary>
        public string GetPreprocessedStr() => string.Join(" ", Preprocessed);
        /// <summary>Stored SrcDbgInfo</summary>
        public virtual string GetSrcDbgInfo() => GetPreprocessedStr();
        /// <summary>Stored SrcProcInfo</summary>
        public virtual string GetSrcProcInfo() => SrcProcInfo;

        public override string ToString() =>
             $"WordToVecDB::ProcessItem:'{SrcOriginalId}'; info:'{SrcProcInfo}';";
    }
}
