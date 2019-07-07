using FastTextProcess.Entities;
using FastTextProcess.Enums;
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
        SQLiteCommand _cmdInsertOrReplace;
        SQLiteCommand _cmdFindIdByWord;
        SQLiteCommand _cmdUpdateVectOfWord;

        SQLiteCommand CmdInsert
        {
            get
            {
                if (_cmdInsert == null)
                {
                    var sql = string.Format(
                        "INSERT INTO {0} ({1}, {2}, {3}) VALUES (${1}, ${2}, ${3})",
                        TableName, Dict.FldnWord, Dict.FldnVect, Dict.FldnLangId);
                    _cmdInsert = Ctx.CreateCmd(sql);
                    _cmdInsert.Parameters.Add(Dict.FldnWord, DbType.String);
                    _cmdInsert.Parameters.Add(Dict.FldnVect, DbType.Binary);
                    _cmdInsert.Parameters.Add(Dict.FldnLangId, DbType.Int32);
                    _cmdInsert.Prepare();
                }
                return _cmdInsert;
            }
        }

        SQLiteCommand CmdInsertOrReplace
        {
            get
            {
                if (_cmdInsertOrReplace == null)
                {
                    var sql = string.Format(
                        "INSERT OR REPLACE INTO {0} ({1}, {2}, {3}) VALUES (${1}, ${2}, ${3})",
                        TableName, Dict.FldnWord, Dict.FldnVect, Dict.FldnLangId);
                    _cmdInsertOrReplace = Ctx.CreateCmd(sql);
                    _cmdInsertOrReplace.Parameters.Add(Dict.FldnWord, DbType.String);
                    _cmdInsertOrReplace.Parameters.Add(Dict.FldnVect, DbType.Binary);
                    _cmdInsertOrReplace.Parameters.Add(Dict.FldnLangId, DbType.Int32);
                    _cmdInsertOrReplace.Prepare();
                }
                return _cmdInsertOrReplace;
            }
        }


        SQLiteCommand CmdUpdateVectOfWord
        {
            get
            {
                if (_cmdUpdateVectOfWord == null)
                {
                    var sql = string.Format(
                        "UPDATE {0} SET {1}=${1} WHERE {2}=${2} AND {3}=${3}",
                        TableName, Dict.FldnVect, Dict.FldnWord, Dict.FldnLangId);
                    _cmdUpdateVectOfWord = Ctx.CreateCmd(sql);
                    _cmdUpdateVectOfWord.Parameters.Add(Dict.FldnWord, DbType.String);
                    _cmdUpdateVectOfWord.Parameters.Add(Dict.FldnVect, DbType.Binary);
                    _cmdUpdateVectOfWord.Parameters.Add(Dict.FldnLangId, DbType.Int32);
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
                        "SELECT {1} FROM {0} WHERE {2} = ${2} AND ({3} = ${3} OR {3} = {4}) ",
                        TableName, Dict.FldnId, Dict.FldnWord, Dict.FldnLangId, (int)FTLangLabel.NotSpecified);
                    _cmdFindIdByWord = Ctx.CreateCmd(sql);
                    _cmdFindIdByWord.Parameters.Add(Dict.FldnWord, DbType.String);
                    _cmdFindIdByWord.Parameters.Add(Dict.FldnLangId, DbType.Int32);
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
            CmdInsert.Parameters[Dict.FldnLangId].Value = w2v.Lang;
            var res = CmdInsert.ExecuteNonQuery();
            w2v.Id = Ctx.LastInsertRowId;
            return res;
        }

        public int InsertOrReplace(Dict w2v)
        {
            CmdInsertOrReplace.Parameters[Dict.FldnWord].Value = w2v.Word;
            CmdInsertOrReplace.Parameters[Dict.FldnVect].Value = w2v.Vect;
            CmdInsertOrReplace.Parameters[Dict.FldnLangId].Value = w2v.Lang;
            var res = CmdInsertOrReplace.ExecuteNonQuery();
            w2v.Id = Ctx.LastInsertRowId;
            return res;
        }

        public int UpdateVectOfWord(Dict w2v)
        {
            CmdUpdateVectOfWord.Parameters[Dict.FldnWord].Value = w2v.Word;
            CmdUpdateVectOfWord.Parameters[Dict.FldnVect].Value = w2v.Vect;
            CmdUpdateVectOfWord.Parameters[Dict.FldnLangId].Value = w2v.Lang;
            var res = CmdUpdateVectOfWord.ExecuteNonQuery();
            return res;
        }

        public long? FindIdByWord(string word, FTLangLabel lang)
        {
            CmdFindIdByWord.Parameters[Dict.FldnWord].Value = word;
            CmdFindIdByWord.Parameters[Dict.FldnLangId].Value = (int)lang;
            var res = CmdFindIdByWord.ExecuteScalar();
            return res == null || DBNull.Value.Equals(res) ? (long?)null : Convert.ToInt64(res);
        }

        public int ControlWordsIndex(bool is_enabled)
        {
            var sql = is_enabled
                ? $"CREATE INDEX IF NOT EXISTS inxWord{TableName} ON {TableName} ({Dict.FldnWord}, {Dict.FldnLangId})"
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

        public IEnumerable<Dict> GetAll(FTLangLabel lang)
        {
            var sql = string.Format(
                       "SELECT {1}, {2}, {3}, {4} FROM {0} WHERE {4} = ${4}",
                       TableName
                       , Dict.FldnId, Dict.FldnWord, Dict.FldnVect, Dict.FldnLangId);
            var cmd = Ctx.CreateCmd(sql);
            cmd.Parameters.AddWithValue(Dict.FldnLangId, (int)lang);
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                    yield return new Dict
                    {
                        Id = rd.GetInt64(0),
                        Word = rd.GetString(1),
                        Vect = (byte[])rd[2],
                        Lang = (FTLangLabel)rd.GetInt32(3)
                    }; 
            }
        }

        public Dict FindByWord(string word, FTLangLabel lang)
        {
            var sql = string.Format(
                       "SELECT {1}, {2}, {3}, {4} FROM {0} WHERE {2} = ${2} AND ({4} = ${4} OR {4} = {5}) ",
                       TableName
                       , Dict.FldnId, Dict.FldnWord, Dict.FldnVect, Dict.FldnLangId, (int)FTLangLabel.NotSpecified);
            var cmd = Ctx.CreateCmd(sql);
            cmd.Parameters.AddWithValue(Dict.FldnWord, word);
            cmd.Parameters.AddWithValue(Dict.FldnLangId, (int)lang);
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                    return new Dict
                    {
                        Id = rd.GetInt64(0),
                        Word = rd.GetString(1),
                        Vect = (byte[])rd[2],
                        Lang = (FTLangLabel)rd.GetInt32(3)
                    };
            }
            return null;
        }

    }
}
