using FastTextProcess.Entities;
using System;
using System.Data.SQLite;

namespace FastTextProcess.Context
{
    /// <summary>
    /// FastText dictionary DB: Dict/DictAddins DbSet
    /// </summary>
    public class DictDbSet
    {
        public enum DictDb { Main, Addin }

        readonly string _table;
        readonly DbContext _ctx;
        SQLiteCommand _cmdInsert;
        

        public DictDbSet(DbContext ctx, DictDb db_kind)
        {
            _ctx = ctx;
            _table = db_kind == DictDb.Main ? "Dict" : "DictAddins";
        }

        SQLiteCommand CmdInsert
        {
            get
            {
                if (_cmdInsert == null)
                {
                    var sql = $"INSERT INTO {_table} ([{Dict.FldnWord}], [{Dict.FldnVect}])" 
                        +$" VALUES (${Dict.FldnWord}, ${Dict.FldnVect})";
                    _cmdInsert = _ctx.CreateCmd(sql);
                    _cmdInsert.Parameters.Add(Dict.FldnWord, System.Data.DbType.String);
                    _cmdInsert.Parameters.Add(Dict.FldnVect, System.Data.DbType.Binary);
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
            w2v.Id = _ctx.LastInsertRowId;
            return res;
        }

        public int ControlWordsIndex(bool is_enabled)
        {
            var sql = is_enabled
                ? $"CREATE INDEX IF NOT EXISTS inxWord{_table} ON {_table} ({Dict.FldnWord})"
                : $"DROP INDEX inxWord{_table}";
            var cmd = _ctx.CreateCmd(sql);
            return cmd.ExecuteNonQuery();
        }
    }
}
