using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Impls.Base;
using ApartmentManagementSystem.EF.Repositories.Interfaces;

namespace ApartmentManagementSystem.EF.Repositories.Impls
{
    internal class ApartmentResidentsRepository: Repository<ApartmentResident>, IApartmentResidentsRepository
    {
        public ApartmentResidentsRepository(DbFactory dbFactory, UserAudit userAudit) : base(dbFactory, userAudit)
        {
        }
    }
}