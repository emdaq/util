using System;
using System.Collections.Generic;
using System.Data;

namespace Emdaq.DataAccess
{
    /// <summary>
    /// Supervises the use of connections/transactions across services.
    /// </summary>
    public class DataSupervisor
    {
        private IDbTransaction _transaction; // the current transaction, if there is one
        private IDbConnection _connection; // the current connection, if there is one
        private Func<IDbConnection> _openConnGetter; // func to get a new open connection
        
        private object _connOwner; // first guy to ConfirmConnection
        private object _transOwner; // first guy to ConfirmTransaction
        private readonly Stack<object> _transHolders = new Stack<object>(); // all guys to ConfirmTransaction 

        #region internal, for RepositoryBase

        internal void Register(Func<IDbConnection> cf)
        {
            if (_openConnGetter == null)
            {
                _openConnGetter = cf;
            }
            else if (_openConnGetter != cf)
            {
                throw new Exception("Cannot use Supervisor across different connections.");
            }
        }

        internal void Release(object owner)
        {
            // if releasing smallest scope transHolder, pop it off
            if (_transHolders.Count > 0 && _transHolders.Peek() == owner)
            {
                _transHolders.Pop();
            }

            // if releasing transaction owner, no longer need transaction
            if (owner == _transOwner)
            {
                _transOwner = null;

                // if there is still a transaction at this point, someone messed up
                if (_transaction != null)
                {
                    Rollback();
                }
            }

            // if releasing connection owner, nuke everything
            if (owner == _connOwner)
            {
                if (_connection != null)
                {
                    _connection.Close();
                }
                _transOwner = null;
                _connOwner = null;
            }
        }

        internal IDbConnection Conn
        {
            get
            {
                if (_connOwner == null)
                {
                    throw new Exception("Need to ConfirmConnection before using it.");
                }

                if (_connection == null || _connection.State != ConnectionState.Open)
                {
                    _connection = _openConnGetter();
                }

                return _connection;
            }
        }

        public IDbTransaction Trans
        {
            get
            {
                // if no one is in a ConfirmTransaction scope, no transaction
                if (_transOwner == null)
                {
                    return null;
                }

                return _transaction ?? (_transaction = Conn.BeginTransaction());
            }
        }

        #endregion

        /// <summary>
        /// Gaurentees that all db access in scope uses the same one connection.
        /// Returns a disposable context that should be used to mark scope.
        /// </summary>
        public SupervisedContext ConfirmConnection()
        {
            var conn = new SupervisedContext(this);

            if (_connOwner == null)
            {
                _connOwner = conn;
            }

            return conn;
        }

        /// <summary>
        /// Gaurentees that all db access in scope uses the same one transaction.
        /// You MUST Commit() or Rollback() inside this scope!
        /// Returns a disposable context that should be used to mark scope.
        /// </summary>
        public SupervisedContext ConfirmTransaction()
        {
            var conn = ConfirmConnection();

            if (_transOwner == null)
            {
                _transOwner = conn;
            }

            _transHolders.Push(conn);

            return conn;
        }

        /// <summary>
        /// Commit the scoped transaction. 
        /// NoOp if not inside ConfirmTransaction() scope.
        /// </summary>
        public void Commit()
        {
            if (_transHolders.Count == 1 && _transaction != null)
            {
                _transaction.Commit();
                _transaction = null;
            }

            // if > 1 transHolder, someone with bigger scope has a transaction, they need to commit
        }

        /// <summary>
        /// Rollback the scoped transaction. 
        /// NoOp if not inside ConfirmTransaction() scope.
        /// </summary>
        public void Rollback()
        {
            _transOwner = null;

            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }

            if (_connection != null)
            {
                _connection.Close();
            }
        }
    }
}
