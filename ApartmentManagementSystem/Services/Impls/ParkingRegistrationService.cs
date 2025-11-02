using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Services.Interfaces;

namespace ApartmentManagementSystem.Services.Impls
{
    internal class ParkingRegistrationService : IParkingRegistrationService
    {
        private readonly IParkingRegistrationRepository _parkingRegistrationRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ParkingRegistrationService(IParkingRegistrationRepository parkingRegistrationRepository, IUnitOfWork unitOfWork)
        {
            _parkingRegistrationRepository = parkingRegistrationRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task CreateParkingRegistration(ParkingRegistrationDto request)
        {
            var parkingRegistration = new ParkingRegistration()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                ApartmentId = request.ApartmentId,
                VehicleType = request.VehicleType
            };
            await _parkingRegistrationRepository.Add(parkingRegistration);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<ParkingRegistrationDto>> GetParkingRegistrations(Guid aparmentId)
        {
            var parkingRegistration = _parkingRegistrationRepository.List(p => p.Equals(aparmentId)).Select(p => new ParkingRegistrationDto()
            {
                ApartmentBuildingId = p.ApartmentBuildingId,
                ApartmentId = p.ApartmentBuildingId,
                Id = p.Id,
                VehicleType = p.VehicleType
            });
            return parkingRegistration;
        }
    }
}