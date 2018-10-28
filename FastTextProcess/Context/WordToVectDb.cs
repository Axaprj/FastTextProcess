using FastTextProcess.Entities;
using System;
using System.Data.SQLite;

namespace FastTextProcess.Context
{
    public class WordToVectDb : IDisposable
    {
        public enum DictDb { Main, Addin }

        const string FLDN_Word = "Word";
        const string FLDN_Vect = "Vect";
        readonly SQLiteConnection _conn;
        readonly string _table;
        SQLiteCommand _cmdInsert;
        
        public WordToVectDb(string db_file, DictDb db_kind)
        {
            _conn = new SQLiteConnection($"Data Source=\"{db_file}\";Version=3;");
            _conn.Open();
            _table = db_kind == DictDb.Main ? "Dict" : "DictAddins";
        }

        public void Dispose()
        {
            _conn.Dispose();
        }

        SQLiteCommand CmdInsert
        {
            get
            {
                if (_cmdInsert == null)
                {
                    var sql = $"INSERT INTO {_table} ([{FLDN_Word}], [{FLDN_Vect}]) VALUES (${FLDN_Word}, ${FLDN_Vect})";
                    _cmdInsert = new SQLiteCommand(sql, _conn);
                    _cmdInsert.Parameters.Add(FLDN_Word, System.Data.DbType.String);
                    _cmdInsert.Parameters.Add(FLDN_Vect, System.Data.DbType.Binary);
                    _cmdInsert.Prepare();
                }
                return _cmdInsert;
            }
        }

        public SQLiteTransaction BeginTransaction() => _conn.BeginTransaction();


        public int Insert(Word2Vect w2v)
        {
            CmdInsert.Parameters[FLDN_Word].Value = w2v.Word;
            CmdInsert.Parameters[FLDN_Vect].Value = w2v.Vect;
            var res = CmdInsert.ExecuteNonQuery();
            w2v.Id = _conn.LastInsertRowId;
            return res;
        }

        public static int ControlWordsIndex(string db_file, bool is_enabled)
        {
            using (var conn = new SQLiteConnection($"Data Source=\"{db_file}\";Version=3;"))
            {
                conn.Open();
                var sql = is_enabled
                    ? "CREATE INDEX IF NOT EXISTS inxWord ON {_table} (Word)"
                    : "DROP INDEX inxWord{_table}";
                var cmd = new SQLiteCommand(sql, conn);
                return cmd.ExecuteNonQuery();
            }
        }

        public static int CreateDB(string db_file)
        {
            SQLiteConnection.CreateFile(db_file);
            using (var conn = new SQLiteConnection($"Data Source=\"{db_file}\";Version=3;"))
            {
                conn.Open();
                var sql = Properties.Resources.word2vect_create;
                var cmd = new SQLiteCommand(sql, conn);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
