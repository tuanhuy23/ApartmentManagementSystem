using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Impls.Base;
using ApartmentManagementSystem.EF.Repositories.Interfaces;

namespace ApartmentManagementSystem.EF.Repositories.Impls
{
    internal class FeedbackRepository: Repository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(DbFactory dbFactory, UserAudit userAudit) : base(dbFactory, userAudit)
        {
        }
    }
}