using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;

namespace ApartmentManagementSystem.Services.Impls
{
    class AccountService : IAccountService
    {
        private readonly HttpContext _httpContext = null;
        private readonly IApartmentBuildingRepository _apartmentBuildingRepository; 
        public AccountService(IHttpContextAccessor httpContextAccessor, IApartmentBuildingRepository apartmentBuildingRepository)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                _httpContext = httpContextAccessor.HttpContext;
            }
            _apartmentBuildingRepository = apartmentBuildingRepository;
        }
        public async Task<AccountInfoResponseDto> GetAccountInfo()
        {
            var accountInfo = IdentityHelper.GetIdentity(_httpContext);
            return accountInfo;
        }

        public async Task<ApartmentBuildingDto> GetCurrentApartment()
        {
            var accountInfo = IdentityHelper.GetIdentity(_httpContext);
            if (accountInfo.ApartmentBuildingId.Equals("root"))
            {
                return new ApartmentBuildingDto()
                {
                    Id = "Root",
                    Name = "Root"
                };
            }
            var apartmentBuilding = _apartmentBuildingRepository.List().FirstOrDefault(a => a.Id.Equals(new Guid(accountInfo.ApartmentBuildingId)));
            if (apartmentBuilding == null)
                throw new DomainException(ErrorCodeConsts.ApartmentBuildingNotFound, ErrorMessageConsts.ApartmentBuildingNotFound, System.Net.HttpStatusCode.NotFound);
            return new ApartmentBuildingDto()
            {
                Id = apartmentBuilding.Id.ToString(),
                Name = apartmentBuilding.Name
            };
        }
    }
}