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
using Azure.Core;
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
        public async Task CreateFeeNotice(CreateOrUpdateFeeNoticeDto request)
        {
            var billingCycleReqExtract = ExtractBillingCyle(request.BillingCycle);

            if (billingCycleReqExtract == null)
                throw new DomainException(ErrorCodeConsts.BillingCycleInvalidFormat, ErrorCodeConsts.BillingCycleInvalidFormat, System.Net.HttpStatusCode.BadRequest);

            if (request.FeeDetails == null)
                throw new DomainException(ErrorCodeConsts.FeeDetailIsRequired, ErrorCodeConsts.FeeDetailIsRequired, System.Net.HttpStatusCode.BadRequest);

            var lastFeeNotice = _feeNoticeRepository.List().FirstOrDefault(f => f.ApartmentId.Equals(request.ApartmentId)
                                    && f.ApartmentBuildingId.Equals(request.ApartmentBuildingId) && request.BillingCycle.Equals(f.BillingCycle) 
                                    && !FeeNoticeStatus.Canceled.Equals(f.Status) && !FeeNoticeStatus.UnPaid.Equals(f.PaymentStatus));
            if (lastFeeNotice != null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeAlreadyExists, ErrorCodeConsts.FeeNoticeAlreadyExists, System.Net.HttpStatusCode.BadRequest);

            var billingSetting = _billingCycleSettingRepository.List().FirstOrDefault(b => b.ApartmentBuildingId.Equals(request.ApartmentBuildingId));

            if (billingSetting == null)
                throw new DomainException(ErrorCodeConsts.BillingCycleSettingIsNotFound, ErrorCodeConsts.BillingCycleSettingIsNotFound, System.Net.HttpStatusCode.BadRequest);
            var closingDate = new DateTime(billingCycleReqExtract.Year, billingCycleReqExtract.Month, billingSetting.ClosingDayOfMonth);
            
            if(closingDate > DateTime.UtcNow)
                throw new DomainException(ErrorCodeConsts.FeeNoticeNotDue, ErrorCodeConsts.FeeNoticeNotDue, System.Net.HttpStatusCode.BadRequest);

            var feeNotice = new FeeNotice()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                ApartmentId = request.ApartmentId,
                BillingCycle = request.BillingCycle,
                Status = FeeNoticeStatus.Draft,
                PaymentStatus = FeeNoticeStatus.NA
            };
            feeNotice = await CreateOrUpdateFeeNotice(feeNotice, request);          
            feeNotice.DueDate = DateTime.UtcNow.AddDays(billingSetting.PaymentDueDate);
            await _feeNoticeRepository.Add(feeNotice);
            await _unitOfWork.CommitAsync();
        }

        public async Task<FeeNoticeDto> GetFeeDetail(Guid id)
        {
            var feeNotice = _feeNoticeRepository.List().Include(f => f.FeeDetails).ThenInclude(fd => fd.FeeDetailTiers)
                                                       .Include(f => f.FeeDetails).ThenInclude(f => f.FeeType).FirstOrDefault(f => f.Id.Equals(id));
            if (feeNotice == null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeNotFound, ErrorCodeConsts.FeeNoticeNotFound, System.Net.HttpStatusCode.BadRequest);
            var response = new FeeNoticeDto()
            {
                ApartmentBuildingId = feeNotice.ApartmentBuildingId,
                Id = feeNotice.Id,
                ApartmentId = feeNotice.ApartmentId,
                BillingCycle = feeNotice.BillingCycle,
                DueDate = feeNotice.DueDate,
                IssueDate = feeNotice.IssueDate,
                PaymentStatus = feeNotice.PaymentStatus,
                Status = feeNotice.Status,
                TotalAmount = feeNotice.TotalAmount,
            };
            if (feeNotice.FeeDetails == null) return response;
            var feeDetailDtos = new List<FeeDetailDto>();
            foreach (var feeDetail in feeNotice.FeeDetails)
            {
                if (feeDetail.FeeType == null) continue;
                string currentUtilityReadingId = string.Empty;
                if (feeDetail.CurrentReadingDate != null)
                {
                    var currentUtilityReading = _utilityReadingRepository.List().FirstOrDefault(u => u.ReadingDate.Equals(feeDetail.CurrentReadingDate));
                    if (currentUtilityReading == null) continue;
                    currentUtilityReadingId = currentUtilityReading.Id.ToString();
                }

                var feeDetailDto = new FeeDetailDto()
                {
                    FeeNoticeId = feeDetail.FeeNoticeId,
                    FeeTypeId = feeDetail.FeeTypeId,
                    SubTotal = feeDetail.SubTotal,
                    Consumption = feeDetail.Consumption,
                    CurrentReadingDate = feeDetail.CurrentReadingDate,
                    CurrentReading = feeDetail.CurrentReading,
                    PreviousReading = feeDetail.PreviousReading,
                    PreviousReadingDate = feeDetail.PreviousReadingDate,
                    UtilityCurentReadingId = string.IsNullOrEmpty(currentUtilityReadingId) ? null : new Guid(currentUtilityReadingId),
                    Proration = feeDetail.Proration,
                    GrossCost = feeDetail.NetCost,
                    VATRate = feeDetail.VATRate,
                    VATCost = feeDetail.VATCost,
                };
                if (feeDetail.FeeType.CalculationType.Equals(CalculationType.TIERED) && feeDetail.FeeDetailTiers != null)
                {
                    feeDetailDto.FeeTierDetails = feeDetail.FeeDetailTiers.Select(f => new FeeTierDetail()
                    {
                        Consumption = f.Consumption,
                        ConsumptionEnd = f.ConsumptionEnd,
                        ConsumptionStart = f.ConsumptionStart,
                        TierOrder = f.TierOrder,
                        UnitName = f.UnitName,
                        UnitRate = f.UnitRate,
                        ConsumptionEndOriginal = f.ConsumptionEndOriginal,
                        ConsumptionStartOriginal = f.ConsumptionStartOriginal,
                    });
                }
                feeDetailDtos.Add(feeDetailDto);
            }
            response.FeeDetails = feeDetailDtos;
            return response;
        }

        public async Task UpdateFeeNotice(CreateOrUpdateFeeNoticeDto request)
        {
            if (request.Id == null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeIdIsRequired, ErrorCodeConsts.FeeNoticeIdIsRequired, System.Net.HttpStatusCode.BadRequest);

            var feeNotice = _feeNoticeRepository.List().FirstOrDefault(f => f.Id.Equals(request.Id));

            if (feeNotice == null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeNotFound, ErrorCodeConsts.FeeNoticeNotFound, System.Net.HttpStatusCode.BadRequest);

            if (!feeNotice.Status.Equals(FeeNoticeStatus.Draft) || !feeNotice.Status.Equals(FeeNoticeStatus.Canceled))
                throw new DomainException(ErrorCodeConsts.FeeNoticeCannotBeModified, ErrorCodeConsts.FeeNoticeCannotBeModified, System.Net.HttpStatusCode.BadRequest);
            feeNotice = await CreateOrUpdateFeeNotice(feeNotice, request);
            _feeNoticeRepository.Update(feeNotice);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<FeeNoticeDto>> GetFeeNotices(Guid apartmentId)
        {
            var feeNotice = _feeNoticeRepository.List().Where(f => f.ApartmentId.Equals(apartmentId));
            if (feeNotice == null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeNotFound, ErrorCodeConsts.FeeNoticeNotFound, System.Net.HttpStatusCode.BadRequest);
            return feeNotice.Select(f => new FeeNoticeDto()
            {
                ApartmentBuildingId = f.ApartmentBuildingId,
                ApartmentId = f.ApartmentId,
                BillingCycle = f.BillingCycle,
                DueDate = f.DueDate,
                Id = f.Id,
                Status = f.Status,
                PaymentStatus = f.PaymentStatus,
                IssueDate = f.IssueDate,
                TotalAmount = f.TotalAmount
            });
        }
        
        public async Task<IEnumerable<UtilityReadingDto>> GetUtilityReadings(Guid apartmentId)
        {
            var utilityReading = _utilityReadingRepository.List().Include(u => u.FeeType).Where(u => apartmentId.Equals(u.ApartmentId));
            if (utilityReading == null)
                throw new DomainException(ErrorCodeConsts.UtilityReadingDataNotFound, ErrorCodeConsts.UtilityReadingDataNotFound, System.Net.HttpStatusCode.NotFound);
            return utilityReading.Select(u => new UtilityReadingDto()
            {
                ApartmentId = u.ApartmentId,
                CurrentReading = u.CurrentReading,
                FeeTypeId = u.FeeTypeId,
                FeeTypeName = u.FeeType.Name,
                Id = u.Id
            }).ToList();
        }

        private async Task<FeeNotice> CreateOrUpdateFeeNotice(FeeNotice feeNotice, CreateOrUpdateFeeNoticeDto request)
        {
            var apartment = _apartmentRepository.List().Include(a => a.ParkingRegistrations).FirstOrDefault(a => a.ApartmentBuildingId.Equals(request.ApartmentBuildingId) && a.Id.Equals(request.ApartmentId));
            if (apartment == null) throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorCodeConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);
            var feeDetails = new List<FeeDetail>();
            foreach (var feeTypeId in request.FeeTypeIds)
            {
                var feeType = _feeTypeRepository.List().Include(f => f.QuantityRateConfigs).Include(f => f.FeeRateConfigs).ThenInclude(f => f.FeeTiers).FirstOrDefault(f => feeTypeId.Equals(f.Id) && f.IsActive);
                if (feeType == null) continue;
                if (request.FeeDetails == null) continue;
                var feeDetailReq = request.FeeDetails.FirstOrDefault(u => u.FeeTypeId.Equals(feeTypeId));
                if (feeDetailReq == null) continue;
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
                    var feeDetail = CreateFeeDetailByFeeTypeQuantity(feeType, feeDetailReq, apartment.ParkingRegistrations);
                    feeDetails.Add(feeDetail);
                    continue;
                }
                if (feeType.CalculationType.Equals(CalculationType.TIERED) && feeType.FeeRateConfigs != null && feeDetailReq.UtilityReading != null)
                {
                    var feeDetail = CreateFeeDetailByFeeTypeTier(feeType, feeDetailReq);
                    feeDetails.Add(feeDetail);
                }
            }
            feeNotice.FeeDetails = feeDetails;
            feeNotice.TotalAmount = feeDetails.Sum(f => f.SubTotal);
            return feeNotice;
        }

        private FeeDetail CreateFeeDetailByFeeTypeTier(FeeType feeType, CreateOrUpdateFeeDetailDto feeDetailReq)
        {
            if (feeType.FeeRateConfigs == null)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotConfigured, ErrorCodeConsts.FeeTypeNotConfigured, System.Net.HttpStatusCode.BadRequest);

            var feeRateConfig = feeType.FeeRateConfigs.FirstOrDefault(f => f.IsActive);

            if (feeRateConfig == null)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotConfigured, ErrorCodeConsts.FeeTypeNotConfigured, System.Net.HttpStatusCode.BadRequest);

            var utilityReadingDto = feeDetailReq.UtilityReading;
            if (utilityReadingDto == null)
                throw new DomainException(ErrorCodeConsts.UtilityReadingDataIsRequired, ErrorCodeConsts.UtilityReadingDataIsRequired, System.Net.HttpStatusCode.BadRequest);

            var previousUtilityReading = _utilityReadingRepository.List().OrderByDescending(u => u.ReadingDate)
                                .FirstOrDefault(u => u.ApartmentBuildingId.Equals(feeRateConfig.ApartmentBuildingId) && u.ApartmentId.Equals(feeDetailReq.ApartmentId) && u.FeeTypeId.Equals(feeRateConfig.Id) 
                                && (utilityReadingDto. UtilityCurentReadingId == null ? true : !u.Id.Equals(utilityReadingDto. UtilityCurentReadingId.Value)));
            double previousReading = 0;
            double actualUserTotalDays = 30;
            if (previousUtilityReading != null)
            {
                if (utilityReadingDto.ReadingDate < previousUtilityReading.ReadingDate)
                    throw new DomainException(ErrorCodeConsts.CurrentUtilityReadingDateCannotBeEarlier, ErrorCodeConsts.CurrentUtilityReadingDateCannotBeEarlier, System.Net.HttpStatusCode.BadRequest);
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
            var feeDetailTiers = new List<FeeDetailTier>();
            foreach (var feeTier in feeRateConfig.FeeTiers.OrderBy(f => f.TierOrder))
            {
                if (remainCons <= 0) break;
                double adjustedTierLimit = (feeTier.ConsumptionEnd - feeTier.ConsumptionStart) * ratioChange;
                var consumptionTier = remainCons - adjustedTierLimit;
                if (consumptionTier < 0)
                {
                    cost = cost + ((decimal)remainCons * feeTier.UnitRate);
                }
                else
                {
                    cost = cost + ((decimal)adjustedTierLimit * feeTier.UnitRate);
                }
                remainCons = consumptionTier;
                feeDetailTiers.Add(new FeeDetailTier()
                {
                    TierOrder = feeTier.TierOrder,
                    UnitName = feeTier.UnitName,
                    UnitRate = feeTier.UnitRate,
                    Consumption = adjustedTierLimit,
                    ConsumptionEnd = feeTier.ConsumptionEnd * ratioChange,
                    ConsumptionStart = feeTier.ConsumptionStart * ratioChange,
                    ConsumptionStartOriginal = feeTier.ConsumptionStart,
                    ConsumptionEndOriginal = feeTier.ConsumptionEnd,
                });
            }
            decimal subTotal = cost;
            decimal netCost = cost;
            decimal vatCost = 0;
            if (feeType.IsActive && feeRateConfig.VATRate != 0)
            {
                vatCost = cost * (decimal)feeRateConfig.VATRate;
                subTotal = subTotal + vatCost;
            }
            if (utilityReadingDto.UtilityCurentReadingId != null)
            {
                var currentUtilityReading = _utilityReadingRepository.List().FirstOrDefault(u => u.Id.Equals(utilityReadingDto.UtilityCurentReadingId.Value));
                if (currentUtilityReading != null)
                {
                    currentUtilityReading.CurrentReading = utilityReadingDto.CurrentReading;
                    currentUtilityReading.ReadingDate = utilityReadingDto.ReadingDate;
                    _utilityReadingRepository.Update(currentUtilityReading);
                }
            }
            else
            {
                _utilityReadingRepository.Add(new UtilityReading()
                {
                    ApartmentBuildingId = feeType.ApartmentBuildingId,
                    ApartmentId = feeDetailReq.ApartmentId,
                    FeeTypeId = feeDetailReq.FeeTypeId,
                    CurrentReading = utilityReadingDto.CurrentReading,
                    ReadingDate = utilityReadingDto.ReadingDate
                });
            }
            return new FeeDetail()
            {
                Consumption = consumption,
                FeeTypeId = feeType.Id,
                PreviousReading = previousUtilityReading != null ? previousUtilityReading.CurrentReading : 0,
                PreviousReadingDate = previousUtilityReading != null ? previousUtilityReading.ReadingDate : null,
                SubTotal = subTotal,
                CurrentReading = utilityReadingDto.CurrentReading,
                CurrentReadingDate = utilityReadingDto.ReadingDate,
                Proration = ratioChange,
                FeeDetailTiers = feeDetailTiers,
                NetCost = netCost,
                VATCost = vatCost
            };
        }
        private FeeDetail CreateFeeDetailByFeeTypeQuantity(FeeType feeType, CreateOrUpdateFeeDetailDto feeDetailReq, IEnumerable<ParkingRegistration> parkings)
        {
            if(feeType.QuantityRateConfigs == null)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotConfigured, ErrorCodeConsts.FeeTypeNotConfigured, System.Net.HttpStatusCode.BadRequest);
            var quantityRateConfigActives = feeType.QuantityRateConfigs.Where(f => f.IsActive);
            var feeDetail = new FeeDetail()
            {
                FeeTypeId = feeType.Id,
            };
            decimal subTotal = 0;
            foreach (var quantityRateConfig in quantityRateConfigActives)
            {
                var countItemType = parkings.Where(p => p.VehicleType.Equals(quantityRateConfig.ItemType)).Count();
                if (countItemType <= 0) continue;
                decimal costByType = countItemType * quantityRateConfig.UnitRate;
                subTotal += costByType;
            }
            if (feeType.IsVATApplicable && feeType.DefaultVATRate != 0)
            {
                subTotal = subTotal * (decimal)feeType.DefaultVATRate;
            }
            feeDetail.SubTotal = subTotal;
            return feeDetail;
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