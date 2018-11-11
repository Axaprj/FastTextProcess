using FastTextProcess.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace FastTextProcess.Context
{
    /// <summary>
    /// FastText dictionary DB: EmbedDict DbSet
    /// </summary>
    public class EmbedDictDbSet : DbSet
    {
        public EmbedDictDbSet(DbContext ctx) : base(ctx) { }

        #region SQL Commands
        SQLiteCommand _cmdFindInxById;
        SQLiteCommand CmdFindInxById
        {
            get
            {
                if (_cmdFindInxById == null)
                {
                    var sql = string.Format(
                        "SELECT {1} FROM {0} WHERE {2} = ${2} OR {3} = ${3}",
                        EmbedDict.Tbln, EmbedDict.FldnInx, EmbedDict.FldnDictId, EmbedDict.FldnDictAddinsId);
                    _cmdFindInxById = Ctx.CreateCmd(sql);
                    _cmdFindInxById.Parameters.Add(EmbedDict.FldnDictId, DbType.Int64);
                    _cmdFindInxById.Parameters.Add(EmbedDict.FldnDictAddinsId, DbType.Int64);
                    _cmdFindInxById.Prepare();
                }
                return _cmdFindInxById;
            }
        }

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

        public long? SelectInxMax()
        {
            var sql = string.Format("SELECT MAX({1}) FROM {0}",
                        EmbedDict.Tbln, EmbedDict.FldnInx);
            var cmd = Ctx.CreateCmd(sql);
            var res = cmd.ExecuteScalar();
            return DBNull.Value.Equals(res) ? (long?)null : Convert.ToInt64(res);
        }
        #endregion

        public long? FindInxById(long id, DictDbSet.DictKind dict_kind)
        {
            CmdFindInxById.Parameters[EmbedDict.FldnDictId].Value =
                dict_kind == DictDbSet.DictKind.Main ? id : -1;
            CmdFindInxById.Parameters[EmbedDict.FldnDictAddinsId].Value =
                dict_kind == DictDbSet.DictKind.Addin ? id : -1;
            var res = CmdFindInxById.ExecuteScalar();
            return res == null ? (long?)null : Convert.ToInt64(res);
        }

    }
}
