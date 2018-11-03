using FastTextProcess.Entities;
using System;
using System.Data.SQLite;

namespace FastTextProcess.Context
{
    public class WordToVectDbSet
    {
        public enum DictDb { Main, Addin }

        const string FLDN_Word = "Word";
        const string FLDN_Vect = "Vect";
        readonly string _table;
        SQLiteCommand _cmdInsert;
        DbContext _ctx;

        public WordToVectDbSet(DbContext ctx, DictDb db_kind)
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
                    var sql = $"INSERT INTO {_table} ([{FLDN_Word}], [{FLDN_Vect}]) VALUES (${FLDN_Word}, ${FLDN_Vect})";
                    _cmdInsert = _ctx.CreateCmd(sql);
                    _cmdInsert.Parameters.Add(FLDN_Word, System.Data.DbType.String);
                    _cmdInsert.Parameters.Add(FLDN_Vect, System.Data.DbType.Binary);
                    _cmdInsert.Prepare();
                }
                return _cmdInsert;
            }
        }

        public int Insert(Word2Vect w2v)
        {
            CmdInsert.Parameters[FLDN_Word].Value = w2v.Word;
            CmdInsert.Parameters[FLDN_Vect].Value = w2v.Vect;
            var res = CmdInsert.ExecuteNonQuery();
            w2v.Id = _ctx.LastInsertRowId;
            return res;
        }

        public int ControlWordsIndex(bool is_enabled)
        {
            var sql = is_enabled
                ? $"CREATE INDEX IF NOT EXISTS inxWord{_table} ON {_table} ({FLDN_Word})"
                : $"DROP INDEX inxWord{_table}";
            var cmd = _ctx.CreateCmd(sql);
            return cmd.ExecuteNonQuery();
        }
    }
}
