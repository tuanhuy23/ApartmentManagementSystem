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
    class ApartmentBuildingService : IApartmentBuildingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApartmentBuildingRepository _apartmentBuildingRepository;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ApartmentBuildingData _apartmentBuildingData;

        public ApartmentBuildingService(IUnitOfWork unitOfWork, IApartmentBuildingRepository apartmentBuildingRepository, IUserService userService, IRoleService roleService,
         ApartmentBuildingData apartmentBuildingData)
        {
            _unitOfWork = unitOfWork;
            _apartmentBuildingRepository = apartmentBuildingRepository;
            _userService = userService;
            _roleService = roleService;
            _apartmentBuildingData = apartmentBuildingData;
        }

        public Pagination<ApartmentBuildingDto> GetApartmentBuildings(RequestQueryBaseDto<object> request)
        {
            var response = _apartmentBuildingRepository.List(a => !a.IsDeleted).Select(x => new ApartmentBuildingDto()
            {
                Address = x.Address,
                ApartmentBuildingImgUrl = x.ApartmentBuildingImgUrl,
                ContactEmail = x.ContactEmail,
                ContactPhone = x.ContactPhone,
                CurrencyUnit = x.CurrencyUnit,
                Description = x.Description,
                Id = x.Id.ToString(),
                Name = x.Name,
                Status = x.Status
            });
            if (request.Filters != null && request.Filters.Any())
            {
                response = FilterHelper.ApplyFilters(response, request.Filters);
            }
            if (request.Sorts != null && request.Sorts.Any())
            {
                response = SortHelper.ApplySort(response, request.Sorts);
            }
            return new Pagination<ApartmentBuildingDto>()
            {
                Items = response.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = response.Count()
            };
        }
        public async Task CreateOrUpdateApartmentBuilding(CreateOrUpdateApartmentBuildingDto request)
        {
            if (request.Id == null)
            {
                await CreateApartmentBuilding(request);
            }
            else
            {
                await UpdateApartmentBuilding(request);
            }
            await _unitOfWork.CommitAsync();

        }
        public async Task<ApartmentBuildingDto> GetApartmentBuilding(Guid id)
        {
            var apartmentBuilding = _apartmentBuildingRepository.List(a => a.Id.Equals(id)).Include(a => a.Files).FirstOrDefault();
            if (apartmentBuilding == null)
                throw new DomainException(ErrorCodeConsts.ApartmentBuildingNotFound, ErrorMessageConsts.ApartmentBuildingNotFound, System.Net.HttpStatusCode.NotFound);

            return new ApartmentBuildingDto()
            {
                Address = apartmentBuilding.Address,
                ApartmentBuildingImgUrl = apartmentBuilding.ApartmentBuildingImgUrl,
                ContactEmail = apartmentBuilding.ContactEmail,
                ContactPhone = apartmentBuilding.ContactPhone,
                CurrencyUnit = apartmentBuilding.CurrencyUnit,
                Description = apartmentBuilding.Description,
                Id = apartmentBuilding.Id.ToString(),
                Name = apartmentBuilding.Name,
                Images = MapAppartmentBuildingImageDto(apartmentBuilding.Files)
            };
        }

        public async Task DeleteApartmentBuilding(List<string> ids)
        {
            if (ids == null || !ids.Any())
                throw new DomainException(ErrorCodeConsts.ApartmentBuildingNotFound, ErrorMessageConsts.ApartmentBuildingNotFound, System.Net.HttpStatusCode.NotFound);
            var apartmentBuidlingIds = ids.Select(i => new Guid(i));
            var apartmentBuildings = _apartmentBuildingRepository.List(a => apartmentBuidlingIds.Contains(a.Id) && !a.IsDeleted);
            foreach(var apartmentBuilding in apartmentBuildings)
            {
                apartmentBuilding.IsDeleted = true;
            }
            _apartmentBuildingRepository.Update(apartmentBuildings);
            await _unitOfWork.CommitAsync();
            _apartmentBuildingData.RemoveApartmentBuilding(ids);
        }

        public async Task UpdateApartmentBuildingStatus(Guid id, UpdateStatusApartmentBuildingDto request)
        {
            var apartmentBuilding = _apartmentBuildingRepository.List(a => a.Id.Equals(id)).FirstOrDefault();
            if (apartmentBuilding == null)
                throw new DomainException(ErrorCodeConsts.ApartmentBuildingNotFound, ErrorMessageConsts.ApartmentBuildingNotFound, System.Net.HttpStatusCode.NotFound);
            var validStatuses = new List<string> { StatusConsts.Active, StatusConsts.InActive };
            if (!validStatuses.Contains(request.Status))
                throw new DomainException(ErrorCodeConsts.ApartmentBuildingStatusInvalidFormat, ErrorMessageConsts.ApartmentBuildingStatusInvalidFormat, System.Net.HttpStatusCode.BadRequest);
            apartmentBuilding.Status = request.Status;
            _apartmentBuildingRepository.Update(apartmentBuilding);
            await _unitOfWork.CommitAsync();
        }
        private IEnumerable<AppartmentBuildingImageDto> MapAppartmentBuildingImageDto(IEnumerable<FileAttachment> imgs)
        {
            var dtos = new List<AppartmentBuildingImageDto>();
            foreach (var imgsItem in imgs)
            {
                dtos.Add(new AppartmentBuildingImageDto()
                {
                    Description = imgsItem.Description,
                    Id = imgsItem.Id.ToString(),
                    Name = imgsItem.Name,
                    Src = imgsItem.Src,
                });
            }
            return dtos;
        }
        private IEnumerable<FileAttachment> MapAppartmentBuildingImageEntity(IEnumerable<UploadAppartmentBuildingImageDto> imgs)
        {
            var entities = new List<FileAttachment>();
            foreach (var imgsItem in imgs)
            {
                entities.Add(new FileAttachment()
                {
                    Description = imgsItem.Description,
                    Name = imgsItem.Name,
                    Src = imgsItem.Src,
                    FileType = Consts.FileType.Media
                });
            }
            return entities;
        }

        private async Task CreateApartmentBuilding(CreateOrUpdateApartmentBuildingDto request)
        {
            var apartmentBuildingRequest = new ApartmentBuilding()
            {
                Address = request.Address,
                ContactEmail = request.ContactEmail,
                ApartmentBuildingImgUrl = request.ApartmentBuildingImgUrl,
                ContactPhone = request.ContactPhone,
                CurrencyUnit = request.CurrencyUnit,
                Description = request.Description,
                Name = request.Name,
                Status = StatusConsts.Active,
                OwnerUserName = request.ManagementUserName,
            };
            if (request.Images != null)
            {
                apartmentBuildingRequest.Files = MapAppartmentBuildingImageEntity(request.Images).ToList();
            }
            var apartmentBuilding = await _apartmentBuildingRepository.Add(apartmentBuildingRequest);
            var roleManagementId = await _roleService.GetRoleIdByRoleName(RoleDefaulConsts.Management);

            await _userService.CreateOrUpdateUser(new CreateOrUpdateUserRequestDto()
            {
                AppartmentBuildingId = apartmentBuilding.Id.ToString(),
                DisplayName = request.ManagementDisplayName,
                Email = request.ManagementEmail,
                Password = request.ManagementPassword,
                PhoneNumber = request.ManagementPhoneNumber,
                UserName = request.ManagementUserName,
                RoleId = roleManagementId,
            });
            _apartmentBuildingData.AddApartmentBuilding(new ApartmentBuildingDto()
            {
                Id = apartmentBuilding.Id.ToString(),
                Name = apartmentBuilding.Name
            });
        }

        private async Task UpdateApartmentBuilding(CreateOrUpdateApartmentBuildingDto request)
        {
            var apartmentBuilding = _apartmentBuildingRepository.List(a => a.Id.Equals(request.Id)).Include(a => a.Files).FirstOrDefault();
            if (apartmentBuilding == null)
                throw new DomainException(ErrorCodeConsts.ApartmentBuildingNotFound, ErrorMessageConsts.ApartmentBuildingNotFound, System.Net.HttpStatusCode.NotFound);
            apartmentBuilding.Address = request.Address;
            apartmentBuilding.Name = request.Name;
            apartmentBuilding.CurrencyUnit = request.CurrencyUnit;
            apartmentBuilding.Description = request.Description;
            apartmentBuilding.ContactEmail = request.ContactEmail;
            apartmentBuilding.ContactPhone = request.ContactPhone;
            apartmentBuilding.ApartmentBuildingImgUrl = request.ApartmentBuildingImgUrl;
            var files = apartmentBuilding.Files;

            if (request.Images == null)
            {
                request.Images = new List<UploadAppartmentBuildingImageDto>();
            }
            foreach(var file in request.Images)
            {
                if(file.Id == null)
                {
                    files.Add(new FileAttachment()
                    {
                        Src = file.Src,
                        Name = file.Name,
                        Description = file.Description,
                        FileType = Consts.FileType.Media
                    });
                    continue;
                }
                var fileEntity = files.FirstOrDefault(f => f.Id.Equals(file.Id.Value));
                if (fileEntity == null) continue;
                fileEntity.Src = file.Src;
                fileEntity.Name = file.Name;
                fileEntity.Description = file.Description;
            } 
            _apartmentBuildingRepository.Update(apartmentBuilding);
            _apartmentBuildingData.AddApartmentBuilding(new ApartmentBuildingDto()
            {
                Id = apartmentBuilding.Id.ToString(),
                Name = apartmentBuilding.Name
            });
        }
    }
}
