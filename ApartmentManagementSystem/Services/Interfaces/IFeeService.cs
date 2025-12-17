using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IFeeService
    {
        Task CreateFeeNotice(IEnumerable<CreateOrUpdateFeeNoticeDto> requests);
        Task<FeeNoticeDto> GetFeeDetail(Guid id);
        Pagination<FeeNoticeDto> GetFeeNotices(RequestQueryBaseDto<Guid> request);
        Task CancelFeeNotice(Guid id);
        Task UpdatePaymentStatusFeeNotice(Guid id);
        Task DeletFeeeNotice(List<string> ids);
        Pagination<UtilityReadingDto> GetUtilityReadings(RequestQueryBaseDto<Guid> request);
        byte[] DownloadExcelTemplate(string fileName, string sheetName, string apartmentId);
        Task<IEnumerable<ImportFeeNoticeResult>> ImportFeeNoticeResult(string apartmentBuildingId, IFormFile file);
    }
}