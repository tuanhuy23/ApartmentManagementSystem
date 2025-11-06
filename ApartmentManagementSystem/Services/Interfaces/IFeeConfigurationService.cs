using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IFeeConfigurationService
    {
        Task CreateOrUpdateFeeType(CreateOrUpdateFeeTypeDto request);
        Task<FeeTypeDto> GetFeeType(Guid id);
        Task<IEnumerable<FeeTypeDto>> GetFeeTypes(string appartmentBuildingId);
    }
}