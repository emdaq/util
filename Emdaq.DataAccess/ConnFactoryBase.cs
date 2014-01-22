using System;
using System.Data.Common;
using Emdaq.Util;
using StackExchange.Profiling.Data;

namespace Emdaq.DataAccess
{
    public abstract class ConnFactoryBase
    {
        private readonly Func<DbConnection> _getConnection;
        private readonly Func<IDbProfiler> _getProfiler;

        protected ConnFactoryBase(Func<DbConnection> getConnection, Func<IDbProfiler> getProfiler)
        {
            _getConnection = getConnection;
            _getProfiler = getProfiler;
        }

        public DbConnection OpenConn(string connStringName)
        {
            var conn = _getConnection();
            conn.ConnectionString = ConfigManager.GetConnString(connStringName);
            conn.Open();
            return conn;
        }

        public ProfiledDbConnection OpenProfiledConn(string connStringName)
        {
            var conn = _getConnection();
            conn.ConnectionString = ConfigManager.GetConnString(connStringName);
            conn.Open();
            return new ProfiledDbConnection(conn, _getProfiler());
        }
    }
}
