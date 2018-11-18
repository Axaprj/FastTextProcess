using FastTextProcess.Entities;
using FastTextProcess.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace FastTextProcess.Context
{
    /// <summary>
    /// FastText processing result DB context
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
        #endregion

        internal long StoreProcessItem(ProcessItem itm)
        {
            CmdSrcInsert.Parameters[ProcessItem.FldnSrcOriginalId].Value = itm.SrcOriginalId;
            CmdSrcInsert.Parameters[ProcessItem.FldnSrcProcInfo].Value = itm.SrcProcInfo;
            CmdSrcInsert.Parameters[ProcessItem.FldnSrcDictInxsStr].Value = itm.GetEmbeddedInxsStr();
            CmdSrcInsert.Parameters[ProcessItem.FldnSrcDbgInfo].Value = itm.GetPreprocessedStr();
            if (CmdSrcInsert.ExecuteNonQuery() != 1)
                throw new InvalidOperationException($"StoreProcessItem failed: {itm}");
            return LastInsertRowId;
        }
    }
}
