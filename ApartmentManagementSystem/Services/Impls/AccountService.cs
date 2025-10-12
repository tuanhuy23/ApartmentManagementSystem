using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Services.Interfaces;

namespace ApartmentManagementSystem.Services.Impls
{
    class AccountService : IAccountService
    {
        private readonly HttpContext _httpContext = null;
        public AccountService(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                _httpContext = httpContextAccessor.HttpContext;
            }
        }
        public async Task<AccountInfoResponseDto> GetAccountInfo()
        {
            var accountInfo = IdentityHelper.GetIdentity(_httpContext);
            return accountInfo;
        }
    }
}