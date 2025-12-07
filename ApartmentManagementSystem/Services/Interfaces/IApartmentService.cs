using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IApartmentService
    {
        Pagination<ApartmentDto> GetApartments(RequestQueryBaseDto<string> request);
        Task<ApartmentDto> GetApartment(Guid id);
        Task CreateApartment(ApartmentDto request);
        Task UpdateApartment(UpdateApartmentDto request);
        Task DeleteApartment(Guid id);
    }
}