using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Consts;
using ApartmentManagementSystem.Consts.Permissions;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Services.Interfaces;

namespace ApartmentManagementSystem.Services.Impls
{
    class ApartmentBuildingService : IApartmentBuildingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApartmentBuildingRepository _apartmentBuildingRepository;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public ApartmentBuildingService(IUnitOfWork unitOfWork, IApartmentBuildingRepository apartmentBuildingRepository, IUserService userService, IRoleService roleService)
        {
            _unitOfWork = unitOfWork;
            _apartmentBuildingRepository = apartmentBuildingRepository;
            _userService = userService;
            _roleService = roleService;
        }

        public Pagination<ApartmentBuildingDto> GetApartmentBuildings(RequestQueryBaseDto<object> request)
        {
            var response = _apartmentBuildingRepository.List().Select(x => new ApartmentBuildingDto()
            {
                Address = x.Address,
                ApartmentBuildingImgUrl = x.ApartmentBuildingImgUrl,
                Code = x.Code,
                ContactEmail = x.ContactEmail,
                ContactPhone = x.ContactPhone,
                CurrencyUnit = x.CurrencyUnit,
                Description = x.Description,
                Id = x.Id.ToString(),
                Name = x.Name,
                Status = x.Status
            });
            if (request.Filters!= null && request.Filters.Any())
            {
                response = FilterHelper.ApplyFilters(response, request.Filters);
            }
            if (request.Sorts!= null && request.Sorts.Any())
            {
                response = SortHelper.ApplySort(response, request.Sorts);
            }
            return new Pagination<ApartmentBuildingDto>()
            {
                Items = response.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = response.Count()
            };
        }
        public async Task CreateApartmentBuilding(CreateApartmentBuildingDto request)
        {
            var apartmentBuildingRequest = new ApartmentBuilding()
            {
                Address = request.Address,
                Code = request.Code,
                ContactEmail = request.ContactEmail,
                ApartmentBuildingImgUrl = request.ApartmentBuildingImgUrl,
                ContactPhone = request.ContactPhone,
                CurrencyUnit = request.CurrencyUnit,
                Description = request.Description,
                Name = request.Name,
                Status = StatusConsts.Active,
                OwnerUserName = request.ManagementUserName,
                Buildings = new List<string>()
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
                    FileType = FileType.Media
                });
            }
            return entities;
        }
    }
}
