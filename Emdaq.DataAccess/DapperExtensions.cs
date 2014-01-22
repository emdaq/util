using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Emdaq.Util.Extensions;

namespace Emdaq.DataAccess
{
    public static class DapperExentsions
    {
        public static string BatchIdInsert(this IDbConnection conn, IEnumerable<int> ids, IDbTransaction trans = null)
        {
            var tmpTableName = "tmpIds" + DateTime.UtcNow.Ticks;

            const string insertTmpIds = @"
            CREATE TEMPORARY TABLE {0} (id INT NOT NULL);
            INSERT INTO {0} (id) VALUES ({1});
            CREATE INDEX IX_{0} ON {0} (id) USING HASH;";

            conn.Execute(insertTmpIds.Fmt(tmpTableName, string.Join("),(", ids)), null, trans);

            return tmpTableName;
        }

        public static string BatchIdInsert(this IDbConnection conn, IEnumerable<long> ids, IDbTransaction trans = null)
        {
            var tmpTableName = "tmpIds" + DateTime.UtcNow.Ticks;

            const string insertTmpIds = @"
            CREATE TEMPORARY TABLE {0} (id BIGINT NOT NULL);
            INSERT INTO {0} (id) VALUES ({1});
            CREATE INDEX IX_{0} ON {0} (id) USING HASH;";

            conn.Execute(insertTmpIds.Fmt(tmpTableName, string.Join("),(", ids)), null, trans);

            return tmpTableName;
        }

        public static IEnumerable<ManyMap<T>> ReadWithMap<T>(this SqlMapper.GridReader multi)
        {
            return multi.Read<T, ManyMap<T>, ManyMap<T>>((obj, map) =>
            {
                map.UnderlyingObj = obj;
                return map;
            }, splitOn: "ManyMapId");
        } 

        public static IEnumerable<ManyMap<T>> QueryMap<T>(this IDbConnection conn, string sql, object param = null, IDbTransaction trans = null, bool buffered = true)
        {
            return conn.Query<T, ManyMap<T>, ManyMap<T>>(sql, (obj, map) =>
                {
                    map.UnderlyingObj = obj;
                    return map;
                }, param, trans, buffered, "ManyMapId");
        } 
    }
}
