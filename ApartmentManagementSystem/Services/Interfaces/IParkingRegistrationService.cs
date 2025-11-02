using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IParkingRegistrationService
    {
        Task<IEnumerable<ParkingRegistrationDto>> GetParkingRegistrations(Guid aparmentId);
        Task CreateParkingRegistration(ParkingRegistrationDto request);
    }
}