using FastTextProcess.Entities;
using System;
using System.Data.SQLite;

namespace FastTextProcess.Context
{
    /// <summary>
    /// FastText dictionary DB: EmbedDict DbSet
    /// </summary>
    public class EmbedDictDbSet
    {
        readonly DbContext _ctx;
        SQLiteCommand _cmdInsert;

        public EmbedDictDbSet(DbContext ctx)
        {
            _ctx = ctx;
        }
        
        SQLiteCommand CmdInsert
        {
            get
            {
                if (_cmdInsert == null)
                {
                    var sql = $"INSERT INTO EmbedDict ({EmbedDict.FldnInx}, {EmbedDict.FldnDictId}, {EmbedDict.FldnDictAddinsId}, {EmbedDict.FldnFreq})"
                        +$" VALUES (${EmbedDict.FldnInx}, ${EmbedDict.FldnDictId}, ${EmbedDict.FldnDictAddinsId}, ${EmbedDict.FldnFreq})";
                    _cmdInsert = _ctx.CreateCmd(sql);
                    _cmdInsert.Parameters.Add(EmbedDict.FldnInx, System.Data.DbType.Int64);
                    _cmdInsert.Parameters.Add(EmbedDict.FldnDictId, System.Data.DbType.Int64);
                    _cmdInsert.Parameters.Add(EmbedDict.FldnDictAddinsId, System.Data.DbType.Int64);
                    _cmdInsert.Parameters.Add(EmbedDict.FldnFreq, System.Data.DbType.Int64);
                    _cmdInsert.Prepare();
                }
                return _cmdInsert;
            }
        }

        public int Insert(EmbedDict ed)
        {
            CmdInsert.Parameters[EmbedDict.FldnInx].Value = ed.Inx;
            CmdInsert.Parameters[EmbedDict.FldnDictId].Value = ed.DictId;
            CmdInsert.Parameters[EmbedDict.FldnDictAddinsId].Value = ed.DictAddinsId;
            CmdInsert.Parameters[EmbedDict.FldnFreq].Value = ed.Freq;
            var res = CmdInsert.ExecuteNonQuery();
            return res;
        }
    }
}
