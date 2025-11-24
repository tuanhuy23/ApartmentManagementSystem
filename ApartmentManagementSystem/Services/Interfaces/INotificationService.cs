using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface INotificationService
    {
        public Task<IEnumerable<NotificationDto>> GetNotifications(string userId);
        public Pagination<AnnouncementDto> GetAnnouncements(RequestQueryBaseDto<Guid> request);
        public AnnouncementDto GetAnnouncement(Guid id);
        public Task CreateOrUpdateAnnouncements(AnnouncementDto request);
        public Task DeleteAnnouncements(Guid id);
        public Task CreateNotification(NotificationDto request);
        public Task DeleteNotification(Guid id);
        public Task MarkNotificationIsRead(Guid id);
    }
}