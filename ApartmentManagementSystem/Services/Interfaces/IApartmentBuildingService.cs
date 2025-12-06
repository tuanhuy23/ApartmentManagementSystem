using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IApartmentBuildingService
    {
        public Pagination<ApartmentBuildingDto> GetApartmentBuildings(RequestQueryBaseDto<object> request);
        public Task CreateOrUpdateApartmentBuilding(CreateOrUpdateApartmentBuildingDto request);
        public Task<ApartmentBuildingDto> GetApartmentBuilding(Guid id);
        public Task DeleteApartmentBuilding(List<string> ids);
        public Task UpdateApartmentBuildingStatus(Guid id, UpdateStatusApartmentBuildingDto request);
    }
}
