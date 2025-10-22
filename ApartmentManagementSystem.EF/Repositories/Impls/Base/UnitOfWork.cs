using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;

namespace ApartmentManagementSystem.EF.Repositories.Impls.Base
{
    internal class UnitOfWork : IUnitOfWork
    {
        private DbFactory _dbFactory;
        public UnitOfWork(DbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public async Task<int> CommitAsync()
        {
            return await _dbFactory.DbContext.SaveChangesAsync();
        }
    }
}
