using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace FastTextProcess
{
    /// <summary>
    /// Abstract DB context (base class)
    /// </summary>
    public abstract class DbContext:IDisposable
    {
        readonly SQLiteConnection _conn;

        public DbContext(string db_file, bool foreign_keys=true)
        {
            var conn_str = $"Data Source=\"{db_file}\";Version=3;";
            if(foreign_keys)
                conn_str += "foreign keys=true;";
            _conn = new SQLiteConnection(conn_str);
            _conn.Open();
        }

        public void Dispose()
        {
            _conn.Close();
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
