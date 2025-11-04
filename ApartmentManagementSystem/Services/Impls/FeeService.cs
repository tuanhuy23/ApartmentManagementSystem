using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Services.Impls
{
    internal class FeeService : IFeeService
    {
        private readonly IUtilityReadingRepository _utilityReadingRepository;
        private readonly IFeeNoticeRepository _feeNoticeRepository;
        private readonly IBillingCycleSettingRepository _billingCycleSettingRepository;
        private readonly IFeeTypeRepository _feeTypeRepository;
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FeeService(IUtilityReadingRepository utilityReadingRepository, IFeeNoticeRepository feeNoticeRepository, IBillingCycleSettingRepository billingCycleSettingRepository,
         IApartmentRepository apartmentRepository, IFeeTypeRepository feeTypeRepository, IUnitOfWork unitOfWork)
        {
            _utilityReadingRepository = utilityReadingRepository;
            _feeNoticeRepository = feeNoticeRepository;
            _billingCycleSettingRepository = billingCycleSettingRepository;
            _apartmentRepository = apartmentRepository;
            _feeTypeRepository = feeTypeRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task CreateFeeNotice(CreateFeeNoticeDto request)
        {
            var billingCycleExtract = ExtractBillingCyle(request.BillingCycle);

            if (billingCycleExtract == null)
                throw new DomainException(ErrorCodeConsts.BillingCycleInvalidFormat, ErrorCodeConsts.BillingCycleInvalidFormat, System.Net.HttpStatusCode.BadRequest);

            if (request.UtilityReadings == null)
                throw new DomainException(ErrorCodeConsts.FeeTypeIsRequired, ErrorCodeConsts.FeeTypeIsRequired, System.Net.HttpStatusCode.BadRequest);

            var feeTypeIds = request.FeeTypeIds

            var lastFeeNotice = _feeNoticeRepository.List().Include(f => f.FeeDetails).OrderByDescending(f => f.BillingCycle).FirstOrDefault(f => f.ApartmentId.Equals(request.ApartmentId) && f.ApartmentBuildingId.Equals(request.ApartmentBuildingId) && f.);

            var feeNotice = new FeeNotice()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                ApartmentId = request.ApartmentId,
                BillingCycle = request.BillingCycle
            };
            var apartment = _apartmentRepository.List().Include(a => a.ParkingRegistrations).FirstOrDefault(a => a.ApartmentBuildingId.Equals(request.ApartmentBuildingId) && a.Id.Equals(request.ApartmentId));
            if (apartment == null) throw new DomainException(ErrorCodeConsts.ApartmentNotFoud, ErrorCodeConsts.ApartmentNotFoud, System.Net.HttpStatusCode.NotFound);
            var feeDetails = new List<FeeDetail>();
            foreach (var feeTypeId in request.FeeTypeIds)
            {
                var feeType = _feeTypeRepository.List().Include(f => f.FeeRateConfigs).ThenInclude(f => f.FeeTiers).FirstOrDefault(f => feeTypeId.Equals(f.Id) && f.IsActive);
                if (feeType == null) continue;

                if (feeType.CalculationType.Equals(CalculationType.Area))
                {
                    feeDetails.Add(new FeeDetail()
                    {
                        FeeTypeId = feeType.Id,
                        SubTotal = (decimal)apartment.Area * feeType.DefaultRate
                    });
                    continue;
                }
                else if (feeType.CalculationType.Equals(CalculationType.QUANTITY) && apartment.ParkingRegistrations != null)
                {
                    feeDetails.Add(new FeeDetail()
                    {
                        FeeTypeId = feeType.Id,
                        SubTotal = (decimal)apartment.ParkingRegistrations.Count * feeType.DefaultRate
                    });
                    continue;
                }
                if (request.UtilityReadings == null) continue;
                var utilityReadingDto = request.UtilityReadings.FirstOrDefault(u => u.FeeTypeId.Equals(feeTypeId));
                if (utilityReadingDto == null) continue;

                if (feeType.CalculationType.Equals(CalculationType.TIERED) && feeType.FeeRateConfigs != null)
                {
                    var feeDetail = await CreateFeeDetailByFeeTypeTier(feeType, utilityReadingDto);
                    feeDetails.Add(feeDetail);
                }
            }
            var billingSetting = _billingCycleSettingRepository.List().FirstOrDefault(b => b.ApartmentBuildingId.Equals(request.ApartmentBuildingId));
            if (billingSetting == null)
                throw new DomainException(ErrorCodeConsts.BillingCycleSettingIsNotFound, ErrorCodeConsts.BillingCycleSettingIsNotFound, System.Net.HttpStatusCode.BadRequest);
            feeNotice.DueDate = DateTime.UtcNow.AddDays(billingSetting.PaymentDueDate);
            feeNotice.FeeDetails = feeDetails;
            await _feeNoticeRepository.Add(feeNotice);
            await _unitOfWork.CommitAsync();
        }

        public Task<FeeNoticeDto> GetFeeDtail(Guid id)
        {
            throw new NotImplementedException();
        }

        private async Task<FeeDetail> CreateFeeDetailByFeeTypeTier(FeeType feeType, UtilityReadingDto utilityReadingDto)
        {
            if (feeType.FeeRateConfigs == null)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotConfig, ErrorCodeConsts.FeeTypeNotConfig, System.Net.HttpStatusCode.BadRequest);

            var feeRateConfig = feeType.FeeRateConfigs.FirstOrDefault(f => f.IsActive);

            if (feeRateConfig == null)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotConfig, ErrorCodeConsts.FeeTypeNotConfig, System.Net.HttpStatusCode.BadRequest);

           var previousUtilityReading = _utilityReadingRepository.List().OrderByDescending(u => u.ReadingDate)
                                .FirstOrDefault(u => u.ApartmentBuildingId.Equals(feeRateConfig.ApartmentBuildingId) && u.ApartmentId.Equals(utilityReadingDto.ApartmentId) && u.FeeTypeId.Equals(feeRateConfig.Id));
            double previousReading = 0;
            double actualUserTotalDays = 30;
            if (previousUtilityReading != null)
            {
                if (utilityReadingDto.ReadingDate < previousUtilityReading.ReadingDate)
                    throw new DomainException(ErrorCodeConsts.CurentUtilityReadingDateNotEarlierPrevious, ErrorCodeConsts.CurentUtilityReadingDateNotEarlierPrevious, System.Net.HttpStatusCode.BadRequest);
                previousReading = previousUtilityReading.CurrentReading;
                actualUserTotalDays = (utilityReadingDto.ReadingDate - previousUtilityReading.ReadingDate).TotalDays;
            }
            double consumption = utilityReadingDto.CurrentReading - previousReading;
            double ratioChange = 1;
            if (actualUserTotalDays != 30)
            {
                ratioChange = Math.Truncate(actualUserTotalDays / 30 * 100) / 100;
            }
            double remainCons = consumption;
            decimal cost = 0;
            foreach (var feeTier in feeRateConfig.FeeTiers.OrderBy(f => f.TierOrder))
            {
                if (remainCons <= 0) break;
                double adjustedTierLimit = (feeTier.ConsumptionEnd - feeTier.ConsumptionStart) * ratioChange;
                remainCons = remainCons - adjustedTierLimit;
                if (remainCons < 0)
                {
                    cost = cost + ((decimal)remainCons * feeTier.UnitRate);
                }
                else
                {
                    cost = cost + ((decimal)adjustedTierLimit * feeTier.UnitRate);
                }
            }
            decimal subTotal = cost;
            if (feeType.IsActive && feeRateConfig.VATRate != 0)
            {
                subTotal = cost * (decimal)feeRateConfig.VATRate;
            }
            return new FeeDetail()
            {
                Consumption = consumption,
                FeeTypeId = feeType.Id,
                PreviousReading = previousUtilityReading != null ? previousUtilityReading.CurrentReading : 0,
                PreviousReadingDate = previousUtilityReading != null ? previousUtilityReading.ReadingDate : null,
                SubTotal = subTotal,
                CurrentReading = utilityReadingDto.CurrentReading,
                CurrentReadingDate = utilityReadingDto.ReadingDate
            };

           
        }
        private BillingCycleExtract ExtractBillingCyle(string billingCycle)
        {
            string format = "yyyy-MM";
            if (DateTime.TryParseExact(billingCycle, format,CultureInfo.InvariantCulture,DateTimeStyles.None, out DateTime parsedDate))
            {
                return new BillingCycleExtract()
                {
                    Month = parsedDate.Month,
                    Year = parsedDate.Year
                };
            }
            return null;
        }
    }
    internal class BillingCycleExtract
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }
}