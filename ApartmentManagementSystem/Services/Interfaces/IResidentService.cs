using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IResidentService
    {
        public Pagination<ResidentDto> GetResidents(RequestQueryBaseDto<Guid> request);
        public Task<ResidentDto> GetResident(Guid residentId, Guid apartmentId);
        public Task CreateOrUpdateResident(ResidentDto request);
        public Task DeleteResident(Guid apartmentId, List<string> ids);
    }
}
