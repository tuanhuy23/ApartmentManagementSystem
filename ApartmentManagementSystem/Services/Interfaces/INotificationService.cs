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
        public Task DeleteAnnouncements(List<string> ids);
        public Task CreateNotification(CreateNotificationDto request);
        public Task DeleteNotification(List<DeleteNotificationDto> request);
        public Task MarkNotificationIsRead(List<string> ids);
    }
} 