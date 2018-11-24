using FastTextProcess.Entities;
using FastTextProcess.Properties;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace FastTextProcess.Context
{
    /// <summary>
    /// FastText dictionary DB context
    /// TAGS: thread_safe
    /// </summary>
    public class FastTextProcessDB : DbContext
    {
        public FastTextProcessDB(string db_file, bool foreign_keys = true)
            : base(db_file, foreign_keys) { }

        public static void CreateDB(string db_file) => CreateDB(db_file, Resources.word2vect_create);

        public DictDbSet Dict(DictDbSet.DictKind db_kind) => new DictDbSet(this, db_kind);

        public EmbedDictDbSet EmbedDict() => new EmbedDictDbSet(this);

        public int ProcessEmbedJoins(Action<EmbedJoin> actProcess, long from_inx = 0)
        {
            int cnt = 0;
            var sql = string.Format(
                    "SELECT {1}, {2}, {3} FROM {0} WHERE {1} >= ${1}",
                    EmbedJoin.TblName, EmbedJoin.FldnInx, EmbedJoin.FldnVect, EmbedJoin.FldnFreq);
            var cmd = CreateCmd(sql);
            cmd.Parameters.AddWithValue(EmbedJoin.FldnInx, from_inx);
            var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var ej = new EmbedJoin
                {
                    Inx = rd.GetInt64(0),
                    Vect = (byte[])rd.GetValue(1),
                    Freq = rd.GetInt64(2)
                };
                actProcess(ej);
                cnt++;
            }
            return cnt;
        }

    }
}
