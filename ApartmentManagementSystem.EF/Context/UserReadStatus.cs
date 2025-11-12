using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class UserReadStatus : EntityBase<Guid>
    {
        public string UserId { get; set; }
        public Guid AnnouncementId { get; set; }
        public Announcement Announcement { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public DateTime ReadAt{ get; set; }
    }
}