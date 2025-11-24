using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IFeeConfigurationService
    {
        Task CreateOrUpdateFeeType(CreateOrUpdateFeeTypeDto request);
        Task<FeeTypeDto> GetFeeType(Guid id);
        Pagination<FeeTypeDto> GetFeeTypes(RequestQueryBaseDto<string> request);
    }
}