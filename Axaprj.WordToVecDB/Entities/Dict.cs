using Axaprj.WordToVecDB.Enums;
using System;
using System.Collections.Generic;

namespace Axaprj.WordToVecDB.Entities
{
    /// <summary>
    /// word2vect.Dict entity
    /// </summary>
    public class Dict
    {
        const int DEF_VECT_SIZE = 300;
        const float DEF_VECT_MIN = -0.4F;
        const float DEF_VECT_MAX = 0.4F;
        internal const string FldnId = "Id";
        internal const string FldnWord = "Word";
        internal const string FldnVect = "Vect";
        internal const string FldnLangId = "LangId";

        public long Id { get; set; }
        public string Word { get; set; }
        public byte[] Vect { get; set; }
        public LangLabel Lang { get; set; }

        public static Dict CreateParseFT(string str, LangLabel lang)
        {
            var sarr = str.Trim().Split(' ');
            var sfarr = new string[sarr.Length - 1];
            Array.Copy(sarr, 1, sfarr, 0, sfarr.Length);
            var farr = Array.ConvertAll(sfarr, float.Parse);
            //var barr = new byte[farr.Length * 4];
            //Buffer.BlockCopy(farr, 0, barr, 0, barr.Length);
            var barr = Float2Byte(farr);
            return new Dict { Word = sarr[0], Vect = barr, Lang = lang };
        }

        public static Dict CreateRnd(Random rnd, string word, LangLabel lang
            , int vect_sz = DEF_VECT_SIZE, float vv_min = DEF_VECT_MIN, float vv_max = DEF_VECT_MAX)
        {
            var farr = new float[vect_sz];
            var delta = vv_max - vv_min;
            for (int inx = 0; inx < vect_sz; inx++)
            {
                farr[inx] = (float)(vv_max - delta * rnd.NextDouble());
            }
            var barr = Float2Byte(farr);
            return new Dict { Id = -1, Word = word, Vect = barr, Lang = lang };
        }

        static byte[] Float2Byte(float[] farr)
        {
            var barr = new byte[farr.Length * 4];
            Buffer.BlockCopy(farr, 0, barr, 0, barr.Length);
            return barr;
        }

        public static Dict CreateEmpty(string word = "<%NONE%>"
            , LangLabel lang = LangLabel.NA
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
            return (float)(prod / (Math.Sqrt(sq_a) * Math.Sqrt(sq_b)));
        }

        public float GetCosine(Dict other)
        {
            float[] a = GetVectFloat(this.Vect);
            float[] b = GetVectFloat(other.Vect);
            return GetCosine(a, b);
        }
        
        public Dict GetNearest(IEnumerable<Dict> dicts, float min_cosine = 0f)
        {
            float max_cosine = 0f;
            Dict nearest = null;
            foreach (var dict in dicts)
            {
                var cc = GetCosine(dict);
                if (cc > max_cosine)
                {
                    max_cosine = cc;
                    nearest = dict;
                }
            }
            return max_cosine > min_cosine ? nearest: null;
        }

        public override string ToString()
        {
            var msg = "Dict[";
            try
            {
                msg += $"'{Word ?? "NULL"}' {Lang} {Id}";
            }
            catch (Exception ex)
            {
                msg += $"ToStr Err: {ex}";
            }
            return msg + "]";
        }
    }
}
