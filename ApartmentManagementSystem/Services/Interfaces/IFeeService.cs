using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IFeeService
    {
        Task CreateFeeNotice(CreateOrUpdateFeeNoticeDto request);
        Task<FeeNoticeDto> GetFeeDetail(Guid id);
        Pagination<FeeNoticeDto> GetFeeNotices(RequestQueryBaseDto<Guid> request);
        Task UpdateFeeNotice(CreateOrUpdateFeeNoticeDto request);
        Pagination<UtilityReadingDto> GetUtilityReadings(RequestQueryBaseDto<Guid> request);
    }
}