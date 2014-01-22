using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Emdaq.Util.Extensions;

namespace Emdaq.DataAccess
{
    /// <summary>
    /// Base for any class that interacts with the database
    /// </summary>
    public abstract class RepositoryBase
    {
        protected RepositoryBase(DataSupervisor supervisor, Func<IDbConnection> connGetter)
        {
            Supervisor = supervisor;
            Supervisor.Register(connGetter);
        }

        protected DataSupervisor Supervisor { get; private set; }
        protected IDbConnection Conn { get { return Supervisor.Conn; } }
        protected IDbTransaction Trans { get { return Supervisor.Trans; } }

        #region helpers for standard dapper

        protected int Execute(string sql, object param = null, int? timeout = null)
        {
            using (Supervisor.ConfirmConnection())
            {
                return Supervisor.Conn.Execute(sql, param, Supervisor.Trans, timeout);
            }
        }

        protected IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true)
        {
            using (Supervisor.ConfirmConnection())
            {
                return Supervisor.Conn.Query<T>(sql, param, Supervisor.Trans, buffered); 
            }
        }

        protected IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id")
        {
            using (Supervisor.ConfirmConnection())
            {
                return Supervisor.Conn.Query(sql, map, param, Supervisor.Trans, buffered, splitOn);
            }
        }

        #endregion

        #region helpers for emdaq dapper additions

        protected string BatchIdInsert(IEnumerable<int> ids)
        {
            using (Supervisor.ConfirmConnection())
            {
                return Supervisor.Conn.BatchIdInsert(ids, Supervisor.Trans);
            }
        }

        protected string BatchIdInsert(IEnumerable<long> ids)
        {
            using (Supervisor.ConfirmConnection())
            {
                return Supervisor.Conn.BatchIdInsert(ids, Supervisor.Trans);
            }
        }

        protected IEnumerable<ManyMap<T>> QueryMap<T>(string sql, object param = null, bool buffered = true)
        {
            using (Supervisor.ConfirmConnection())
            {
                return Supervisor.Conn.QueryMap<T>(sql, param, Supervisor.Trans, buffered);
            }
        }

        protected int QueryLastInsertId(string sql, object param = null)
        {
            using (Supervisor.ConfirmConnection())
            {
                return (int)Supervisor.Conn.Query<UInt64>(sql, param, Supervisor.Trans).Single();
            }
        }

        protected IEnumerable<T> QueryWithIdFilter<T>(string sql, IEnumerable<int> ids = null, 
                                                      string idColumnName = "id", object param = null, bool buffered = true)
        {
            return InternalQueryWithIdFilter(sql, ids, idColumnName, param, buffered, Query<T>);
        }

        protected IEnumerable<T> QueryWithIdFilter<T>(string sql, IEnumerable<long> ids = null,
                                                      string idColumnName = "id", object param = null, bool buffered = true)
        {
            return InternalQueryWithIdFilter(sql, ids, idColumnName, param, buffered, Query<T>);
        }

        protected IEnumerable<T> QueryWithIdFilter<T>(string sql, IEnumerable<string> ids = null,
                                                      string idColumnName = "id", object param = null, bool buffered = true)
        {
            if (ids == null)
            {
                return Query<T>(sql, param, buffered);
            }

            sql = sql.TrimEnd(';') + " WHERE {0} IN ('".Fmt(idColumnName) + string.Join("', '", ids) + "');";

            return Query<T>(sql, param, buffered);
        } 

        protected IEnumerable<TReturn> QueryWithIdFilter<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, IEnumerable<int> ids = null,
                                                                                   string idColumnName = "id", object param = null, bool buffered = true, string splitOn = "Id")
        {
            return InternalQueryWithIdFilter(sql, ids, idColumnName, param, buffered,
                                             (sqlQuery, sqlParam, buff) => Query(sqlQuery, map, sqlParam, buff, splitOn));
        }

        protected IEnumerable<ManyMap<T>> QueryMapWithIdFilter<T>(string sql, IEnumerable<int> ids = null,
                                                                  string idColumnName = "id", object param = null, bool buffered = true)
        {
            return InternalQueryWithIdFilter(sql, ids, idColumnName, param, buffered, QueryMap<T>);
        }

        protected IEnumerable<ManyMap<T>> QueryMapWithIdFilter<T>(string sql, IEnumerable<long> ids = null,
                                                                    string idColumnName = "id", object param = null, bool buffered = true)
        {
            return InternalQueryWithIdFilter(sql, ids, idColumnName, param, buffered, QueryMap<T>);
        }

        private IEnumerable<T> InternalQueryWithIdFilter<T>(string sql, IEnumerable<int> ids, string idColumnName,
                                                            object param, bool buffered, Func<string, object, bool, IEnumerable<T>> actualQueryFunc)
        {
            if (ids == null)
            {
                return actualQueryFunc(sql, param, buffered);
            }

            using (Supervisor.ConfirmConnection())
            {
                var tmpTableName = BatchIdInsert(ids);

                sql = sql.TrimEnd(';') + " JOIN {0} tmpIds on tmpIds.id = {1};".Fmt(tmpTableName, idColumnName);

                return actualQueryFunc(sql, param, buffered);
            }
        }

        private IEnumerable<T> InternalQueryWithIdFilter<T>(string sql, IEnumerable<long> ids, string idColumnName,
                                                            object param, bool buffered, Func<string, object, bool, IEnumerable<T>> actualQueryFunc)
        {
            if (ids == null)
            {
                return actualQueryFunc(sql, param, buffered);
            }

            using (Supervisor.ConfirmConnection())
            {
                var tmpTableName = BatchIdInsert(ids);

                sql = sql.TrimEnd(';') + " JOIN {0} tmpIds on tmpIds.id = {1};".Fmt(tmpTableName, idColumnName);

                return actualQueryFunc(sql, param, buffered);
            }
        }

        #endregion
    }
}