using ApartmentManagementSystem.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.EF
{
    internal class DbFactory : IDisposable
    {
        private bool _disposed;
        private Func<ApartmentManagementDbContext> _instanceFunc;
        private DbContext _dbContext;
        public DbContext DbContext => _dbContext ?? (_dbContext = _instanceFunc.Invoke());
        public DbFactory(Func<ApartmentManagementDbContext> instanceFunc)
        {
            _instanceFunc = instanceFunc;
        }
        public void Dispose()
        {
            if (!_disposed && _dbContext != null)
            {
                _disposed = true;
                _dbContext.Dispose();
            }
        }
    }
}
