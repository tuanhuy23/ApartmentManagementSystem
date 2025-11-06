using ApartmentManagementSystem.Dtos;
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
                DefaultRate = request.DefaultRate,
                IsActive = false,
                IsVATApplicable = request.IsVATApplicable,
                Name = request.Name,     
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
            }
            if (request.Id == null)
            {
                await _feeTypeRepository.Add(feeType);
            }
            else
            {
                _feeTypeRepository.Update(feeType);
            }
            await _unitOfWork.CommitAsync();
        }
        
        public async Task<FeeTypeDto> GetFeeType(Guid id)
        {
            var feeType = _feeTypeRepository.List().FirstOrDefault(f => f.Id.Equals(id));
            if (feeType == null)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotFound, ErrorCodeConsts.FeeTypeNotFound, System.Net.HttpStatusCode.NotFound);
            string feeRateConfigIdApplicable = string.Empty;
            if (feeType.FeeRateConfigs != null)
            {
                var feeRateConfig = feeType.FeeRateConfigs.FirstOrDefault(f => f.IsActive);
                if (feeRateConfig != null)
                {
                    feeRateConfigIdApplicable = feeRateConfig.Id.ToString();
                }            
            }
            return new FeeTypeDto()
            {
                ApartmentBuildingId = feeType.ApartmentBuildingId,
                CalculationType = feeType.CalculationType,
                DefaultRate = feeType.DefaultRate,
                Id = feeType.Id,
                IsActive = feeType.IsActive,
                Name = feeType.Name,
                IsVATApplicable = feeType.IsVATApplicable
            };
        }

        public async Task<IEnumerable<FeeTypeDto>> GetFeeTypes(string apartmentBuildingId)
        {
            var feeTypes = _feeTypeRepository.List().Include(f => f.FeeRateConfigs).Where(f => f.ApartmentBuilding.Equals(new Guid(apartmentBuildingId)));
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
            return feeTypeDtos;
        }

        private IEnumerable<FeeRateConfig> CreateOrUpdateFeeRateConfig(FeeType feeType, IEnumerable<CreateOrUpdateFeeRateConfigDto> request)
        {
            var feeRateConfigs = feeType.FeeRateConfigs;
            if (request == null) return new List<FeeRateConfig>();

            if (feeRateConfigs == null)
            {
                feeRateConfigs = new List<FeeRateConfig>();
            }

            foreach (var feeRateConfigDto in request)
            {
                if (feeRateConfigDto.Id == null)
                {
                    var inCommingFeeRateConfig = new FeeRateConfig()
                    {
                        ApartmentBuildingId = feeType.ApartmentBuildingId,
                        FeeTypeId = feeType.Id,
                        IsActive = false,
                        Name = feeRateConfigDto.Name,
                        VATRate = feeRateConfigDto.VATRate
                    };
                    inCommingFeeRateConfig.FeeTiers = CreateOrUpdateFeeRateConfig(inCommingFeeRateConfig, feeRateConfigDto.FeeTiers).ToList();
                    feeRateConfigs.Add(inCommingFeeRateConfig);
                    continue;
                }
                var feeRateConfig = feeRateConfigs.FirstOrDefault(f => f.Id.Equals(feeRateConfigDto.Id.Value));
                if (feeRateConfig == null) continue;
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

            foreach (var quantityRateConfigDto in request)
            {
                if (quantityRateConfigDto.Id == null)
                {
                    var inCommingQuantityRateConfig = new QuantityRateConfig()
                    {
                        ApartmentBuildingId = feeType.ApartmentBuildingId,
                        FeeTypeId = feeType.Id,
                        IsActive = false,
                        ItemType = quantityRateConfigDto.ItemType,
                        VATRate = quantityRateConfigDto.VATRate,
                        UnitRate = quantityRateConfigDto.UnitRate
                    };
                    quantityRateConfigs.Add(inCommingQuantityRateConfig);
                    continue;
                }
                var quantityRateConfig = quantityRateConfigs.FirstOrDefault(f => f.Id.Equals(quantityRateConfigDto.Id.Value));
                if (quantityRateConfig == null) continue;
                quantityRateConfig.ItemType = quantityRateConfigDto.ItemType;
                quantityRateConfig.VATRate = quantityRateConfigDto.VATRate;
                quantityRateConfig.UnitRate = quantityRateConfigDto.UnitRate;
            }
            return quantityRateConfigs;
        }
    }
}