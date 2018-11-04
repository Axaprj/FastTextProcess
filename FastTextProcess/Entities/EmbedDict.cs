using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Entities
{
    public class EmbedDict
    {
        internal const string FldnInx = "Inx";
        internal const string FldnDictId = "DictId";
        internal const string FldnDictAddinsId = "DictAddinsId";
        internal const string FldnFreq = "Freq";

        public long Inx { get; set; }
        public long? DictId { get; set; }
        public long? DictAddinsId { get; set; }
        public long Freq { get; set; }
    }
}
