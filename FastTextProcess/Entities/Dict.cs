using FastTextProcess.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Entities
{
    /// <summary>
    /// word2vect.Dict entity
    /// </summary>
    public class Dict
    {
        internal const string FldnId = "Id";
        internal const string FldnWord = "Word";
        internal const string FldnVect = "Vect";
        internal const string FldnLangId = "LangId";

        public long Id { get; set; }
        public string Word { get; set; }
        public byte[] Vect { get; set; }
        public FTLangLabel Lang { get; set; }

        public static Dict Create(string str, FTLangLabel lang)
        {
            var sarr = str.Trim().Split(' ');
            var sfarr = new string[sarr.Length - 1];
            Array.Copy(sarr, 1, sfarr, 0, sfarr.Length);
            var farr = Array.ConvertAll(sfarr, float.Parse);
            var barr = new byte[farr.Length * 4];
            Buffer.BlockCopy(farr, 0, barr, 0, barr.Length);
            return new Dict { Word = sarr[0], Vect = barr, Lang = lang };
        }

        public static Dict CreateEmpty(string word = "<%NONE%>"
            , FTLangLabel lang = FTLangLabel.NotSpecified
            , int vect_sz = 300)
        {
            var res = new Dict
            {
                Id = -1,
                Word = word,
                Vect = new byte[vect_sz * 4],
                Lang = lang
            };
            Array.Clear(res.Vect, 0, vect_sz * 4);
            return res;
        }

        public static float[] GetVectFloat(byte[] vect)
        {
            var lenb = vect.Length;
            if (vect.Length % 4 != 0)
                throw new InvalidOperationException(
                    $"Wrong vector byte array length {lenb}");
            var res = new float[(int)lenb / 4];
            for (int inx = 0; inx < res.Length; inx++)
                res[inx] = BitConverter.ToSingle(vect, inx * 4);
            return res;
        }
    }
}
