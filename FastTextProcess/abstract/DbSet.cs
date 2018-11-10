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

        public DbSet(DbContext ctx) { Ctx = ctx; }
    }
}
