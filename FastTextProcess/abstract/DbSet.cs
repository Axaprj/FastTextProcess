using FastTextProcess.Entities;
using System;
using System.Data.SQLite;

namespace FastTextProcess
{
    /// <summary>
    /// Abstract DbSet
    /// </summary>
    public abstract class DbSet
    {
        protected readonly DbContext Ctx;
        protected internal readonly string TableName;

        internal DbSet(DbContext ctx, string table_name)
        {
            Ctx = ctx;
            TableName = table_name;
        }

        public int DeleteAll()
        {
            var cmd = Ctx.CreateCmd($"DELETE FROM {TableName}");
            return cmd.ExecuteNonQuery();
        }
    }
}
