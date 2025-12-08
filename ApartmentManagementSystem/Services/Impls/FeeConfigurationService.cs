using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Services.Impls
{
    internal class FeeConfigurationService : IFeeConfigurationService
    {
        private readonly IFeeTypeRepository _feeTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FeeConfigurationService(IFeeTypeRepository feeTypeRepository, IUnitOfWork unitOfWork)
        {
            _feeTypeRepository = feeTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateOrUpdateFeeType(CreateOrUpdateFeeTypeDto request)
        {
            FeeType feeType = new FeeType()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                CalculationType = request.CalculationType,
                IsActive = false,
            };
            if (request.Id != null)
            {
                feeType = _feeTypeRepository.List().Include(f => f.QuantityRateConfigs).Include(f => f.FeeRateConfigs).ThenInclude(f => f.FeeTiers).FirstOrDefault(f => f.Id.Equals(request.Id.Value));
                if (feeType == null)
                    throw new DomainException(ErrorCodeConsts.FeeTypeNotFound, ErrorCodeConsts.FeeTypeNotFound, System.Net.HttpStatusCode.NotFound);
            }

            if (CalculationType.TIERED.Equals(feeType.CalculationType) && request.FeeRateConfigs != null)
            {
                feeType.FeeRateConfigs = CreateOrUpdateFeeRateConfig(feeType, request.FeeRateConfigs).ToList();
            }
            if (CalculationType.QUANTITY.Equals(request.CalculationType) && request.QuantityRateConfigs != null)
            {
                feeType.QuantityRateConfigs = CreateOrUpdateQuantityRateConfig(feeType, request.QuantityRateConfigs).ToList();
            }
            if (CalculationType.Area.Equals(request.CalculationType))
            {
                feeType.DefaultRate = request.DefaultRate;
                feeType.ApplyDate = request.ApplyDate;
            }
            feeType.DefaultRate = request.DefaultRate;
            feeType.IsVATApplicable = request.IsVATApplicable;
            feeType.DefaultVATRate = request.DefaultVATRate;
            feeType.Name = request.Name;
            if (request.Id == null)
            {
                await _feeTypeRepository.Add(feeType);
            }
            else
            {
                feeType.IsActive = request.IsActive;
                _feeTypeRepository.Update(feeType);
            }
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteFeeType(IEnumerable<string> ids)
        {
            var feeConfigIds = ids.Select(i => new Guid(i));
            var feeConfigs = _feeTypeRepository.List(f => feeConfigIds.Contains(f.Id)).Include(f => f.FeeDetails).ToList();
            foreach(var feeConfig in feeConfigs)
            {
                if (feeConfig.FeeDetails.Any())
                    throw new DomainException(ErrorCodeConsts.FeeTypeIsApply, ErrorCodeConsts.FeeTypeIsApply, System.Net.HttpStatusCode.BadRequest);
                _feeTypeRepository.Delete(feeConfig);
            }
            await _unitOfWork.CommitAsync();
        }

        public async Task<FeeTypeDto> GetFeeType(Guid id)
        {
            var feeType = _feeTypeRepository.List().Include(f => f.QuantityRateConfigs).Include(f => f.FeeRateConfigs).ThenInclude(f =>f.FeeTiers).FirstOrDefault(f => f.Id.Equals(id));
            if (feeType == null)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotFound, ErrorCodeConsts.FeeTypeNotFound, System.Net.HttpStatusCode.NotFound);
            var feeTypeDto = new FeeTypeDto()
            {
                ApartmentBuildingId = feeType.ApartmentBuildingId,
                CalculationType = feeType.CalculationType,
                DefaultRate = feeType.DefaultRate,
                Id = feeType.Id,
                IsActive = feeType.IsActive,
                Name = feeType.Name,
                IsVATApplicable = feeType.IsVATApplicable,
                DefaultVATRate = feeType.DefaultVATRate
            };
            if (feeType.QuantityRateConfigs != null && feeType.CalculationType.Equals(CalculationType.QUANTITY))
            {
                feeTypeDto.QuantityRateConfigs = feeType.QuantityRateConfigs.Select(q => new QuantityRateConfigDto()
                {
                    Id = q.Id,
                    FeeTypeId = q.FeeTypeId,
                    ApartmentBuildingId = q.ApartmentBuildingId,
                    IsActive = q.IsActive,
                    ItemType = q.ItemType,
                    UnitRate = q.UnitRate,
                }).ToList();
            }
            else if (feeType.FeeRateConfigs != null && feeType.CalculationType.Equals(CalculationType.TIERED))
            {
                var feeRateConfigDtos = new List<FeeRateConfigDto>();
                foreach (var feeRateConfig in feeType.FeeRateConfigs)
                {
                    var feeRateConfigDto = new FeeRateConfigDto();
                    if (feeRateConfig.FeeTiers != null)
                    {
                        feeRateConfigDto.FeeTiers = feeRateConfig.FeeTiers.Select(f => new FeeTierDto()
                        {
                            ConsumptionEnd = f.ConsumptionEnd,
                            ConsumptionStart = f.ConsumptionStart,
                            FeeRateConfigId = f.FeeRateConfigId,
                            Id = f.Id,
                            TierOrder = f.TierOrder,
                            UnitName = f.UnitName,
                            UnitRate = f.UnitRate
                        }).ToList();
                    }
                    feeRateConfigDto.ApartmentBuildingId = feeRateConfig.ApartmentBuildingId;
                    feeRateConfigDto.Id = feeRateConfig.Id;
                    feeRateConfigDto.Name = feeRateConfig.Name;
                    feeRateConfigDto.FeeTypeId = feeRateConfig.FeeTypeId;
                    feeRateConfigDto.VATRate = feeRateConfig.VATRate;
                    feeRateConfigDto.Name = feeRateConfig.Name;
                    feeRateConfigDto.UnitName = feeRateConfig.UnitName;
                    feeRateConfigDto.IsActive = feeRateConfig.IsActive;
                    feeRateConfigDtos.Add(feeRateConfigDto);
                }
                feeTypeDto.FeeRateConfigs = feeRateConfigDtos;
            }
            return feeTypeDto;
        }

        public Pagination<FeeTypeDto> GetFeeTypes(RequestQueryBaseDto<string> request)
        {
            var feeTypes = _feeTypeRepository.List().Include(f => f.FeeRateConfigs).Where(f => f.ApartmentBuildingId.Equals(new Guid(request.Request)));
            var feeTypeDtos = new List<FeeTypeDto>();
            foreach (var feeType in feeTypes)
            {
                string feeRateConfigIdApplicable = string.Empty;
                if (feeType.FeeRateConfigs != null)
                {
                    var feeRateConfig = feeType.FeeRateConfigs.FirstOrDefault(f => f.IsActive);
                    if (feeRateConfig != null)
                    {
                        feeRateConfigIdApplicable = feeRateConfig.Id.ToString();
                    }
                }

                feeTypeDtos.Add(new FeeTypeDto()
                {
                    ApartmentBuildingId = feeType.ApartmentBuildingId,
                    CalculationType = feeType.CalculationType,
                    DefaultRate = feeType.DefaultRate,
                    Id = feeType.Id,
                    IsActive = feeType.IsActive,
                    IsVATApplicable = feeType.IsVATApplicable,
                    Name = feeType.Name,
                });
            }
            var response = feeTypeDtos.AsQueryable();
            if (request.Filters!= null && request.Filters.Any())
            {
                response = FilterHelper.ApplyFilters(response, request.Filters);
            }
            if (request.Sorts!= null && request.Sorts.Any())
            {
                response = SortHelper.ApplySort(response, request.Sorts);
            }
            return new Pagination<FeeTypeDto>()
            {
                Items = response.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = response.Count()
            };
        }

        private IEnumerable<FeeRateConfig> CreateOrUpdateFeeRateConfig(FeeType feeType, IEnumerable<CreateOrUpdateFeeRateConfigDto> request)
        {
            var feeRateConfigs = feeType.FeeRateConfigs;
            if (request == null) return new List<FeeRateConfig>();

            if (feeRateConfigs == null)
            {
                feeRateConfigs = new List<FeeRateConfig>();
            }
            bool setActived = false;
            foreach (var feeRateConfigDto in request)
            {
                bool isActive = false;
                if (feeRateConfigDto.IsActive && !setActived)
                {
                    setActived = true;
                    isActive = true;
                }
                if (feeRateConfigDto.Id == null)
                {
                    var inCommingFeeRateConfig = new FeeRateConfig()
                    {
                        ApartmentBuildingId = feeType.ApartmentBuildingId,
                        FeeTypeId = feeType.Id,
                        IsActive = isActive,
                        Name = feeRateConfigDto.Name,
                        VATRate = feeRateConfigDto.VATRate,
                        UnitName = "M3" //TODO
                    };
                    inCommingFeeRateConfig.FeeTiers = CreateOrUpdateFeeRateConfig(inCommingFeeRateConfig, feeRateConfigDto.FeeTiers).ToList();
                    feeRateConfigs.Add(inCommingFeeRateConfig);
                    continue;
                }
                var feeRateConfig = feeRateConfigs.FirstOrDefault(f => f.Id.Equals(feeRateConfigDto.Id.Value));
                if (feeRateConfig == null) continue;
                feeRateConfig.IsActive = isActive;
                feeRateConfig.FeeTiers = CreateOrUpdateFeeRateConfig(feeRateConfig, feeRateConfigDto.FeeTiers).ToList();
                feeRateConfig.Name = feeRateConfigDto.Name;
                feeRateConfig.VATRate = feeRateConfigDto.VATRate; 
            }
            return feeRateConfigs;
        }
        private IEnumerable<FeeTier> CreateOrUpdateFeeRateConfig(FeeRateConfig feeRateConfig, IEnumerable<CreateOrUpdateFeeRateTierDto> request)
        {
            if (request == null) return new List<FeeTier>();
            var feeTiers = feeRateConfig.FeeTiers;
            if (feeTiers == null)
            {
                feeTiers = new List<FeeTier>();
            }
            foreach (var feeTierDto in request)
            {
                if (feeTierDto.Id == null)
                {
                    feeTiers.Add(new FeeTier()
                    {
                        ConsumptionEnd = feeTierDto.ConsumptionEnd,
                        ConsumptionStart = feeTierDto.ConsumptionStart,
                        FeeRateConfigId = feeRateConfig.Id,
                        TierOrder = feeTierDto.TierOrder,
                        UnitName = feeTierDto.UnitName,
                        UnitRate = feeTierDto.UnitRate,
                    });
                    continue;
                }
                var feeTier = feeTiers.FirstOrDefault(f => f.Id.Equals(feeTierDto.Id.Value));
                if (feeTier == null) continue;
                feeTier.ConsumptionEnd = feeTierDto.ConsumptionEnd;
                feeTier.ConsumptionStart = feeTierDto.ConsumptionStart;
                feeTier.UnitName = feeTierDto.UnitName;
                feeTier.UnitRate = feeTierDto.UnitRate;
                feeTier.TierOrder = feeTierDto.TierOrder;
            }
            return feeTiers;
        }
        
        private IEnumerable<QuantityRateConfig> CreateOrUpdateQuantityRateConfig(FeeType feeType, IEnumerable<CreateOrUpdateQuantityRateConfigDto> request)
        {
            var quantityRateConfigs = feeType.QuantityRateConfigs;
            if (request == null) return new List<QuantityRateConfig>();

            if (quantityRateConfigs == null)
            {
                quantityRateConfigs = new List<QuantityRateConfig>();
            }
            bool setActived = false;
            foreach (var quantityRateConfigDto in request)
            {
                bool isActive = false;
                if (quantityRateConfigDto.IsActive && !setActived)
                {
                    setActived = true;
                    isActive = true;
                }
                if (quantityRateConfigDto.Id == null)
                {
                    var inCommingQuantityRateConfig = new QuantityRateConfig()
                    {
                        ApartmentBuildingId = feeType.ApartmentBuildingId,
                        FeeTypeId = feeType.Id,
                        IsActive = isActive,
                        ItemType = quantityRateConfigDto.ItemType,
                        UnitRate = quantityRateConfigDto.UnitRate
                    };
                    quantityRateConfigs.Add(inCommingQuantityRateConfig);
                    continue;
                }
                var quantityRateConfig = quantityRateConfigs.FirstOrDefault(f => f.Id.Equals(quantityRateConfigDto.Id.Value));
                if (quantityRateConfig == null) continue;
                quantityRateConfig.ItemType = quantityRateConfigDto.ItemType;
                quantityRateConfig.UnitRate = quantityRateConfigDto.UnitRate;
                quantityRateConfig.IsActive = isActive;
            }
            return quantityRateConfigs;
        }
    }
}