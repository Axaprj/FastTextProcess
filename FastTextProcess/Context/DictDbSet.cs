using FastTextProcess.Entities;
using System;
using System.Data;
using System.Data.SQLite;

namespace FastTextProcess.Context
{
    /// <summary>
    /// FastText dictionary DB: Dict/DictAddins DbSet
    /// </summary>
    public class DictDbSet : DbSet
    {
        /// <summary>
        /// Dictionary tables enumeration 
        /// </summary>
        public enum DictKind { Main, Addin }

        readonly string _table;
        SQLiteCommand _cmdInsert;
        SQLiteCommand _cmdFindIdByWord;

        public DictDbSet(DbContext ctx, DictKind db_kind) : base(ctx)
        {
            _table = db_kind == DictKind.Main ? "Dict" : "DictAddins";
        }
        #region SQL Commands
        SQLiteCommand CmdInsert
        {
            get
            {
                if (_cmdInsert == null)
                {
                    var sql = string.Format(
                        "INSERT INTO {0} ({1}, {2}) VALUES (${1}, ${2})",
                        _table, Dict.FldnWord, Dict.FldnVect);
                    _cmdInsert = Ctx.CreateCmd(sql);
                    _cmdInsert.Parameters.Add(Dict.FldnWord, DbType.String);
                    _cmdInsert.Parameters.Add(Dict.FldnVect, DbType.Binary);
                    _cmdInsert.Prepare();
                }
                return _cmdInsert;
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
                        _table, Dict.FldnId, Dict.FldnWord);
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

        public long? FindIdByWord(string word)
        {
            CmdFindIdByWord.Parameters[Dict.FldnWord].Value = word;
            var res = CmdFindIdByWord.ExecuteScalar();
            return res == null ? (long?)null : Convert.ToInt64(res);
        }

        public int ControlWordsIndex(bool is_enabled)
        {
            var sql = is_enabled
                ? $"CREATE INDEX IF NOT EXISTS inxWord{_table} ON {_table} ({Dict.FldnWord})"
                : $"DROP INDEX inxWord{_table}";
            var cmd = Ctx.CreateCmd(sql);
            return cmd.ExecuteNonQuery();
        }
    }
}
