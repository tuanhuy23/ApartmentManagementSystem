using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface INotificationService
    {
        public Task<Pagination<AnnouncementDto>> GetAnnouncements(RequestQueryBaseDto<Guid> request);
        public AnnouncementDto GetAnnouncement(Guid id);
        public Task CreateOrUpdateAnnouncements(AnnouncementDto request);
        public Task DeleteAnnouncements(List<string> ids);
        byte[] DownloadExcelTemplate(string fileName, string sheetName);
        Task<IEnumerable<ApartmentAnnouncementDto>> ImportApartmentIdResult(string apartmentBuildingId, IFormFile file);
        IEnumerable<ApartmentAnnouncementDto> GetApartmentData(string apartmentBuildingId);
    }
}