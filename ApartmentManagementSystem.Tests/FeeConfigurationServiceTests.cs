using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Consts;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Impls;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ApartmentManagementSystem.Tests
{
    [TestFixture]
    public class FeeConfigurationServiceTests
    {

        private ServiceProvider _serviceProvider;
        private IServiceScope _scope;
        private IFeeConfigurationService _service;
        private ApartmentManagementDbContext _dbContext;
        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApartmentManagementDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString()));

            services.AddSingleton(new UserAudit()
            {
                UserDisplayName = "Unit Test",
                UserId = Guid.NewGuid().ToString(),
                UserName = "unittest"
            });

            services.RegisterDbContextApartmentManagementService();
            services.RegisterRepository();
            services.AddScoped<IFeeConfigurationService, FeeConfigurationService>();

            _serviceProvider = services.BuildServiceProvider();

            _scope = _serviceProvider.CreateScope();
            _service = _scope.ServiceProvider.GetRequiredService<IFeeConfigurationService>();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ApartmentManagementDbContext>();
        }

        [TearDown]
        public void TearDown()
        {
            _scope?.Dispose();
            _serviceProvider?.Dispose();
            _dbContext?.Dispose();
        }

        [Test]
        public async Task Create_AreaType_ShouldSaveDefaultRate_AndIgnoreLists()
        {
            var request = new CreateOrUpdateFeeTypeDto
            {
                Id = null,
                Name = "Phí Quản Lý",
                CalculationType = CalculationType.Area,
                DefaultRate = 15000,
                IsActive = true,
                FeeRateConfigs = null,
                QuantityRateConfigs = null,
                ApartmentBuildingId = Guid.NewGuid(),
                IsVATApplicable = false,
                DefaultVATRate = 0,
                ApplyDate = new DateTime(2024, 1, 1)
            };

            await _service.CreateOrUpdateFeeType(request);

            var inDb = await _dbContext.FeeTypes.FirstOrDefaultAsync(x => x.Name == "Phí Quản Lý");
            Assert.That(inDb, Is.Not.Null);
            Assert.That(inDb.CalculationType, Is.EqualTo(CalculationType.Area));
            Assert.That(inDb.DefaultRate, Is.EqualTo(15000));
        }

        [Test]
        public async Task Create_QuantityType_ShouldSaveQuantityConfigs()
        {
            var request = new CreateOrUpdateFeeTypeDto
            {
                Name = "Phí Gửi Xe Ô tô",
                CalculationType = CalculationType.QUANTITY,
                IsActive = true,
                ApartmentBuildingId = Guid.NewGuid(),
                IsVATApplicable = false,
                DefaultVATRate = 0,
                ApplyDate = new DateTime(2024, 1, 1),
                QuantityRateConfigs = new List<CreateOrUpdateQuantityRateConfigDto>
                {
                    new CreateOrUpdateQuantityRateConfigDto
                    {
                        ItemType = "Sedan 4 chỗ",
                        UnitRate = 1200000,
                        IsActive = true
                    },
                    new CreateOrUpdateQuantityRateConfigDto
                    {
                        ItemType = "SUV 7 chỗ",
                        UnitRate = 1500000,
                        IsActive = true
                    }
                }
            };
            await _service.CreateOrUpdateFeeType(request);
            var feeType = await _dbContext.FeeTypes
                .Include(x => x.QuantityRateConfigs)
                .FirstOrDefaultAsync(x => x.Name == "Phí Gửi Xe Ô tô");

            Assert.That(feeType, Is.Not.Null);
            Assert.That(feeType.QuantityRateConfigs, Is.Not.Null);
            Assert.That(feeType.QuantityRateConfigs.Count, Is.EqualTo(2));
            Assert.That(feeType.QuantityRateConfigs.Any(x => x.ItemType == "Sedan 4 chỗ"), Is.True);
        }

        [Test]
        public async Task Create_TieredType_ShouldSaveDeeplyNestedTiers()
        {
            var request = new CreateOrUpdateFeeTypeDto
            {
                Name = "Tiền Nước Sinh Hoạt",
                CalculationType = CalculationType.TIERED,
                IsActive = true,
                FeeRateConfigs = new List<CreateOrUpdateFeeRateConfigDto>
                {
                    new CreateOrUpdateFeeRateConfigDto
                    {
                        Name = "Bảng giá nước 2024",
                        UnitName = "m3",
                        IsActive = true,
                        ApplyDate = new DateTime(2024, 1, 1),
                        VATRate = 5,
                        FeeTiers = new List<CreateOrUpdateFeeRateTierDto>
                        {
                            new CreateOrUpdateFeeRateTierDto { TierOrder = 1, ConsumptionStart = 0, ConsumptionEnd = 10, UnitRate = 6000 },
                            new CreateOrUpdateFeeRateTierDto { TierOrder = 2, ConsumptionStart = 11, ConsumptionEnd = 20, UnitRate = 8000 }
                        }
                    }
                }
            };

            await _service.CreateOrUpdateFeeType(request);

            var feeType = await _dbContext.FeeTypes
                .Include(x => x.FeeRateConfigs)
                .ThenInclude(c => c.FeeTiers)
                .FirstOrDefaultAsync(x => x.Name == "Tiền Nước Sinh Hoạt");

            Assert.That(feeType, Is.Not.Null);

            var config = feeType.FeeRateConfigs.FirstOrDefault();
            Assert.That(config, Is.Not.Null);
            Assert.That(config.Name, Is.EqualTo("Bảng giá nước 2024"));

            Assert.That(config.FeeTiers, Is.Not.Null);
            Assert.That(config.FeeTiers.Count, Is.EqualTo(2));
            Assert.That(config.FeeTiers.First(t => t.TierOrder == 1).UnitRate, Is.EqualTo(6000));
        }

        [Test]
        public async Task Create_TieredType_InvalidRange_ShouldThrowException()
        {
            var request = new CreateOrUpdateFeeTypeDto
            {
                Name = "Tiền Nước Sinh Hoạt",
                CalculationType = CalculationType.TIERED,
                IsActive = true,
                FeeRateConfigs = new List<CreateOrUpdateFeeRateConfigDto>
                {
                    new CreateOrUpdateFeeRateConfigDto
                    {
                        Name = "Bảng giá nước 2024",
                        UnitName = "m3",
                        IsActive = true,
                        ApplyDate = new DateTime(2024, 1, 1),
                        VATRate = 5,
                        FeeTiers = new List<CreateOrUpdateFeeRateTierDto>
                        {
                            new CreateOrUpdateFeeRateTierDto { TierOrder = 1, ConsumptionStart = 10, ConsumptionEnd = 5, UnitRate = 6000 },
                        }
                    }
                }
            };

           var ex = Assert.ThrowsAsync<DomainException>(async () => 
                await _service.CreateOrUpdateFeeType(request));
            Assert.That(ex.Message, Does.Contain(ErrorMessageConsts.FeeTierConsumptionStartMustLessThanEnd));
        }

        [Test]
        public async Task Create_BoundaryVATRate_ShouldSucceed()
        {
            var request = new CreateOrUpdateFeeTypeDto
            {
                Name = "VAT Cao",
                CalculationType = CalculationType.Area,
                DefaultRate = 1000,
                IsVATApplicable = true,
                DefaultVATRate = 100,
                IsActive = true
            };

            await _service.CreateOrUpdateFeeType(request);
            var inDb = await _dbContext.FeeTypes.FirstOrDefaultAsync(x => x.Name == "VAT Cao");
            Assert.That(inDb.DefaultVATRate, Is.EqualTo(100));
        }

        [Test]
        public async Task Update_FeeType_ShouldUpdateProperties_AndConfigs()
        {
            var oldEntity = new FeeType
            {
                Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "Cũ",
                CalculationType = CalculationType.Area,
                DefaultRate = 5000,
                IsActive = true,
                ApplyDate = new DateTime(2024, 1, 1),
                ApartmentBuildingId = Guid.NewGuid(),
            };
            _dbContext.FeeTypes.Add(oldEntity);
            await _dbContext.SaveChangesAsync();

            var request = new CreateOrUpdateFeeTypeDto
            {
                Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "Mới",
                CalculationType = CalculationType.Area,
                DefaultRate = 10000,
                IsActive = true,
            };

            await _service.CreateOrUpdateFeeType(request);

            var updated = await _dbContext.FeeTypes.FindAsync(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));
            Assert.That(updated.Name, Is.EqualTo("Mới"));
            Assert.That(updated.DefaultRate, Is.EqualTo(10000));
        }

        [Test]
        public void CreateOrUpdate_InvalidId_ShouldThrowException()
        {
            var request = new CreateOrUpdateFeeTypeDto { Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), Name = "Fake" };

            var ex = Assert.ThrowsAsync<DomainException>(async () =>
                await _service.CreateOrUpdateFeeType(request));
            Assert.That(ex.Code, Is.EqualTo(ErrorCodeConsts.FeeTypeNotFound));
        }

        [Test]
        public async Task GetById_QuantityType_ShouldIncludeConfigs()
        {
            var feeType = new FeeType
            {
                Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "Gửi xe máy",
                CalculationType = CalculationType.QUANTITY,
                ApartmentBuildingId = Guid.NewGuid(),
                IsActive = true,
                IsVATApplicable = false,
                DefaultVATRate = 0,
                ApplyDate = new DateTime(2024, 1, 1),
                QuantityRateConfigs = new List<QuantityRateConfig>()
                {
                    new QuantityRateConfig
                    {
                        Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835d"),
                        FeeTypeId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                        ItemType = "Honda",
                        UnitRate = 50000
                    }
                }
            };

            _dbContext.FeeTypes.Add(feeType);
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetFeeType(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c")));

            Assert.That(result.QuantityRateConfigs, Is.Not.Null);
            Assert.That(result.QuantityRateConfigs.Count(), Is.EqualTo(1));
            Assert.That(result.QuantityRateConfigs.First().ItemType, Is.EqualTo("Honda"));
        }

        [Test]
        public async Task GetById_TieredType_ShouldIncludeConfigsAndTiers()
        {
            var feeType = new FeeType
            {
                Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "Điện",
                CalculationType = CalculationType.TIERED,
                IsActive = true,
                FeeRateConfigs = new List<FeeRateConfig>()
                {
                    new FeeRateConfig
                    {
                        Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                        FeeTypeId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                        Name = "Giá điện",
                        UnitName = "kWh",
                        IsActive = true,
                        ApplyDate = new DateTime(2024, 1, 1),
                        VATRate = 5,
                        FeeTiers = new List<FeeTier>()
                        {
                            new FeeTier
                            {
                                Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835a"),
                                FeeRateConfigId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                                UnitRate = 2000
                            }
                        }
                    },
                },
            };

            _dbContext.FeeTypes.Add(feeType);
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetFeeType(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FeeRateConfigs, Is.Not.Null.And.Not.Empty);

            var firstConfig = result.FeeRateConfigs.First();
            Assert.That(firstConfig.FeeTiers, Is.Not.Null.And.Not.Empty);
            Assert.That(firstConfig.FeeTiers.First().UnitRate, Is.EqualTo(2000));
        }

        [Test]
        public async Task GetFeeTypes_FilterByName_ShouldReturnFilteredResult()
        {
            _dbContext.FeeTypes.AddRange(
                new FeeType
                {
                    Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                    Name = "Dịch vụ A",
                    CalculationType = "AREA",
                    DefaultRate = 5000,
                    IsActive = true,
                    ApplyDate = new DateTime(2024, 1, 1),
                    ApartmentBuildingId =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                },
                new FeeType
                {
                    Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835d"),
                    Name = "Dịch vụ B",
                    CalculationType = "AREA",
                    DefaultRate = 5000,
                    IsActive = true,
                    ApplyDate = new DateTime(2024, 1, 1),
                    ApartmentBuildingId =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                },
                new FeeType
                {
                    Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835b"),
                    Name = "Khác",
                    CalculationType = "AREA",
                    DefaultRate = 5000,
                    IsActive = true,
                    ApplyDate = new DateTime(2024, 1, 1),
                    ApartmentBuildingId =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                }
            );
            await _dbContext.SaveChangesAsync();

            var request = new RequestQueryBaseDto<string> { Page = 1, PageSize = 10, Request = "8901c8ad-0d02-4d11-85c6-e1e3ba9d835c" };
            var result = _service.GetFeeTypes(request);
            Assert.That(result.Totals, Is.EqualTo(3));
            Assert.That(result.Items.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task DeleteFeeType_ShouldDeleteCascadeConfigs()
        {
            var feeType = new FeeType
            {
                Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835b"),
                CalculationType = CalculationType.QUANTITY,
                IsActive = true,
                ApplyDate = new DateTime(2024, 1, 1),
                ApartmentBuildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835b"),
                Name = "Phí Gửi Xe",
                QuantityRateConfigs = new List<QuantityRateConfig>()
                {
                   new QuantityRateConfig {
                       Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                       FeeTypeId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835b"),
                       ItemType = "Test",
                       UnitRate = 1000,
                       ApartmentBuildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835b"),
                       IsActive = true
                   }
                }
            };
            _dbContext.FeeTypes.Add(feeType);
            await _dbContext.SaveChangesAsync();

            await _service.DeleteFeeType(new List<string> { "8901c8ad-0d02-4d11-85c6-e1e3ba9d835b" });

            var deletedParent = await _dbContext.FeeTypes.FindAsync(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));
            Assert.That(deletedParent, Is.Null);
        }

        [Test]
        public void DeleteFeeType_EmptyList_ShouldNotThrow()
        {
            var ex = Assert.ThrowsAsync<DomainException>(async () =>
               await _service.DeleteFeeType(new List<string>()));
            Assert.That(ex.Code, Is.EqualTo(ErrorCodeConsts.FeeTypeNotFound));
        }
    }
}