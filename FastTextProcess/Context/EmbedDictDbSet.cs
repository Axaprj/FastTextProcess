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
        internal EmbedDictDbSet(DbContext ctx) : base(ctx, "EmbedDict") { }

        #region SQL Commands
        SQLiteCommand _cmdFindInxByDictId;
        SQLiteCommand CmdFindInxByDictId
        {
            get
            {
                if (_cmdFindInxByDictId == null)
                {
                    var sql = string.Format(
                        "SELECT {1} FROM {0} WHERE {2} = ${2}",
                        TableName, EmbedDict.FldnInx, EmbedDict.FldnDictId);
                    _cmdFindInxByDictId = Ctx.CreateCmd(sql);
                    _cmdFindInxByDictId.Parameters.Add(EmbedDict.FldnDictId, DbType.Int64);
                    _cmdFindInxByDictId.Prepare();
                }
                return _cmdFindInxByDictId;
            }
        }

        SQLiteCommand _cmdFindInxByDictAddinsId;
        SQLiteCommand CmdFindInxByDictAddinsId
        {
            get
            {
                if (_cmdFindInxByDictAddinsId == null)
                {
                    var sql = string.Format(
                        "SELECT {1} FROM {0} WHERE {2} = ${2}",
                        TableName, EmbedDict.FldnInx, EmbedDict.FldnDictAddinsId);
                    _cmdFindInxByDictAddinsId = Ctx.CreateCmd(sql);
                    _cmdFindInxByDictAddinsId.Parameters.Add(EmbedDict.FldnDictAddinsId, DbType.Int64);
                    _cmdFindInxByDictAddinsId.Prepare();
                }
                return _cmdFindInxByDictAddinsId;
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
                         TableName, 
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
                         TableName, EmbedDict.FldnInx, EmbedDict.FldnFreq);
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
            CmdIncrementFreq.Parameters[EmbedDict.FldnInx].Value = inx;
            CmdIncrementFreq.Parameters[EmbedDict.FldnFreq].Value = add_freq;
            return CmdIncrementFreq.ExecuteNonQuery();
        }

        public long? SelectInxMax()
        {
            var sql = string.Format("SELECT MAX({1}) FROM {0}",
                        TableName, EmbedDict.FldnInx);
            var cmd = Ctx.CreateCmd(sql);
            var res = cmd.ExecuteScalar();
            return res == null || DBNull.Value.Equals(res) ? (long?)null : Convert.ToInt64(res);
        }

        public long? FindInxById(long id, DictDbSet.DictKind dict_kind)
        {
            object res = null;
            if (dict_kind == DictDbSet.DictKind.Main)
            {
                CmdFindInxByDictId.Parameters[EmbedDict.FldnDictId].Value = id;
                res = CmdFindInxByDictId.ExecuteScalar();
            }
            else if (dict_kind == DictDbSet.DictKind.Addin)
            {
                CmdFindInxByDictAddinsId.Parameters[EmbedDict.FldnDictAddinsId].Value = id;
                res = CmdFindInxByDictAddinsId.ExecuteScalar();
            }
            else throw new NotSupportedException($"{dict_kind} not supported");
            return res == null || DBNull.Value.Equals(res) ? (long?)null : Convert.ToInt64(res);
        }

    }
}
