using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountInfoResponseDto> GetAccountInfo();
    }
}