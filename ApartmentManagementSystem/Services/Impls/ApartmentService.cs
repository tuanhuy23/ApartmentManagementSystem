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
            if(_apartmentRepository.List(a => a.Name.Equals(request.Name) && a.ApartmentBuildingId.Equals(request.ApartmentBuildingId)).Any())
                throw new DomainException(ErrorCodeConsts.ApartmentNameIsDuplicate, ErrorMessageConsts.ApartmentNameIsDuplicate, System.Net.HttpStatusCode.BadRequest);
            if (request.Area <= 0)
                throw new DomainException(ErrorCodeConsts.ApartmentAreaGreaterThanZero, ErrorMessageConsts.ApartmentAreaGreaterThanZero, System.Net.HttpStatusCode.BadRequest);
            var apartment = new Apartment()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                Area = request.Area,
                Floor = request.Floor,
                Name = request.Name
            };
            await _apartmentRepository.Add(apartment);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteApartment(Guid id)
        {
            var apartments = _apartmentRepository.List(a => a.Id.Equals(id)).FirstOrDefault();
            if (apartments == null) 
                throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorMessageConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);
            _apartmentRepository.Delete(apartments);
            await _unitOfWork.CommitAsync();
        }

        public async Task<ApartmentDto> GetApartment(Guid id)
        {
            var apartments = _apartmentRepository.List(a => a.Id.Equals(id)).FirstOrDefault();
            if (apartments == null) 
                throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorMessageConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);
            return new ApartmentDto()
            {
                Id = apartments.Id,
                ApartmentBuildingId = apartments.ApartmentBuildingId,
                Name = apartments.Name,
                Area = apartments.Area,
                Floor = apartments.Floor
            };
        }

        public Pagination<ApartmentDto> GetApartments(RequestQueryBaseDto<string> request)
        {
            var apartments = _apartmentRepository.List(a => a.ApartmentBuildingId.Equals(new Guid(request.Request))).Select(a => new ApartmentDto()
            {
                Id = a.Id,
                ApartmentBuildingId = a.ApartmentBuildingId,
                Name = a.Name,
                Area = a.Area,
                Floor = a.Floor
            });
            if (request.Filters!= null && request.Filters.Any())
            {
                apartments = FilterHelper.ApplyFilters(apartments, request.Filters);
            }
            if (request.Sorts!= null && request.Sorts.Any())
            {
                apartments = SortHelper.ApplySort(apartments, request.Sorts);
            }
            return new Pagination<ApartmentDto>()
            {
                Items = apartments.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = apartments.Count()
            };
        }

        public async Task UpdateApartment(UpdateApartmentDto request)
        {
            var apartment = _apartmentRepository.List(a => a.Id.Equals(request.Id)).FirstOrDefault();
            if (apartment == null) 
                throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorMessageConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);
            apartment.Area = request.Area;
            apartment.Floor = request.Floor;

            if (_apartmentRepository.List(a => a.Name.Equals(request.Name)).Any())
                throw new DomainException(ErrorCodeConsts.ApartmentNameIsDuplicate, ErrorMessageConsts.ApartmentNameIsDuplicate, System.Net.HttpStatusCode.BadRequest);
            apartment.Name = request.Name;
            _apartmentRepository.Update(apartment);
            await _unitOfWork.CommitAsync();
        }
    }
}