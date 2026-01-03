using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Consts;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF.Context;
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
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;
        public ResidentService(IApartmentRepository apartmentRepository, IApartmentResidentsRepository apartmentResidentsRepository, IResidentRepository residentRepository,
         IUserService userService, IRoleService roleService, IUnitOfWork unitOfWork)
        {
            _apartmentRepository = apartmentRepository;
            _residentRepository = residentRepository;
            _userService = userService;
            _apartmentResidentsRepository = apartmentResidentsRepository;
            _roleService = roleService;
            _unitOfWork = unitOfWork;
        }
        public async Task CreateOrUpdateResident(ResidentDto request)
        {
            var apartment = _apartmentRepository.List().Include(a => a.ApartmentResidents).FirstOrDefault(a => a.Id.Equals(request.ApartmentId));
            if (apartment == null) throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorMessageConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);

            if (request.Id == null)
            {
                await CreateResident(apartment, request);
            }
            else
            {
                UpdateResident(apartment, request);
            }
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteResident(Guid apartmentId, List<string> ids)
        {
            var residentIds = ids.Select(i => new Guid(i));
            var residents = _residentRepository.List(r => residentIds.Contains(r.Id)).Include(r => r.ApartmentResidents).ToList();
            var apartmentResidents = new List<ApartmentResident>();
            foreach(var resident in residents)
            {
                if (resident.ApartmentResidents == null) continue;
                var apartmentResidentCurrents = resident.ApartmentResidents.Where(a => a.ApartmentId.Equals(apartmentId));
                apartmentResidents.AddRange(apartmentResidentCurrents);
            }

            var userIds = residents.Select(r => r.UserId);
            await _userService.DeleteUsers(userIds);
            _apartmentResidentsRepository.Delete(apartmentResidents);
            _residentRepository.Delete(residents);
            await _unitOfWork.CommitAsync();
        }

        public async Task<ResidentDto> GetResident(Guid residentId, Guid apartmentId)
        {
            var resident = _residentRepository.List().Include(a => a.ApartmentResidents).FirstOrDefault(a => a.Id.Equals(residentId));
            if (resident == null) throw new DomainException(ErrorCodeConsts.ResidentNotFound, ErrorMessageConsts.ResidentNotFound, System.Net.HttpStatusCode.NotFound);
            var residentDto = new ResidentDto()
            {
                Id = resident.Id,
                ApartmentBuildingId = resident.ApartmentBuildingId,
                BrithDay = resident.BrithDay,
                Name = resident.Name,
                PhoneNumber = resident.PhoneNumber,
                IdentityNumber = resident.IdentityNumber
            };
            if (resident.ApartmentResidents == null) return residentDto;
            var apartmentCurent = resident.ApartmentResidents.FirstOrDefault(a => a.ApartmentId.Equals(apartmentId));
            if (apartmentCurent == null) throw new DomainException(ErrorCodeConsts.ResidentNotExistInApartment, ErrorMessageConsts.ResidentNotExistInApartment, System.Net.HttpStatusCode.NotFound);
            residentDto.ApartmentId = apartmentCurent.ApartmentId;
            residentDto.MemberType = apartmentCurent.MemberType;
            if (string.IsNullOrEmpty(resident.UserId))
            {
                return residentDto;
            }
            var userRes = await _userService.GetUser(resident.UserId);
            if (userRes == null)
                throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            residentDto.UserName = userRes.UserName;
            residentDto.Email = userRes.Email;
            residentDto.UserId = userRes.UserId;
            return residentDto;
        }

        public Pagination<ResidentDto> GetResidents(RequestQueryBaseDto<Guid> request)
        {
            var apartment = _apartmentRepository.List().Include(a => a.ApartmentResidents).ThenInclude(a => a.Resident).FirstOrDefault(a => a.Id.Equals(request.Request));
            if (apartment == null) throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorMessageConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);
            var residentDtos = new List<ResidentDto>();
            if (apartment.ApartmentResidents == null) return new Pagination<ResidentDto>() { Items = residentDtos };
            foreach (var resident in apartment.ApartmentResidents)
            {
                residentDtos.Add(new ResidentDto()
                {
                    ApartmentBuildingId = apartment.ApartmentBuildingId,
                    ApartmentId = resident.ApartmentId,
                    Name = resident.Resident.Name,
                    Id = resident.ResidentId,
                    MemberType = resident.MemberType,
                    PhoneNumber = resident.Resident.PhoneNumber
                });
            }
            var residents = residentDtos.AsQueryable();
            if (request.Filters != null && request.Filters.Any())
            {
                residents = FilterHelper.ApplyFilters(residents, request.Filters);
            }
            if (request.Sorts != null && request.Sorts.Any())
            {
                residents = SortHelper.ApplySort(residents, request.Sorts);
            }
            return new Pagination<ResidentDto>()
            {
                Items = residents.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = residents.Count()
            };
        }
        private async Task CreateResident(Apartment apartment, ResidentDto request)
        {
            var residentNew = new Resident()
            {
                ApartmentBuildingId = apartment.ApartmentBuildingId,
                BrithDay = request.BrithDay != null ? DateTime.SpecifyKind(request.BrithDay.Value, DateTimeKind.Utc) : null,
                IdentityNumber = request.IdentityNumber,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber
            };
            var aparmentResidentNew = new ApartmentResident()
            {
                ApartmentId = apartment.Id,
                MemberType = MemberType.Member
            };
            var aparmentResident = apartment.ApartmentResidents;
            bool isExistOwner = false;
            if (aparmentResident != null)
            {
                if (aparmentResident.Any(a => a.MemberType.Equals(MemberType.Owner)))
                {
                    isExistOwner = true;
                }
            }
            if (request.MemberType.Equals(MemberType.Owner))
            {
                if (isExistOwner)
                    throw new DomainException(ErrorCodeConsts.ResidentOwnerAlreadyExist, ErrorMessageConsts.ResidentOwnerAlreadyExist, System.Net.HttpStatusCode.BadRequest);
                aparmentResidentNew.MemberType = MemberType.Owner;
                var roleResidentId = await _roleService.GetRoleIdByRoleName(RoleDefaulConsts.Resident);
                var userRes = await _userService.CreateOrUpdateUser(new CreateOrUpdateUserRequestDto()
                {
                    AppartmentBuildingId = apartment.ApartmentBuildingId.ToString(),
                    ApartmentId = apartment.Id.ToString(),
                    DisplayName = request.Name,
                    Email = request.Email,
                    Password = request.Password,
                    PhoneNumber = request.PhoneNumber,
                    UserName = request.UserName,
                    RoleId = roleResidentId
                });
                if (userRes == null)
                    throw new DomainException(ErrorCodeConsts.ErrorCreatingUser, ErrorMessageConsts.ErrorCreatingUser, System.Net.HttpStatusCode.BadRequest);
                residentNew.UserId = userRes.UserId;
            }
            var resident = await _residentRepository.Add(residentNew);
            aparmentResidentNew.ResidentId = resident.Id;
            await _apartmentResidentsRepository.Add(aparmentResidentNew);
        }
        private void UpdateResident(Apartment apartment, ResidentDto request)
        {
            var resident = _residentRepository.List().FirstOrDefault(r => r.Id.Equals(request.Id.Value));
            if (resident == null)
                throw new DomainException(ErrorCodeConsts.ResidentNotFound, ErrorMessageConsts.ResidentNotFound, System.Net.HttpStatusCode.NotFound);
            resident.BrithDay = request.BrithDay != null ? DateTime.SpecifyKind(request.BrithDay.Value, DateTimeKind.Utc) : null;
            resident.Name = request.Name;
            resident.IdentityNumber = request.IdentityNumber;
            resident.PhoneNumber = request.PhoneNumber;
            _residentRepository.Update(resident);
        }
    }
}
