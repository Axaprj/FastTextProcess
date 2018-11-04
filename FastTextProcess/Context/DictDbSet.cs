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
        public enum DictDb { Main, Addin }

        readonly string _table;
        SQLiteCommand _cmdInsert;

        public DictDbSet(DbContext ctx, DictDb db_kind) : base(ctx)
        {
            _table = db_kind == DictDb.Main ? "Dict" : "DictAddins";
        }

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

        public int Insert(Dict w2v)
        {
            CmdInsert.Parameters[Dict.FldnWord].Value = w2v.Word;
            CmdInsert.Parameters[Dict.FldnVect].Value = w2v.Vect;
            var res = CmdInsert.ExecuteNonQuery();
            w2v.Id = Ctx.LastInsertRowId;
            return res;
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
