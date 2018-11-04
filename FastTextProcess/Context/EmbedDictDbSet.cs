using FastTextProcess.Entities;
using System;
using System.Data.SQLite;

namespace FastTextProcess.Context
{
    /// <summary>
    /// FastText dictionary DB: EmbedDict DbSet
    /// </summary>
    public class EmbedDictDbSet : DbSet
    {
        public EmbedDictDbSet(DbContext ctx) : base(ctx) { }

        public int Insert(EmbedDict ed)
        {
            var sql = string.Format(
                "INSERT INTO EmbedDict ({0}, {1}, {2}, {3}) VALUES (${0}, ${1}, ${2}, ${3})",
                 EmbedDict.FldnInx, EmbedDict.FldnDictId, EmbedDict.FldnDictAddinsId, EmbedDict.FldnFreq);
            var cmd = Ctx.CreateCmd(sql);
            cmd.Parameters.AddWithValue(EmbedDict.FldnInx, ed.Inx);
            cmd.Parameters.AddWithValue(EmbedDict.FldnDictId, ed.DictId);
            cmd.Parameters.AddWithValue(EmbedDict.FldnDictAddinsId, ed.DictAddinsId);
            cmd.Parameters.AddWithValue(EmbedDict.FldnFreq, ed.Freq);
            var res = cmd.ExecuteNonQuery();
            return res;
        }
    }
}
