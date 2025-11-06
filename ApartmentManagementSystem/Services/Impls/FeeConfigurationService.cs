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
        private readonly IFeeRateConfigRepository _feeRateConfigRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FeeConfigurationService(IFeeTypeRepository feeTypeRepository, IFeeRateConfigRepository feeRateConfigRepository, IUnitOfWork unitOfWork)
        {
            _feeRateConfigRepository = feeRateConfigRepository;
            _feeTypeRepository = feeTypeRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task CreateFeeRateConfig(CreateFeeRateConfigDto request, Guid feeTypeId)
        {
            var feeType = _feeTypeRepository.List().FirstOrDefault(f => f.Id.Equals(feeTypeId));
            if (feeType == null)
                throw new DomainException(ErrorCodeConsts.FeeTypeNotFound, ErrorCodeConsts.FeeTypeNotFound, System.Net.HttpStatusCode.NotFound);
            var feeRateConfig = new FeeRateConfig()
            {
                ApartmentBuildingId = feeType.ApartmentBuildingId,
                FeeTypeId = feeType.Id,
                IsActive = false,
                Name = request.Name,
                VATRate = request.VATRate
            };
            if (request.FeeTiers != null)
            {
                feeRateConfig.FeeTiers = request.FeeTiers.Select(ft => new FeeTier()
                {
                    ConsumptionEnd = ft.ConsumptionEnd,
                    ConsumptionStart = ft.ConsumptionStart,
                    TierOrder = ft.TierOrder,
                    UnitName = ft.UnitName,
                    UnitRate = ft.UnitRate
                }).ToList();
            }
            await _feeRateConfigRepository.Add(feeRateConfig);
            await _unitOfWork.CommitAsync();
        }

        public async Task CreateFeeType(CreateFeeTypeDto request)
        {
            var feeType = new FeeType()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                CalculationType = request.CalculationType,
                DefaultRate = request.DefaultRate,
                IsActive = false,
                IsVATApplicable = request.IsVATApplicable,
                Name = request.Name,
            };
            if (CalculationType.TIERED.Equals(request.CalculationType) && request.FeeRateConfigs != null)
            {
                var feeRateConfigs = new List<FeeRateConfig>();
                foreach (var reqFeeRateconfig in request.FeeRateConfigs)
                {
                    if (reqFeeRateconfig.FeeTiers == null)
                        throw new DomainException(ErrorCodeConsts.FeeTierIsRequired, ErrorCodeConsts.FeeTierIsRequired, System.Net.HttpStatusCode.BadRequest);
                    feeRateConfigs.Add(new FeeRateConfig()
                    {
                        ApartmentBuildingId = request.ApartmentBuildingId,
                        Name = reqFeeRateconfig.Name,
                        VATRate = reqFeeRateconfig.VATRate,
                        IsActive = false,
                        FeeTiers = reqFeeRateconfig.FeeTiers.Select(t => new FeeTier()
                        {
                            ConsumptionEnd = t.ConsumptionEnd,
                            ConsumptionStart = t.ConsumptionStart,
                            TierOrder = t.TierOrder,
                            UnitName = t.UnitName,
                            UnitRate = t.UnitRate
                        }).ToList()
                    });
                }
                feeType.FeeRateConfigs = feeRateConfigs;
            }
            if (CalculationType.QUANTITY.Equals(request.CalculationType) && request.QuantityRateConfigs != null)
            {
                var quantityRateConfigs = new List<QuantityRateConfig>();
                foreach (var reqQuantityRateconfig in request.QuantityRateConfigs)
                {
                    quantityRateConfigs.Add(new QuantityRateConfig()
                    {
                        ApartmentBuildingId = request.ApartmentBuildingId,
                        ItemType = reqQuantityRateconfig.ItemType,
                        VATRate = reqQuantityRateconfig.VATRate,
                        IsActive = false,
                        UnitRate = reqQuantityRateconfig.UnitRate,
                    });
                }
                feeType.QuantityRateConfigs = quantityRateConfigs;
            }
            await _feeTypeRepository.Add(feeType);
            await _unitOfWork.CommitAsync();
        }

        public Task DeleteFeeRateConfig(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FeeRateConfigDto>> GetFeeRateConfigs(Guid feeTypeId)
        {
            var feeRateConfigs = _feeRateConfigRepository.List().Include(fc => fc.FeeTiers).Where(f => f.FeeTypeId.Equals(feeTypeId));
            if (feeRateConfigs == null) return new List<FeeRateConfigDto>();
            var feeRateConfigDtos = new List<FeeRateConfigDto>();

            foreach (var feeRateConfig in feeRateConfigs)
            {
                var feeRateConfigDto = new FeeRateConfigDto()
                {
                    ApartmentBuildingId = feeRateConfig.ApartmentBuildingId,
                    FeeTypeId = feeRateConfig.FeeTypeId,
                    Id = feeRateConfig.Id,
                    IsActive = feeRateConfig.IsActive,
                    Name = feeRateConfig.Name,
                    VATRate = feeRateConfig.VATRate
                };
                if (feeRateConfig.FeeTiers != null)
                {
                    feeRateConfigDto.FeeTiers = feeRateConfig.FeeTiers.Select(ft => new FeeTierDto()
                    {
                        ConsumptionEnd = ft.ConsumptionEnd,
                        ConsumptionStart = ft.ConsumptionStart,
                        FeeRateConfigId = ft.FeeRateConfigId,
                        Id = ft.Id,
                        TierOrder = ft.TierOrder,
                        UnitName = ft.UnitName,
                        UnitRate = ft.UnitRate
                    });
                }
                feeRateConfigDtos.Add(feeRateConfigDto);
            }
            return feeRateConfigDtos;
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
                FeeRateConfigIdApplicable = feeRateConfigIdApplicable,
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
                    FeeRateConfigIdApplicable = feeRateConfigIdApplicable
                });
            }
            return feeTypeDtos;
        }

        public async Task UpdateFeeRateConfig(UpdateFeeRateConfigDto request)
        {
            var feeRateConfig = _feeRateConfigRepository.List().Include(f => f.FeeTiers).FirstOrDefault(f => f.Id.Equals(request.Id));
            if (feeRateConfig == null)
                throw new DomainException(ErrorCodeConsts.FeeRateConfigNotFound, ErrorCodeConsts.FeeRateConfigNotFound, System.Net.HttpStatusCode.NotFound);
            feeRateConfig.Name = request.Name;
            feeRateConfig.VATRate = request.VATRate;
            if (request.FeeTiers == null)
            {
                feeRateConfig.FeeTiers = null;
            }
            else
            {
                feeRateConfig.FeeTiers = request.FeeTiers.Select(ft => new FeeTier()
                {
                    Id = ft.Id,
                    ConsumptionEnd = ft.ConsumptionEnd,
                    ConsumptionStart = ft.ConsumptionStart,
                    TierOrder = ft.TierOrder,
                    UnitName = ft.UnitName,
                    UnitRate = ft.UnitRate,
                }).ToList();
            }
            _feeRateConfigRepository.Update(feeRateConfig);
            await _unitOfWork.CommitAsync();
        }
    }
}