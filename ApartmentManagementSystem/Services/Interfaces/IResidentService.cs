using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IResidentService
    {
        public IEnumerable<ResidentDto> GetResidents(Guid apartmentId);
        public ResidentDto GetResident(Guid residentId);
        public Task CreateOrUpdateResident(ResidentDto request);
        public Task DeleteResident(Guid residentId);
    }
}
