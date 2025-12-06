using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IParkingRegistrationService
    {
        Pagination<ParkingRegistrationDto> GetParkingRegistrations(RequestQueryBaseDto<Guid> request);
        Task CreateOrUpdateParkingRegistration(ParkingRegistrationDto request);
        Task<ParkingRegistrationDto> GetParkingRegistration(Guid id);
        Task DeleteParkingRegistration(List<string> ids);
    }
}