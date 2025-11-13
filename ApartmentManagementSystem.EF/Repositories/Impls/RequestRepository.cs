using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Impls.Base;
using ApartmentManagementSystem.EF.Repositories.Interfaces;

namespace ApartmentManagementSystem.EF.Repositories.Impls
{
    internal class RequestRepository: Repository<Request>, IRequestRepository
    {
        public RequestRepository(DbFactory dbFactory, UserAudit userAudit) : base(dbFactory, userAudit)
        {
        }
    }
}