using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IApartmentBuildingService
    {
        public Pagination<ApartmentBuildingDto> GetApartmentBuildings(RequestQueryBaseDto<object> request);
        public Task CreateApartmentBuilding(CreateApartmentBuildingDto request);
    }
}
