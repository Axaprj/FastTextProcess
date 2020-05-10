using Axaprj.Textc.Vect.Attributes;
using Axaprj.WordToVecDB;
using Axaprj.WordToVecDB.Entities;
using Axaprj.WordToVecDB.Enums;

namespace Axaprj.Textc.Vect
{
    public class VRequestContext : Axaprj.Textc.Vect.RequestContext, IVRequestContext
    {
        //const string VNAME_TextSlice = "VRequestContext.TextSlice";
        //const string VNAME_TextProcess = "VRequestContext.TextProcess";
        //const string VNAME_ReplaceAttrib = "VRequestContext.ReplaceAttrib";
        //const string VNAME_ReplaceEnum = "VRequestContext.ReplaceEnum";

        public string W2VDictFile { get; set; }

        public LangLabel LangLabel { get; set; }

        public float MinCosine { get; set; }

        public Dict FindVectByWord(string word)
        {
            var serv = VectorsService.Instance(W2VDictFile);
            return serv.FindByWord(word, LangLabel);
        }

        //public string TextSlice
        //{
        //    get => (string)GetVariable(VNAME_TextSlice);
        //    set => SetVariable(VNAME_TextSlice, value);
        //}

        //public string TextProcess
        //{
        //    get => (string)GetVariable(VNAME_TextProcess);
        //    set => SetVariable(VNAME_TextProcess, value);
        //}

        //public ReplaceTextCAttribute ReplaceAttrib
        //{
        //    get => (ReplaceTextCAttribute)GetVariable(VNAME_ReplaceAttrib);
        //    set => SetVariable(VNAME_ReplaceAttrib, value);
        //}

        //public object ReplaceEnum
        //{
        //    get => GetVariable(VNAME_ReplaceEnum);
        //    set => SetVariable(VNAME_ReplaceEnum, value);
        //}

        //public void ReplaceByEnum(string macro_value, string remaining_text, out int prev_txt_len)
        //{
        //    try
        //    {
        //        prev_txt_len = TextProcess.Length - TextSlice.Length - 1;
        //        var prev_txt = prev_txt_len > 0
        //                ? TextProcess.Substring(0, prev_txt_len) : TextProcess;
        //        TextProcess = StringUtil.InsertReplaceMacro(prev_txt, macro_value, remaining_text);
        //    }
        //    catch
        //    { // debug
        //        throw;
        //    }
        //}
    }
}
