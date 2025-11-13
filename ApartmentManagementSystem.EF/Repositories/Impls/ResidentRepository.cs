using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Impls.Base;
using ApartmentManagementSystem.EF.Repositories.Interfaces;

namespace ApartmentManagementSystem.EF.Repositories.Impls
{
    internal class ResidentRepository : Repository<Resident>, IResidentRepository
    {
        public ResidentRepository(DbFactory dbFactory, UserAudit userAudit) : base(dbFactory, userAudit)
        {
        }
    }
}