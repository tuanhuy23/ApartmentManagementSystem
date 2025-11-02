using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IBillingCycleSettingService
    {
        Task<BillingCycleSettingDto> GetBillingCycleSetting(string apartmentBuildingId);
        Task CreateBillingCycleSetting(BillingCycleSettingDto request);
    }
}