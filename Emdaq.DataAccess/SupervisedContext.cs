using System;

namespace Emdaq.DataAccess
{
    /// <summary>
    /// Used only to scope a connection or transaction
    /// </summary>
    public class SupervisedContext : IDisposable
    {
        private readonly DataSupervisor _supervisor;

        internal SupervisedContext(DataSupervisor supervisor)
        {
            _supervisor = supervisor;
        }

        public void Dispose()
        {
            _supervisor.Release(this);
        }
    }
}