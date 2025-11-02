using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IApartmentService
    {
        Task<IEnumerable<ApartmentDto>> GetApartments(string apartmentBuildingId);
        Task<ApartmentDto> GetApartment(Guid id);
        Task CreateApartment(ApartmentDto request);
        Task UpdateApartment(UpdateApartmentDto request, Guid id);
        Task DeleteApartment(Guid id);
    }
}