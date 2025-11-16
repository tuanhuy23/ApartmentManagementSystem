using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;

namespace ApartmentManagementSystem.Services.Impls
{
    internal class ApartmentService : IApartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApartmentRepository _apartmentRepository;
        public ApartmentService(IUnitOfWork unitOfWork, IApartmentRepository apartmentRepository)
        {
            _unitOfWork = unitOfWork;
            _apartmentRepository = apartmentRepository;
        }
        public async Task CreateApartment(ApartmentDto request)
        {
            var apartment = new Apartment()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                Area = request.Area,
                Floor = request.Floor,
                Name = request.Name,
                Building = string.Empty
            };
            await _apartmentRepository.Add(apartment);
            await _unitOfWork.CommitAsync();
        }

        public Task DeleteApartment(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApartmentDto> GetApartment(Guid id)
        {
            var apartments = _apartmentRepository.List(a => a.Id.Equals(id)).FirstOrDefault();
            if (apartments == null) 
                throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorCodeConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);
            return new ApartmentDto()
            {
                Id = apartments.Id,
                ApartmentBuildingId = apartments.ApartmentBuildingId,
                Name = apartments.Name,
                Area = apartments.Area,
                Floor = apartments.Floor
            };
        }

        public async Task<IEnumerable<ApartmentDto>> GetApartments(string apartmentBuildingId)
        {
            var apartments = _apartmentRepository.List(a => a.ApartmentBuildingId.Equals(new Guid(apartmentBuildingId))).Select(a => new ApartmentDto()
            {
                Id = a.Id,
                ApartmentBuildingId = a.ApartmentBuildingId,
                Name = a.Name,
                Area = a.Area,
                Floor = a.Floor
            });
            return apartments;
        }

        public Task UpdateApartment(UpdateApartmentDto request, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}