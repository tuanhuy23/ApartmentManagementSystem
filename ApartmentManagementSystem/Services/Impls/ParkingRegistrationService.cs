using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Exceptions;
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
        public async Task CreateOrUpdateParkingRegistration(ParkingRegistrationDto request)
        {
            if (request.Id == null)
            {
                var parkingRegistration = new ParkingRegistration()
                {
                    ApartmentBuildingId = request.ApartmentBuildingId,
                    ApartmentId = request.ApartmentId,
                    VehicleType = request.VehicleType
                };
                await _parkingRegistrationRepository.Add(parkingRegistration);
            }
            else
            {
                var parkingRegistration = _parkingRegistrationRepository.List(p => p.Id.Equals(request.Id.Value)).FirstOrDefault();
                if (parkingRegistration == null)
                    throw new DomainException(ErrorCodeConsts.ParkingRegistrationNotFound, ErrorMessageConsts.ParkingRegistrationNotFound, System.Net.HttpStatusCode.NotFound);
                parkingRegistration.VehicleDescription = request.VehicleDescription;
                parkingRegistration.VehicleNumber = request.VehicleNumber;
                parkingRegistration.VehicleType = request.VehicleType;
                _parkingRegistrationRepository.Update(parkingRegistration);
            }
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteParkingRegistration(List<string> ids)
        {
            var parkingIds = ids.Select(i => new Guid(i));
            var parkingRegistration = _parkingRegistrationRepository.List(p => parkingIds.Contains(p.Id)).ToList();
            _parkingRegistrationRepository.Delete(parkingRegistration);
            await _unitOfWork.CommitAsync();
        }

        public async Task<ParkingRegistrationDto> GetParkingRegistration(Guid id)
        {
            var parkingRegistration = _parkingRegistrationRepository.List(p => p.Id.Equals(id)).FirstOrDefault();
            if (parkingRegistration == null)
                throw new DomainException(ErrorCodeConsts.ParkingRegistrationNotFound, ErrorMessageConsts.ParkingRegistrationNotFound, System.Net.HttpStatusCode.NotFound);
            return new ParkingRegistrationDto()
            {
                ApartmentBuildingId = parkingRegistration.ApartmentBuildingId,
                ApartmentId = parkingRegistration.ApartmentId,
                Id = parkingRegistration.Id,
                VehicleDescription = parkingRegistration.VehicleDescription,
                VehicleNumber = parkingRegistration.VehicleNumber,
                VehicleType = parkingRegistration.VehicleType
            };
        }

        public Pagination<ParkingRegistrationDto> GetParkingRegistrations(RequestQueryBaseDto<Guid> request)
        {
            var parkingRegistration = _parkingRegistrationRepository.List(p => p.ApartmentId.Equals(request.Request)).Select(p => new ParkingRegistrationDto()
            {
                ApartmentBuildingId = p.ApartmentBuildingId,
                ApartmentId = p.ApartmentBuildingId,
                Id = p.Id,
                VehicleType = p.VehicleType
            });
            if (request.Filters != null && request.Filters.Any())
            {
                parkingRegistration = FilterHelper.ApplyFilters(parkingRegistration, request.Filters);
            }
            if (request.Sorts != null && request.Sorts.Any())
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