using FastTextProcess.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Context
{
    /// <summary>
    /// FastText processing result DB context
    /// </summary>
    public class FastTextResultDB : DbContext
    {
        public FastTextResultDB(string db_file, bool foreign_keys = true)
            : base(db_file, foreign_keys) { }

        public static void CreateDB(string db_file) => CreateDB(db_file, Resources.vect_result_create);

        //public EmbedDictDbSet EmbedDict() => new EmbedDictDbSet(this);
    }
}
