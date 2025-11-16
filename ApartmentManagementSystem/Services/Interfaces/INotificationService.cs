using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface INotificationService
    {
        public Task<IEnumerable<NotificationDto>> GetNotifications(string userId);
        public IEnumerable<AnnouncementDto> GetAnnouncements(Guid apartmentBuildingId);
        public AnnouncementDto GetAnnouncement(Guid id);
        public Task CreateOrUpdateAnnouncements(AnnouncementDto request);
        public Task DeleteAnnouncements(Guid id);
        public Task CreateNotification(NotificationDto request);
        public Task DeleteNotification(Guid id);
        public Task MarkNotificationIsRead(Guid id);
    }
}