using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IFeeConfigurationService
    {
        Task CreateFeeType(CreateFeeTypeDto request);
        Task<FeeTypeDto> GetFeeType(Guid id);
        Task<IEnumerable<FeeTypeDto>> GetFeeTypes(string appartmentBuildingId);
        Task<IEnumerable<FeeRateConfigDto>> GetFeeRateConfigs(Guid feeTypeId);
        Task CreateFeeRateConfig(CreateFeeRateConfigDto request, Guid feeTypeId);
        Task UpdateFeeRateConfig(UpdateFeeRateConfigDto request);
        Task DeleteFeeRateConfig(Guid id);
    }
}