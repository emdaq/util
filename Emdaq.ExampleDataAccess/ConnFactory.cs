using System;
using System.Data.Common;
using Emdaq.DataAccess;
using StackExchange.Profiling.Data;

namespace Emdaq.ExampleDataAccess
{
    public interface IConnFactory
    {
        DbConnection OpenEmdaqUnitTestConn();
        ProfiledDbConnection OpenEmdaqConn();
    }

    public class ConnFactory : ConnFactoryBase, IConnFactory
    {
        #region singleton

        private ConnFactory(Func<DbConnection> getConnection, Func<IDbProfiler> getProfiler) : base(getConnection, getProfiler)
        { }

        public static ConnFactory I { get; private set; }

        public static void Initialize<T>() where T : DbConnection, new()
        {
            I = new ConnFactory(() => new T(), () => null);
        }

        public static void Initialize<T>(Func<IDbProfiler> getProfiler) where T : DbConnection, new()
        {
            I = new ConnFactory(() => new T(), getProfiler);
        }

        #endregion
        
        public DbConnection OpenEmdaqUnitTestConn()
        {
            return OpenConn("Emdaq");
        }

        public ProfiledDbConnection OpenEmdaqConn()
        {
            return OpenProfiledConn("Emdaq");
        }
    }
}
