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

        SQLiteCommand _cmdInsert;
        SQLiteCommand CmdInsert
        {
            get
            {
                if (_cmdInsert == null)
                {
                    var sql = string.Format(
                        "INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES (${1}, ${2}, ${3}, ${4})",
                         EmbedDict.Tbln, 
                         EmbedDict.FldnInx, EmbedDict.FldnDictId, EmbedDict.FldnDictAddinsId, EmbedDict.FldnFreq);
                    _cmdInsert = Ctx.CreateCmd(sql);
                    _cmdInsert.Parameters.Add(EmbedDict.FldnInx,  DbType.Int64);
                    _cmdInsert.Parameters.Add(EmbedDict.FldnDictId,  DbType.Int64);
                    _cmdInsert.Parameters.Add(EmbedDict.FldnDictAddinsId , DbType.Int64);
                    _cmdInsert.Parameters.Add(EmbedDict.FldnFreq, DbType.Int64);
                    _cmdInsert.Prepare();
                }
                return _cmdInsert;
            }
        }

        SQLiteCommand _cmdIncrementFreq;
        SQLiteCommand CmdIncrementFreq
        {
            get
            {
                if (_cmdIncrementFreq == null)
                {
                    var sql = string.Format(
                        "UPDATE {0} SET {2} = {2} + ${2} WHERE {1} = ${1}",
                         EmbedDict.Tbln, EmbedDict.FldnInx, EmbedDict.FldnFreq);
                    _cmdIncrementFreq = Ctx.CreateCmd(sql);
                    _cmdIncrementFreq.Parameters.Add(EmbedDict.FldnInx, DbType.Int64);
                    _cmdIncrementFreq.Parameters.Add(EmbedDict.FldnFreq, DbType.Int64);
                    _cmdIncrementFreq.Prepare();
                }
                return _cmdIncrementFreq;
            }
        }
        #endregion

        public int Insert(EmbedDict ed)
        {
            CmdInsert.Parameters[EmbedDict.FldnInx].Value = ed.Inx;
            CmdInsert.Parameters[EmbedDict.FldnDictId].Value = ed.DictId;
            CmdInsert.Parameters[EmbedDict.FldnDictAddinsId].Value = ed.DictAddinsId;
            CmdInsert.Parameters[EmbedDict.FldnFreq].Value = ed.Freq;
            return CmdInsert.ExecuteNonQuery();
        }

        public int IncrementFreq(long inx, long add_freq)
        {
            CmdInsert.Parameters[EmbedDict.FldnInx].Value = inx;
            CmdInsert.Parameters[EmbedDict.FldnFreq].Value = add_freq;
            return CmdInsert.ExecuteNonQuery();
        }

        public long? SelectInxMax()
        {
            var sql = string.Format("SELECT MAX({1}) FROM {0}",
                        EmbedDict.Tbln, EmbedDict.FldnInx);
            var cmd = Ctx.CreateCmd(sql);
            var res = cmd.ExecuteScalar();
            return res == null || DBNull.Value.Equals(res) ? (long?)null : Convert.ToInt64(res);
        }

        public long? FindInxById(long id, DictDbSet.DictKind dict_kind)
        {
            CmdFindInxById.Parameters[EmbedDict.FldnDictId].Value =
                dict_kind == DictDbSet.DictKind.Main ? id : -1;
            CmdFindInxById.Parameters[EmbedDict.FldnDictAddinsId].Value =
                dict_kind == DictDbSet.DictKind.Addin ? id : -1;
            var res = CmdFindInxById.ExecuteScalar();
            return res == null || DBNull.Value.Equals(res) ? (long?)null : Convert.ToInt64(res);
        }

    }
}
