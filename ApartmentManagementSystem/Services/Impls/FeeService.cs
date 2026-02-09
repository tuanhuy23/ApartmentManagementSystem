using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Consts;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using NPOI.SS.Formula.Functions;
using static ApartmentManagementSystem.Common.ExcelUtilityHelper;

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
        private readonly HttpContext _httpContext = null;
        public FeeService(IUtilityReadingRepository utilityReadingRepository, IFeeNoticeRepository feeNoticeRepository, IBillingCycleSettingRepository billingCycleSettingRepository,
         IApartmentRepository apartmentRepository, IFeeTypeRepository feeTypeRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _utilityReadingRepository = utilityReadingRepository;
            _feeNoticeRepository = feeNoticeRepository;
            _billingCycleSettingRepository = billingCycleSettingRepository;
            _apartmentRepository = apartmentRepository;
            _feeTypeRepository = feeTypeRepository;
            _unitOfWork = unitOfWork;
            if (httpContextAccessor.HttpContext != null)
            {
                _httpContext = httpContextAccessor.HttpContext;
            }
        }
        public async Task CreateFeeNotice(IEnumerable<CreateOrUpdateFeeNoticeDto> requests)
        {
            var feeNotices = new List<FeeNotice>();
            foreach (var request in requests)
            {
                var feeNotice = new FeeNotice()
                {
                    ApartmentBuildingId = request.ApartmentBuildingId,
                    ApartmentId = request.ApartmentId,
                    BillingCycle = request.BillingCycle,
                    Status = FeeNoticeStatus.Issued,
                    PaymentStatus = FeeNoticeStatus.UnPaid,

                };
                await CreateOrUpdateFeeNotice(feeNotice, request);
                feeNotices.Add(feeNotice);
            }
            await _feeNoticeRepository.Add(feeNotices);
            await _unitOfWork.CommitAsync();
        }

        public async Task<FeeNoticeDto> GetFeeDetail(Guid id)
        {
            var feeNotice = _feeNoticeRepository.List().Include(f => f.FeeDetails).ThenInclude(fd => fd.FeeDetailTiers)
                                                       .Include(f => f.FeeDetails).ThenInclude(f => f.FeeType).FirstOrDefault(f => f.Id.Equals(id));
            if (feeNotice == null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeNotFound, ErrorMessageConsts.FeeNoticeNotFound, System.Net.HttpStatusCode.BadRequest);
 
            return MapToFeeNoticeDto(feeNotice);
        }

         public async Task<FeeNoticeDto> GetResidentFeeDetail(Guid id)
        {
            // Verify if user is resident of the apartment retricted to get only their apartment fee notices
            var accountInfo = IdentityHelper.GetIdentity(_httpContext);
            if (accountInfo == null) throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            

            var feeNotice = _feeNoticeRepository.List().Include(f => f.FeeDetails).ThenInclude(fd => fd.FeeDetailTiers)
                                                       .Include(f => f.FeeDetails).ThenInclude(f => f.FeeType).FirstOrDefault(f => f.Id.Equals(id));
            if (feeNotice == null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeNotFound, ErrorMessageConsts.FeeNoticeNotFound, System.Net.HttpStatusCode.BadRequest);
            if (accountInfo.RoleName.Equals(Consts.RoleDefaulConsts.Resident))
            {
                var apartment = _apartmentRepository.List().FirstOrDefault(a => a.ApartmentBuildingId.Equals(new Guid(accountInfo.ApartmentBuildingId)) && a.Id.Equals(feeNotice.ApartmentId));
                if (apartment == null)
                {
                    throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorMessageConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);
                }
            }
            return MapToFeeNoticeDto(feeNotice);
           
        }

        public Pagination<FeeNoticeDto> GetFeeNotices(RequestQueryBaseDto<Guid> request)
        {
            var feeNotice = _feeNoticeRepository.List().Where(f => f.ApartmentId.Equals(request.Request));
            if (feeNotice == null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeNotFound, ErrorMessageConsts.FeeNoticeNotFound, System.Net.HttpStatusCode.BadRequest);
            var feeNoticeDtos = feeNotice.Select(f => new FeeNoticeDto()
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
            if (request.Filters != null && request.Filters.Any())
            {
                feeNoticeDtos = FilterHelper.ApplyFilters(feeNoticeDtos, request.Filters);
            }
            if (request.Sorts != null && request.Sorts.Any())
            {
                feeNoticeDtos = SortHelper.ApplySort(feeNoticeDtos, request.Sorts);
            }
            return new Pagination<FeeNoticeDto>()
            {
                Items = feeNoticeDtos.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = feeNoticeDtos.Count()
            };
        }

        public Pagination<FeeNoticeDto> GetResidentFeeNotices(RequestQueryBaseDto<object> request)
        {
            // Verify if user is resident of the apartment retricted to get only their apartment fee notices
            var accountInfo = IdentityHelper.GetIdentity(_httpContext);
            if (accountInfo == null) throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            if (accountInfo.RoleName.Equals(Consts.RoleDefaulConsts.Resident))
            {
                var apartment = _apartmentRepository.List().FirstOrDefault(a => a.ApartmentBuildingId.Equals(new Guid(accountInfo.ApartmentBuildingId)));
                if (apartment == null)
                {
                    throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorMessageConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);
                }
            }
            return GetFeeNotices(new RequestQueryBaseDto<Guid>()
            {
                Filters = request.Filters,
                Page = request.Page,
                Sorts = request.Sorts,
                PageSize = request.PageSize,
                Request = new Guid(accountInfo.ApartmentId)
            });
        }

        public async Task CancelFeeNotice(Guid id)
        {
            var feeNotice = _feeNoticeRepository.List().Include(f => f.FeeDetails).ThenInclude(fd => fd.FeeDetailTiers).FirstOrDefault(f => f.Id.Equals(id));
            if (feeNotice == null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeNotFound, ErrorMessageConsts.FeeNoticeNotFound, System.Net.HttpStatusCode.BadRequest);
            if (feeNotice.PaymentStatus.Equals(Consts.FeeNoticeStatus.Paid))
                throw new DomainException(ErrorCodeConsts.FeeNoticeCannotBeModified, ErrorMessageConsts.FeeNoticeCannotBeModified, System.Net.HttpStatusCode.BadRequest);
            foreach (var feeDetail in feeNotice.FeeDetails)
            {
                if (feeDetail.FeeDetailTiers == null) continue;
                if (feeDetail.CurrentReadingDate != null)
                {
                    var currentUtilityReading = _utilityReadingRepository.List().FirstOrDefault(u => u.ReadingDate.Equals(feeDetail.CurrentReadingDate));
                    if (currentUtilityReading == null) continue;
                    _utilityReadingRepository.Delete(currentUtilityReading);
                }
            }
            feeNotice.Status = Consts.FeeNoticeStatus.Canceled;
            _feeNoticeRepository.Update(feeNotice);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdatePaymentStatusFeeNotice(Guid id)
        {
            var feeNotice = _feeNoticeRepository.List().Include(f => f.FeeDetails).ThenInclude(fd => fd.FeeDetailTiers).FirstOrDefault(f => f.Id.Equals(id));
            if (feeNotice == null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeNotFound, ErrorMessageConsts.FeeNoticeNotFound, System.Net.HttpStatusCode.BadRequest);
            if (feeNotice.PaymentStatus.Equals(Consts.FeeNoticeStatus.Canceled))
                throw new DomainException(ErrorCodeConsts.FeeNoticeCannotBeModified, ErrorMessageConsts.FeeNoticeCannotBeModified, System.Net.HttpStatusCode.BadRequest);
            feeNotice.PaymentStatus = Consts.FeeNoticeStatus.Paid;
            await _unitOfWork.CommitAsync();
        }

        public async Task DeletFeeNotice(List<string> ids)
        {
            var feeNoticeIds = ids.Select(i => new Guid(i));
            var feeNotices = _feeNoticeRepository.List(f => feeNoticeIds.Contains(f.Id));
            foreach (var feeNotice in feeNotices)
            {
                if (!feeNotice.Status.Equals(Consts.FeeNoticeStatus.Canceled))
                    throw new DomainException(ErrorCodeConsts.FeeNoticeCannotBeModified, ErrorMessageConsts.FeeNoticeCannotBeModified, System.Net.HttpStatusCode.BadRequest);
                _feeNoticeRepository.Delete(feeNotice);
            }
            await _unitOfWork.CommitAsync();
        }

        public Pagination<UtilityReadingDto> GetUtilityReadings(RequestQueryBaseDto<Guid> request)
        {
            var utilityReading = _utilityReadingRepository.List().Include(u => u.FeeType).Where(u => request.Request.Equals(u.ApartmentId));
            if (utilityReading == null)
                throw new DomainException(ErrorCodeConsts.UtilityReadingDataNotFound, ErrorMessageConsts.UtilityReadingDataNotFound, System.Net.HttpStatusCode.NotFound);
            var utilityReadingDtos = utilityReading.Select(u => new UtilityReadingDto()
            {
                ApartmentId = u.ApartmentId,
                CurrentReading = u.CurrentReading,
                FeeTypeId = u.FeeTypeId,
                FeeTypeName = u.FeeType.Name,
                ReadingDate = u.ReadingDate,
                Id = u.Id
            });
            if (request.Filters != null && request.Filters.Any())
            {
                utilityReadingDtos = FilterHelper.ApplyFilters(utilityReadingDtos, request.Filters);
            }
            if (request.Sorts != null && request.Sorts.Any())
            {
                utilityReadingDtos = SortHelper.ApplySort(utilityReadingDtos, request.Sorts);
            }
            return new Pagination<UtilityReadingDto>()
            {
                Items = utilityReadingDtos.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = utilityReadingDtos.Count()
            };
        }

        public byte[] DownloadExcelTemplate(string fileName, string sheetName, string apartmentId)
        {
            string jsonData = @"{
                            'header': [
                                {'ApartmentName': 'ApartmentName'},
                                {'BillingCycle': 'BillingCycle'},";
            var feeTypes = _feeTypeRepository.List(f => f.ApartmentBuildingId.Equals(new Guid(apartmentId)) && f.IsActive).Include(f => f.FeeRateConfigs).Include(f => f.QuantityRateConfigs);
            var now = DateTime.UtcNow;
            foreach (var feeType in feeTypes)
            {
                if ((feeType.CalculationType.Equals(CalculationType.Area) || feeType.CalculationType.Equals(CalculationType.QUANTITY)) && now > feeType.ApplyDate)
                {
                    jsonData += @"{'" + feeType.CalculationType + "_" + feeType.Name + "': '" + feeType.CalculationType + "_" + feeType.Name + "'},";
                    continue;
                }
                if (feeType.FeeRateConfigs == null) continue;
                var feeRateConfig = feeType.FeeRateConfigs.FirstOrDefault(f => f.IsActive && now > f.ApplyDate);
                if (feeRateConfig == null) continue;
                string dateReaing = "_DateReading";
                string utilityReading = "_UtilityReading";
                jsonData += @"{'" + CalculationType.TIERED + "_" + feeType.Name + "': '" + CalculationType.TIERED + "_" + feeType.Name + "'},";
                jsonData += @"{'" + CalculationType.TIERED + "_" + feeType.Name + "_" + feeRateConfig.Name + utilityReading + "': '" + CalculationType.TIERED + "_" + feeType.Name + "_" + feeRateConfig.Name + utilityReading + "'},";
                jsonData += @"{'" + CalculationType.TIERED + "_" + feeType.Name + "_" + feeRateConfig.Name + dateReaing + "':'" + CalculationType.TIERED + "_" + feeType.Name + "_" + feeRateConfig.Name + dateReaing + "'},";
            }
            jsonData += @"],
                        'body': []
                        }";
            return ExcelUtilityHelper.ExportToExcel(fileName, sheetName, jsonData);
        }
        public async Task<IEnumerable<ImportFeeNoticeResult>> ImportFeeNoticeResult(string apartmentBuildingId, IFormFile file)
        {
            ExcelData jsonData = ExcelUtilityHelper.ImportFromExcel(file);
            var result = new List<ImportFeeNoticeResult>();
            Dictionary<string, FeeNoticeExcelDto> feeNoticeExcelCols = new Dictionary<string, FeeNoticeExcelDto>();
            var now = DateTime.UtcNow;
            var feeTypes = _feeTypeRepository.List(f => f.IsActive && f.ApartmentBuildingId.Equals(new Guid(apartmentBuildingId))).Include(f => f.FeeRateConfigs);
            foreach (var feeType in feeTypes)
            {
                if (feeType.ApplyDate != null && now < feeType.ApplyDate) continue;
                if (!feeType.CalculationType.Equals(CalculationType.TIERED))
                {
                    feeNoticeExcelCols.Add($"{feeType.CalculationType}_{feeType.Name}", new FeeNoticeExcelDto()
                    {
                        FeeTypeId = feeType.Id
                    });
                    continue;
                }
                if (feeType.FeeRateConfigs == null) continue;
                var feeRateConfig = feeType.FeeRateConfigs.FirstOrDefault(f => f.IsActive && now > f.ApplyDate);
                if (feeRateConfig == null) continue;
                feeNoticeExcelCols.Add($"{feeType.CalculationType}_{feeType.Name}", new FeeNoticeExcelDto()
                {
                    FeeTypeId = feeType.Id,
                    FeeRateConfigId = feeRateConfig.Id,
                    DateTimeCol = $"{CalculationType.TIERED}_{feeType.Name}_{feeRateConfig.Name}_DateReading",
                    ReadingCol = $"{CalculationType.TIERED}_{feeType.Name}_{feeRateConfig.Name}_UtilityReading"
                });

            }
            var feeNotices = new List<FeeNotice>();
            foreach (var row in jsonData.body)
            {
                if (row == null) continue;

                var colApartmentName = row["ApartmentName"].ToString();
                if (colApartmentName.IsNullOrEmpty())
                {
                    result.Add(new Dtos.ImportFeeNoticeResult()
                    {
                        ErrorMessage = ErrorMessageConsts.ApartmentNotFound,
                    });
                    continue;
                }
                var apartment = _apartmentRepository.List(a => a.Name.Equals(colApartmentName) && a.ApartmentBuildingId.Equals(new Guid(apartmentBuildingId))).FirstOrDefault();
                if (apartment == null)
                {
                    result.Add(new Dtos.ImportFeeNoticeResult()
                    {
                        ErrorMessage = ErrorMessageConsts.ApartmentNotFound
                    });
                    continue;
                }
                var creatFeeNotice = new CreateOrUpdateFeeNoticeDto()
                {
                    ApartmentBuildingId = new Guid(apartmentBuildingId),
                    BillingCycle = row["BillingCycle"].ToString(),
                    ApartmentId = apartment.Id,
                };
                var feeTypeIds = new List<Guid>();
                var feeDetails = new List<CreateOrUpdateFeeDetailDto>();

                foreach (var feeType in feeNoticeExcelCols)
                {
                    var rowValue = row[feeType.Key];
                    if (rowValue == null) continue;
                    var rowValueStr = rowValue.ToString();
                    if (string.IsNullOrEmpty(rowValueStr)) continue;
                    if (feeType.Value == null) continue;
                    if (rowValueStr.ToString().ToLower().Equals("x"))
                    {
                        feeTypeIds.Add(feeType.Value.FeeTypeId);
                    }
                    if (string.IsNullOrEmpty(feeType.Value.DateTimeCol)) continue;
                    var rowDateTimeValue = row[feeType.Value.DateTimeCol].ToString();
                    var rowReadingValue = row[feeType.Value.ReadingCol].ToString();
                    if (string.IsNullOrEmpty(rowDateTimeValue))
                    {
                        result.Add(new Dtos.ImportFeeNoticeResult()
                        {
                            ApartmentName = apartment.Name,
                            ErrorMessage = ErrorMessageConsts.UtilityReadingDataIsRequired
                        });
                        continue;
                    }
                    if (string.IsNullOrEmpty(rowReadingValue))
                    {
                        result.Add(new Dtos.ImportFeeNoticeResult()
                        {
                            ApartmentName = apartment.Name,
                            ErrorMessage = ErrorMessageConsts.UtilityReadingDataIsRequired
                        });
                        continue;
                    }
                    DateTime resultDatetimeParse;
                    if (!DateTime.TryParseExact(rowDateTimeValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out resultDatetimeParse))
                    {
                        result.Add(new Dtos.ImportFeeNoticeResult()
                        {
                            ApartmentName = apartment.Name,
                            ErrorMessage = ErrorMessageConsts.DateTimeReadingIncorrectFormat
                        });
                        continue;
                    }
                    double resultReadingParse;
                    if (!double.TryParse(rowReadingValue, out resultReadingParse))
                    {
                        result.Add(new Dtos.ImportFeeNoticeResult()
                        {
                            ApartmentName = apartment.Name,
                            ErrorMessage = ErrorMessageConsts.ReadingIncorrectFormat
                        });
                        continue;
                    }
                    feeDetails.Add(new CreateOrUpdateFeeDetailDto()
                    {
                        ApartmentId = apartment.Id,
                        FeeTypeId = feeType.Value.FeeTypeId,
                        UtilityReading = new CreateUtilityReadingDto()
                        {
                            CurrentReading = resultReadingParse,
                            ReadingDate = resultDatetimeParse
                        }
                    });
                }
                creatFeeNotice.FeeTypeIds = feeTypeIds;
                creatFeeNotice.FeeDetails = feeDetails;
                try
                {
                    var feeNotice = new FeeNotice()
                    {
                        ApartmentBuildingId = creatFeeNotice.ApartmentBuildingId,
                        ApartmentId = creatFeeNotice.ApartmentId,
                        BillingCycle = creatFeeNotice.BillingCycle,
                        Status = FeeNoticeStatus.Issued,
                        PaymentStatus = FeeNoticeStatus.UnPaid,

                    };
                    await CreateOrUpdateFeeNotice(feeNotice, creatFeeNotice);
                    feeNotices.Add(feeNotice);
                }
                catch (DomainException ex)
                {
                    result.Add(new Dtos.ImportFeeNoticeResult()
                    {
                        ApartmentName = apartment.Name,
                        ErrorMessage = ex.Message
                    });
                }
            }
            await _feeNoticeRepository.Add(feeNotices);
            await _unitOfWork.CommitAsync();
            return result;
        }
        private FeeNoticeDto MapToFeeNoticeDto(FeeNotice feeNotice)
        {
             var response = new FeeNoticeDto()
            {
                ApartmentBuildingId = feeNotice.ApartmentBuildingId,
                Id = feeNotice.Id,
                ApartmentId = feeNotice.ApartmentId,
                BillingCycle = feeNotice.BillingCycle,
                DueDate = feeNotice.DueDate,
                IssueDate = feeNotice.CreatedDate,
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
        private async Task<FeeNotice> CreateOrUpdateFeeNotice(FeeNotice feeNotice, CreateOrUpdateFeeNoticeDto request)
        {
            var billingCycleReqExtract = ExtractBillingCyle(request.BillingCycle);

            if (billingCycleReqExtract == null)
                throw new DomainException(ErrorCodeConsts.BillingCycleInvalidFormat, ErrorMessageConsts.BillingCycleInvalidFormat, System.Net.HttpStatusCode.BadRequest);

            if (request.FeeDetails == null)
                throw new DomainException(ErrorCodeConsts.FeeDetailIsRequired, ErrorMessageConsts.FeeDetailIsRequired, System.Net.HttpStatusCode.BadRequest);

            var lastFeeNotice = _feeNoticeRepository.List().FirstOrDefault(f => f.ApartmentId.Equals(request.ApartmentId)
                                    && f.ApartmentBuildingId.Equals(request.ApartmentBuildingId) && request.BillingCycle.Equals(f.BillingCycle)
                                    && !FeeNoticeStatus.Canceled.Equals(f.Status) && !FeeNoticeStatus.UnPaid.Equals(f.PaymentStatus));
            if (lastFeeNotice != null)
                throw new DomainException(ErrorCodeConsts.FeeNoticeAlreadyExists, ErrorMessageConsts.FeeNoticeAlreadyExists, System.Net.HttpStatusCode.BadRequest);

            var billingSetting = _billingCycleSettingRepository.List().FirstOrDefault(b => b.ApartmentBuildingId.Equals(request.ApartmentBuildingId));

            if (billingSetting == null)
                throw new DomainException(ErrorCodeConsts.BillingCycleSettingIsNotFound, ErrorMessageConsts.BillingCycleSettingIsNotFound, System.Net.HttpStatusCode.BadRequest);
            var dayEndMonth = DateTime.DaysInMonth(billingCycleReqExtract.Year, billingCycleReqExtract.Month);

            if (billingSetting.ClosingDayOfMonth > dayEndMonth)
            {
                billingSetting.ClosingDayOfMonth = dayEndMonth;
            }

            var closingDate = new DateTime(billingCycleReqExtract.Year, billingCycleReqExtract.Month, billingSetting.ClosingDayOfMonth);

            if (closingDate > DateTime.UtcNow)
                throw new DomainException(ErrorCodeConsts.FeeNoticeNotDue, ErrorMessageConsts.FeeNoticeNotDue, System.Net.HttpStatusCode.BadRequest);

            var apartment = _apartmentRepository.List().Include(a => a.ParkingRegistrations).FirstOrDefault(a => a.ApartmentBuildingId.Equals(request.ApartmentBuildingId) && a.Id.Equals(request.ApartmentId));

            if (apartment == null) throw new DomainException(ErrorCodeConsts.ApartmentNotFound, ErrorMessageConsts.ApartmentNotFound, System.Net.HttpStatusCode.NotFound);
            var feeDetails = new List<FeeDetail>();

            foreach (var feeTypeId in request.FeeTypeIds)
            {
                var feeType = _feeTypeRepository.List().Include(f => f.QuantityRateConfigs).Include(f => f.FeeRateConfigs).ThenInclude(f => f.FeeTiers).FirstOrDefault(f => feeTypeId.Equals(f.Id) && f.IsActive);
                if (feeType == null) continue;
                if (feeType.CalculationType.Equals(CalculationType.Area))
                {
                    feeDetails.Add(CreateFeeDetailByFeeTypeArea(feeType, apartment));
                    continue;
                }
                else if (feeType.CalculationType.Equals(CalculationType.QUANTITY) && apartment.ParkingRegistrations != null)
                {
                    var feeDetail = CreateFeeDetailByFeeTypeQuantity(feeType, apartment.ParkingRegistrations);
                    feeDetails.Add(feeDetail);
                    continue;
                }
                if (request.FeeDetails == null) continue;
                var feeDetailReq = request.FeeDetails.FirstOrDefault(u => u.FeeTypeId.Equals(feeTypeId));
                if (feeDetailReq == null) continue;
                if (feeType.CalculationType.Equals(CalculationType.TIERED) && feeType.FeeRateConfigs != null && feeDetailReq.UtilityReading != null)
                {
                    var feeDetail = CreateFeeDetailByFeeTypeTier(feeType, feeDetailReq, closingDate, billingSetting.ClosingDayOfMonth);
                    feeDetails.Add(feeDetail);
                }
            }
            feeNotice.FeeDetails = feeDetails;
            feeNotice.TotalAmount = feeDetails.Sum(f => f.SubTotal);
            feeNotice.DueDate = DateTime.UtcNow.AddDays(billingSetting.PaymentDueDate);
            return feeNotice;
        }
        private FeeDetail CreateFeeDetailByFeeTypeArea(FeeType feeType, Apartment apartment)
        {
            if (DateTime.UtcNow < feeType.ApplyDate)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotFound, ErrorMessageConsts.FeeTypeNotFound, System.Net.HttpStatusCode.BadRequest);
            return new FeeDetail()
            {
                FeeTypeId = feeType.Id,
                SubTotal = (decimal)apartment.Area * feeType.DefaultRate
            };
        }
        private FeeDetail CreateFeeDetailByFeeTypeTier(FeeType feeType, CreateOrUpdateFeeDetailDto feeDetailReq, DateTime closingDate, int closingDayOfMonth = 30)
        {
            if (feeType.FeeRateConfigs == null)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotConfigured, ErrorMessageConsts.FeeTypeNotConfigured, System.Net.HttpStatusCode.BadRequest);

            var feeRateConfig = feeType.FeeRateConfigs.FirstOrDefault(f => f.IsActive);

            if (feeRateConfig == null || DateTime.UtcNow < feeRateConfig.ApplyDate)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotConfigured, ErrorMessageConsts.FeeTypeNotConfigured, System.Net.HttpStatusCode.BadRequest);

            var utilityReadingDto = feeDetailReq.UtilityReading;
            if (utilityReadingDto == null)
                throw new DomainException(ErrorCodeConsts.UtilityReadingDataIsRequired, ErrorMessageConsts.UtilityReadingDataIsRequired, System.Net.HttpStatusCode.BadRequest);

            var previousUtilityReading = _utilityReadingRepository.List().OrderByDescending(u => u.ReadingDate)
                                .FirstOrDefault(u => u.ApartmentBuildingId.Equals(feeRateConfig.ApartmentBuildingId) && u.ApartmentId.Equals(feeDetailReq.ApartmentId) && u.FeeTypeId.Equals(feeRateConfig.Id)
                                );
            double previousReading = 0;
            double actualUserTotalDays = closingDayOfMonth;
            if (previousUtilityReading != null)
            {
                if (utilityReadingDto.ReadingDate < previousUtilityReading.ReadingDate)
                    throw new DomainException(ErrorCodeConsts.CurrentUtilityReadingDateCannotBeEarlier, ErrorMessageConsts.CurrentUtilityReadingDateCannotBeEarlier, System.Net.HttpStatusCode.BadRequest);
                previousReading = previousUtilityReading.CurrentReading;
                actualUserTotalDays = (utilityReadingDto.ReadingDate - previousUtilityReading.ReadingDate).TotalDays;
            }
            else
            {
                if (utilityReadingDto.ReadingDate < closingDate) 
                    throw new DomainException(ErrorCodeConsts.CurrentUtilityReadingDateCannotBeEarlier, ErrorMessageConsts.CurrentUtilityReadingDateCannotBeEarlier, System.Net.HttpStatusCode.BadRequest);
                actualUserTotalDays = (utilityReadingDto.ReadingDate - closingDate).TotalDays;
            }
            double consumption = utilityReadingDto.CurrentReading - previousReading;
            double ratioChange = 1;
            if (actualUserTotalDays != closingDayOfMonth)
            {
                double rawRatio = (double)actualUserTotalDays / closingDayOfMonth;
                ratioChange = Math.Truncate(rawRatio * 100) / 100;
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
                    UnitName = feeRateConfig.UnitName,
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
                    currentUtilityReading.ReadingDate = DateTime.SpecifyKind(utilityReadingDto.ReadingDate, DateTimeKind.Utc);
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
                    ReadingDate = DateTime.SpecifyKind(utilityReadingDto.ReadingDate, DateTimeKind.Utc)
                });
            }
            return new FeeDetail()
            {
                Consumption = consumption,
                FeeTypeId = feeType.Id,
                PreviousReading = previousUtilityReading != null ? previousUtilityReading.CurrentReading : 0,
                PreviousReadingDate = previousUtilityReading != null ? DateTime.SpecifyKind(previousUtilityReading.ReadingDate, DateTimeKind.Utc) : null,
                SubTotal = subTotal,
                CurrentReading = utilityReadingDto.CurrentReading,
                CurrentReadingDate = DateTime.SpecifyKind(utilityReadingDto.ReadingDate, DateTimeKind.Utc),
                Proration = ratioChange,
                FeeDetailTiers = feeDetailTiers,
                NetCost = netCost,
                VATCost = vatCost
            };
        }
        private FeeDetail CreateFeeDetailByFeeTypeQuantity(FeeType feeType, IEnumerable<ParkingRegistration> parkings)
        {
            if (feeType.QuantityRateConfigs == null || DateTime.UtcNow < feeType.ApplyDate)
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
            DateTime parsedDate;
            if (DateTime.TryParseExact(billingCycle, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return new BillingCycleExtract()
                {
                    Month = parsedDate.Month,
                    Year = parsedDate.Year
                };
            }
            else if (DateTime.TryParseExact(billingCycle, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
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
    internal class FeeNoticeExcelDto
    {
        public string DateTimeCol { get; set; }
        public string ReadingCol { get; set; }
        public Guid FeeTypeId { get; set; }
        public Guid FeeRateConfigId { get; set; }
    }
}