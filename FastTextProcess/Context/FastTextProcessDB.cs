using FastTextProcess.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastTextProcess.Context
{
    public class FastTextProcessDB : DbContext
    {
        public FastTextProcessDB(string db_file) : base(db_file) { }

        public static void CreateDB(string db_file) => CreateDB(db_file, Resources.word2vect_create);

        public WordToVectDbSet WordToVect(WordToVectDbSet.DictDb db_kind) => new WordToVectDbSet(this, db_kind);
    }
}
