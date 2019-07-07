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
        const int DEF_VECT_SIZE = 300;
        internal const string FldnId = "Id";
        internal const string FldnWord = "Word";
        internal const string FldnVect = "Vect";
        internal const string FldnLangId = "LangId";

        public long Id { get; set; }
        public string Word { get; set; }
        public byte[] Vect { get; set; }
        public FTLangLabel Lang { get; set; }

        public static Dict CreateParseFT(string str, FTLangLabel lang)
        {
            var sarr = str.Trim().Split(' ');
            var sfarr = new string[sarr.Length - 1];
            Array.Copy(sarr, 1, sfarr, 0, sfarr.Length);
            var farr = Array.ConvertAll(sfarr, float.Parse);
            var barr = new byte[farr.Length * 4];
            Buffer.BlockCopy(farr, 0, barr, 0, barr.Length);
            return new Dict { Word = sarr[0], Vect = barr, Lang = lang };
        }

        public static Dict CreateRnd(string word, FTLangLabel lang, int vect_sz = DEF_VECT_SIZE)
        {
            var barr = new byte[vect_sz * 4];
            return new Dict { Id = -1, Word = word, Vect = barr, Lang = lang };
        }

        public static Dict CreateEmpty(string word = "<%NONE%>"
            , FTLangLabel lang = FTLangLabel.NotSpecified
            , int vect_sz = DEF_VECT_SIZE)
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

        public static float GetCosine(float[] a, float[] b)
        {
            if (a == null || b == null)
                throw new InvalidOperationException(
                    "Not initialized vectors");
            var len = a.Length;
            if (len != b.Length)
                throw new InvalidOperationException(
                    $"Wrong vector arrays: length(a)={len} length(b)={b.Length}");
            float prod = 0;
            float sq_a = 0;
            float sq_b = 0;
            for (int inx = 0; inx < len; inx++)
            {
                prod += a[inx] * b[inx];
                sq_a += a[inx] * a[inx];
                sq_b += b[inx] * b[inx];
            }
            return (float)(prod / (Math.Sqrt(sq_a)* Math.Sqrt(sq_b)));
        }

        public float GetCosine(Dict other)
        {
            float[] a = GetVectFloat(this.Vect);
            float[] b = GetVectFloat(other.Vect);
            return GetCosine(a, b);
        }
    }
}
