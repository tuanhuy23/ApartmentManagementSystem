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

        public ApartmentBuildingService(IUnitOfWork unitOfWork, IApartmentBuildingRepository apartmentBuildingRepository)
        {
            _unitOfWork = unitOfWork;
            _apartmentBuildingRepository = apartmentBuildingRepository;
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
        public async Task CreateOrUpdateApartmentBuilding(CreateApartmentBuildingDto request)
        {
            var apartmentBuilding = new ApartmentBuilding() { 
                Id = Guid.NewGuid(),
                Address = request.Address,
                Code = request.Code,
                ContactEmail = request.ContactEmail,
                ApartmentBuildingImgUrl= request.ApartmentBuildingImgUrl,
                ContactPhone= request.ContactPhone,
                CurrencyUnit = request.CurrencyUnit,
                Description = request.Description,
                
            };
           
        }
        private IEnumerable<AppartmentBuildingImageDto> MapAppartmentBuildingImageDto(IEnumerable<AppartmentBuildingImage> imgs)
        {
            return new AppartmentBuildingImageDto()
            {
                Name = img.Name,
                Id = img.Id,
                Description = img.Description,
                Src = img.Src
            };
        }
    }
}
