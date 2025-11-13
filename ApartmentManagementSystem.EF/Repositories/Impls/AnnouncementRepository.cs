using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Impls.Base;
using ApartmentManagementSystem.EF.Repositories.Interfaces;

namespace ApartmentManagementSystem.EF.Repositories.Impls
{
    internal class AnnouncementRepository: Repository<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(DbFactory dbFactory, UserAudit userAudit) : base(dbFactory, userAudit)
        {
        }
    }
}