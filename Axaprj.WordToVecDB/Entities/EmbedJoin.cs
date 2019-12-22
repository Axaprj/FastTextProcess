using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.WordToVecDB.Entities
{
    public class EmbedJoin
    {
        internal const string TblName = "EmbedJoin";
        internal const string FldnInx = "Inx";
        internal const string FldnVect = "Vect";
        internal const string FldnFreq = "Freq";

        public long Inx { get; set; }
        public byte[] Vect { get; set; }
        public long Freq { get; set; }

        public string GetVectStr()
        {
            var vect_f = Dict.GetVectFloat(Vect);
            return string.Join(" ", vect_f);
        }
    }

}
