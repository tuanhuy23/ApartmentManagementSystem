using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IResidentService
    {
        public IEnumerable<ResidentDto> GetResidents(Guid apartmentId);
        public Task<ResidentDto> GetResident(Guid residentId, Guid apartmentId);
        public Task CreateOrUpdateResident(ResidentDto request);
        public Task DeleteResident(Guid residentId);
    }
}
