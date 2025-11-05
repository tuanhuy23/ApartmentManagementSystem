using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IFeeService
    {
        Task CreateFeeNotice(CreateFeeNoticeDto request);
        Task<FeeNoticeDto> GetFeeDtail(Guid id);
        Task UpdateFeeNotice(CreateFeeNoticeDto id);
    }
}