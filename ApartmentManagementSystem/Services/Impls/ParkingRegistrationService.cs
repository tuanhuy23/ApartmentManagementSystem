using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
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

        public Pagination<ParkingRegistrationDto> GetParkingRegistrations(RequestQueryBaseDto<Guid> request)
        {
            var parkingRegistration = _parkingRegistrationRepository.List(p => p.Equals(request.Request)).Select(p => new ParkingRegistrationDto()
            {
                ApartmentBuildingId = p.ApartmentBuildingId,
                ApartmentId = p.ApartmentBuildingId,
                Id = p.Id,
                VehicleType = p.VehicleType
            });
            if (request.Filters!= null && request.Filters.Any())
            {
                parkingRegistration = FilterHelper.ApplyFilters(parkingRegistration, request.Filters);
            }
            if (request.Sorts!= null && request.Sorts.Any())
            {
                parkingRegistration = SortHelper.ApplySort(parkingRegistration, request.Sorts);
            }
            return new Pagination<ParkingRegistrationDto>()
            {
                Items = parkingRegistration.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = parkingRegistration.Count()
            };
        }
    }
}