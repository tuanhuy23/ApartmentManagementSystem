using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IApartmentBuildingService
    {
        public IEnumerable<ApartmentBuildingDto> GetApartmentBuildings();
        public Task CreateApartmentBuilding(CreateApartmentBuildingDto request);
    }
}
