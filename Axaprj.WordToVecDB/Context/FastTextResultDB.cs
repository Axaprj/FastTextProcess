using Axaprj.WordToVecDB.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using Axaprj.WordToVecDB.Entities;

namespace Axaprj.WordToVecDB.Context
{
    /// <summary>
    /// FastText processing result DB context
    /// TAGS: not_thread_safe
    /// </summary>
    public class FastTextResultDB : DbContext
    {
        public FastTextResultDB(string db_file, bool foreign_keys = true)
            : base(db_file, foreign_keys) { }

        public static void CreateIfNotExistsDB(string db_file) => 
            CreateDB(db_file, Resources.vect_result_create, if_not_exists:true);

        #region SQL Commands
        SQLiteCommand _cmdSrcInsert;
        SQLiteCommand CmdSrcInsert
        {
            get
            {
                if (_cmdSrcInsert == null)
                {
                    var sql = string.Format(
                        "INSERT OR REPLACE INTO {0} ({1}, {2}, {3}, {4}) VALUES (${1}, ${2}, ${3}, ${4})",
                        ProcessItem.TblnSrc
                        , ProcessItem.FldnSrcOriginalId
                        , ProcessItem.FldnSrcProcInfo
                        , ProcessItem.FldnSrcDictInxsStr
                        , ProcessItem.FldnSrcDbgInfo
                        );
                    _cmdSrcInsert = CreateCmd(sql);
                    _cmdSrcInsert.Parameters.Add(ProcessItem.FldnSrcOriginalId, DbType.String);
                    _cmdSrcInsert.Parameters.Add(ProcessItem.FldnSrcProcInfo, DbType.String);
                    _cmdSrcInsert.Parameters.Add(ProcessItem.FldnSrcDictInxsStr, DbType.String);
                    _cmdSrcInsert.Parameters.Add(ProcessItem.FldnSrcDbgInfo, DbType.String);
                    _cmdSrcInsert.Prepare();
                }
                return _cmdSrcInsert;
            }
        }
        SQLiteCommand _cmdDictInsert;
        SQLiteCommand CmdDictInsert
        {
            get
            {
                if (_cmdDictInsert == null)
                {
                    var sql = string.Format(
                        "INSERT INTO {0} ({1}, {2}) VALUES (${1}, ${2})",
                        ProcessItem.TblnDict, ProcessItem.FldnDictInx, ProcessItem.FldnDictVectStr
                        );
                    _cmdDictInsert = CreateCmd(sql);
                    _cmdDictInsert.Parameters.Add(ProcessItem.FldnDictInx, DbType.Int64);
                    _cmdDictInsert.Parameters.Add(ProcessItem.FldnDictVectStr, DbType.String);
                    _cmdDictInsert.Prepare();
                }
                return _cmdDictInsert;
            }
        }
        #endregion

        public long StoreProcessItem(ProcessItem itm)
        {
            CmdSrcInsert.Parameters[ProcessItem.FldnSrcOriginalId].Value = itm.SrcOriginalId;
            CmdSrcInsert.Parameters[ProcessItem.FldnSrcProcInfo].Value = itm.SrcProcInfo;
            CmdSrcInsert.Parameters[ProcessItem.FldnSrcDictInxsStr].Value = itm.GetEmbeddedInxsStr();
            CmdSrcInsert.Parameters[ProcessItem.FldnSrcDbgInfo].Value = itm.GetPreprocessedStr();
            if (CmdSrcInsert.ExecuteNonQuery() != 1)
                throw new InvalidOperationException($"StoreProcessItem failed: {itm}");
            return LastInsertRowId;
        }

        public void StoreDictItem(EmbedJoin itm)
        {
            CmdDictInsert.Parameters[ProcessItem.FldnDictInx].Value = itm.Inx;
            CmdDictInsert.Parameters[ProcessItem.FldnDictVectStr].Value = itm.GetVectStr();
            if (CmdDictInsert.ExecuteNonQuery() != 1)
                throw new InvalidOperationException($"StoreDictItem failed: {itm}");
        }

        public long? GetDictInxMax()
        {
            var sql = string.Format("SELECT MAX({1}) FROM {0}",
                ProcessItem.TblnDict, ProcessItem.FldnDictInx);
            var cmd = CreateCmd(sql);
            var res = cmd.ExecuteScalar();
            return res == null || DBNull.Value.Equals(res) ? (long?)null : Convert.ToInt64(res);
        }

    }
}
