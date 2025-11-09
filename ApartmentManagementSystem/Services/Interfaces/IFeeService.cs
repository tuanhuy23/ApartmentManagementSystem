using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IFeeService
    {
        Task CreateFeeNotice(CreateOrUpdateFeeNoticeDto request);
        Task<FeeNoticeDto> GetFeeDetail(Guid id);
        Task<IEnumerable<FeeNoticeDto>> GetFeeNotices(Guid apartmentId);
        Task UpdateFeeNotice(CreateOrUpdateFeeNoticeDto request);
        Task<IEnumerable<UtilityReadingDto>> GetUtilityReadings(Guid apartmentId);
    }
}