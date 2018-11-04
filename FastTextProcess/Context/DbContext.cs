using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace FastTextProcess.Context
{
    /// <summary>
    /// Abstract DB context (base class)
    /// </summary>
    public abstract class DbContext:IDisposable
    {
        readonly SQLiteConnection _conn;

        public DbContext(string db_file)
        {
            _conn = new SQLiteConnection($"Data Source=\"{db_file}\";Version=3;");
            _conn.Open();
        }

        public void Dispose()
        {
            _conn.Dispose();
        }

        internal static int CreateDB(string db_file, string ddl_script)
        {
            SQLiteConnection.CreateFile(db_file);
            using (var conn = new SQLiteConnection($"Data Source=\"{db_file}\";Version=3;"))
            {
                conn.Open();
                var cmd = new SQLiteCommand(ddl_script, conn);
                return cmd.ExecuteNonQuery();
            }
        }

        public long LastInsertRowId { get { return _conn.LastInsertRowId; } }

        public SQLiteTransaction BeginTransaction() => _conn.BeginTransaction();

        public SQLiteCommand CreateCmd(string sql) => new SQLiteCommand(sql, _conn);
    }
}
