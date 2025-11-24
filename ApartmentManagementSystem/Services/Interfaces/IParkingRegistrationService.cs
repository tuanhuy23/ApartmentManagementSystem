using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IParkingRegistrationService
    {
        Pagination<ParkingRegistrationDto> GetParkingRegistrations(RequestQueryBaseDto<Guid> request);
        Task CreateParkingRegistration(ParkingRegistrationDto request);
    }
}