using ApartmentManagementSystem.Consts.Permissions;
using ApartmentManagementSystem.Dtos;
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

        public IEnumerable<ApartmentBuildingDto> GetApartmentBuildings()
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
            }).ToList();
            return response;
        }
        public async Task CreateApartmentBuilding(CreateApartmentBuildingDto request)
        {
            var apartmentBuildingId = Guid.NewGuid();
            var roleManagementId = await _roleService.GetRoleIdByRoleName(RoleDefaulConsts.Management);

            var ownerUser = await _userService.CreateOrUpdateUser(new CreateOrUpdateUserRequestDto()
            {
                AppartmentBuildingId = apartmentBuildingId.ToString(),
                DisplayName = request.ManagementDisplayName,
                Email = request.ManagementEmail,
                Password = request.ManagementPassword,
                PhoneNumber = request.ManagementPhoneNumber,
                UserName = request.ManagementUserName,
                RoleId = roleManagementId,
            });

            var apartmentBuilding = new ApartmentBuilding()
            {
                Id = apartmentBuildingId,
                Address = request.Address,
                Code = request.Code,
                ContactEmail = request.ContactEmail,
                ApartmentBuildingImgUrl = request.ApartmentBuildingImgUrl,
                ContactPhone = request.ContactPhone,
                CurrencyUnit = request.CurrencyUnit,
                Description = request.Description,
                Name = request.Name,
                Status = StatusConsts.Active,
                OwnerUserId = ownerUser.UserId,
            };
            if (request.Images != null)
            {
                apartmentBuilding.Images = MapAppartmentBuildingImageEntity(request.Images).ToList();
            }
            await _apartmentBuildingRepository.Add(apartmentBuilding);
            await _unitOfWork.CommitAsync();

        }
        private IEnumerable<AppartmentBuildingImageDto> MapAppartmentBuildingImageDto(IEnumerable<AppartmentBuildingImage> imgs)
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
        private IEnumerable<AppartmentBuildingImage> MapAppartmentBuildingImageEntity(IEnumerable<AppartmentBuildingImageDto> imgs)
        {
            var entities = new List<AppartmentBuildingImage>();
            foreach (var imgsItem in imgs)
            {
                entities.Add(new AppartmentBuildingImage()
                {
                    Description = imgsItem.Description,
                    Name = imgsItem.Name,
                    Src = imgsItem.Src,
                    Id = Guid.NewGuid(),
                });
            }
            return entities;
        }
    }
}
