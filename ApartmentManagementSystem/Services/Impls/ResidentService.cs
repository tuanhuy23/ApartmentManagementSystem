using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Services.Impls
{
    class ResidentService : IResidentService
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IApartmentResidentsRepository _apartmentResidentsRepository;
        private readonly IResidentRepository _residentRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        public ResidentService (IApartmentRepository apartmentRepository, IApartmentResidentsRepository apartmentResidentsRepository, IResidentRepository residentRepository, IUserService userService)
        {
            _apartmentRepository = apartmentRepository;
            _residentRepository = residentRepository;
            _userService = userService;
            _apartmentResidentsRepository = apartmentResidentsRepository;
        }
        public async Task CreateOrUpdateResident(ResidentDto request)
        {
            var apartmentId = _apartmentRepository.List().Include(a => a.ApartmentResidents).FirstOrDefault(a => a.Id.Equals(request.ApartmentId));
            if (apartmentId == null) throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorMessageConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);

        }

        public Task DeleteResident(Guid residentId)
        {
            throw new NotImplementedException();
        }

        public ResidentDto GetResident(Guid residentId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ResidentDto> GetResidents(Guid apartmentId)
        {
            throw new NotImplementedException();
        }
    }
}
