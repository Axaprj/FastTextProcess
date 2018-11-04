using FastTextProcess.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Context
{
    /// <summary>
    /// FastText dictionary DB context
    /// </summary>
    public class FastTextProcessDB : DbContext
    {
        public FastTextProcessDB(string db_file) : base(db_file) { }

        public static void CreateDB(string db_file) => CreateDB(db_file, Resources.word2vect_create);

        public DictDbSet Dict(DictDbSet.DictDb db_kind) => new DictDbSet(this, db_kind);

        public EmbedDictDbSet EmbedDict() => new EmbedDictDbSet(this);
    }
}
