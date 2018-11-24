using FastTextProcess.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace FastTextProcess.Context
{
    /// <summary>
    /// FastText dictionary DB: Dict/DictAddins DbSet
    /// TAGS: not_thread_safe
    /// </summary>
    public class DictDbSet : DbSet
    {
        /// <summary>
        /// Dictionary tables enumeration 
        /// </summary>
        public enum DictKind { Main, Addin }

        internal DictDbSet(DbContext ctx, DictKind db_kind)
            : base(ctx, db_kind == DictKind.Main ? "Dict" : "DictAddins")
        { }

        #region SQL Commands
        SQLiteCommand _cmdInsert;
        SQLiteCommand _cmdFindIdByWord;
        SQLiteCommand _cmdUpdateVectOfWord;

        SQLiteCommand CmdInsert
        {
            get
            {
                if (_cmdInsert == null)
                {
                    var sql = string.Format(
                        "INSERT INTO {0} ({1}, {2}) VALUES (${1}, ${2})",
                        TableName, Dict.FldnWord, Dict.FldnVect);
                    _cmdInsert = Ctx.CreateCmd(sql);
                    _cmdInsert.Parameters.Add(Dict.FldnWord, DbType.String);
                    _cmdInsert.Parameters.Add(Dict.FldnVect, DbType.Binary);
                    _cmdInsert.Prepare();
                }
                return _cmdInsert;
            }
        }

        SQLiteCommand CmdUpdateVectOfWord
        {
            get
            {
                if (_cmdUpdateVectOfWord == null)
                {
                    var sql = string.Format(
                        "UPDATE {0} SET {2}=${2} WHERE {1}=${1}",
                        TableName, Dict.FldnWord, Dict.FldnVect);
                    _cmdUpdateVectOfWord = Ctx.CreateCmd(sql);
                    _cmdUpdateVectOfWord.Parameters.Add(Dict.FldnWord, DbType.String);
                    _cmdUpdateVectOfWord.Parameters.Add(Dict.FldnVect, DbType.Binary);
                    _cmdUpdateVectOfWord.Prepare();
                }
                return _cmdUpdateVectOfWord;
            }
        }

        SQLiteCommand CmdFindIdByWord
        {
            get
            {
                if (_cmdFindIdByWord == null)
                {
                    var sql = string.Format(
                        "SELECT {1} FROM {0} WHERE {2} = ${2}",
                        TableName, Dict.FldnId, Dict.FldnWord);
                    _cmdFindIdByWord = Ctx.CreateCmd(sql);
                    _cmdFindIdByWord.Parameters.Add(Dict.FldnWord, DbType.String);
                    _cmdFindIdByWord.Prepare();
                }
                return _cmdFindIdByWord;
            }
        }
        #endregion

        public int Insert(Dict w2v)
        {
            CmdInsert.Parameters[Dict.FldnWord].Value = w2v.Word;
            CmdInsert.Parameters[Dict.FldnVect].Value = w2v.Vect;
            var res = CmdInsert.ExecuteNonQuery();
            w2v.Id = Ctx.LastInsertRowId;
            return res;
        }

        public int UpdateVectOfWord(Dict w2v)
        {
            CmdUpdateVectOfWord.Parameters[Dict.FldnWord].Value = w2v.Word;
            CmdUpdateVectOfWord.Parameters[Dict.FldnVect].Value = w2v.Vect;
            var res = CmdUpdateVectOfWord.ExecuteNonQuery();
            return res;
        }

        public long? FindIdByWord(string word)
        {
            CmdFindIdByWord.Parameters[Dict.FldnWord].Value = word;
            var res = CmdFindIdByWord.ExecuteScalar();
            return res == null || DBNull.Value.Equals(res) ? (long?)null : Convert.ToInt64(res);
        }

        public int ControlWordsIndex(bool is_enabled)
        {
            var sql = is_enabled
                ? $"CREATE INDEX IF NOT EXISTS inxWord{TableName} ON {TableName} ({Dict.FldnWord})"
                : $"DROP INDEX inxWord{TableName}";
            var cmd = Ctx.CreateCmd(sql);
            return cmd.ExecuteNonQuery();
        }

        public IEnumerable<string> GetWordsWithEmptyVect()
        {
            var sql = string.Format(
                       "SELECT {1} FROM {0} WHERE {2} IS NULL",
                       TableName, Dict.FldnWord, Dict.FldnVect);
            var cmd = Ctx.CreateCmd(sql);
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                    yield return rd.GetString(0);
            }
        }
    }
}
