using ApartmentManagementSystem.Consts;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.Services.Impls;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApartmentManagementSystem.Tests
{
    public class FeeServiceTests
    {
        private ServiceProvider _serviceProvider;
        private IServiceScope _scope;
        private IFeeService _service;
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
            services.AddScoped<IFeeService, FeeService>();

            _serviceProvider = services.BuildServiceProvider();

            _scope = _serviceProvider.CreateScope();
            _service = _scope.ServiceProvider.GetRequiredService<IFeeService>();
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
        public async Task CreateFeeNotice_ValidRequest_ShouldSaveToDb()
        {
            _dbContext.Apartments.Add(new Apartment { Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), Name = "A101", ApartmentBuildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c") });

            var feeType = new FeeType { Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), Name = "Điện", CalculationType = CalculationType.TIERED, IsActive = true };

            var rateConfig = new FeeRateConfig { Id = Guid.NewGuid(), FeeTypeId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), Name = "Giá điện", UnitName = "kWh", IsActive = true, ApplyDate = new DateTime(2024, 1, 1), VATRate = 5 };
            var tier = new FeeTier { Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), FeeRateConfigId = rateConfig.Id, UnitRate = 2000, ConsumptionStart = 0, ConsumptionEnd = 100 };

            feeType.FeeRateConfigs = new List<FeeRateConfig> { rateConfig };
            rateConfig.FeeTiers = new List<FeeTier> { tier };

            _dbContext.FeeTypes.Add(feeType);
            _dbContext.BillingCycleSettings.Add(new BillingCycleSetting
            {
                Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                ApartmentBuildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                ClosingDayOfMonth = 31,
                PaymentDueDate = 15
            });
            await _dbContext.SaveChangesAsync();

            var requests = new List<CreateOrUpdateFeeNoticeDto>
            {
                new CreateOrUpdateFeeNoticeDto
                {
                    Id = null,
                    ApartmentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                    ApartmentBuildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                    BillingCycle = "2025-11",
                    FeeTypeIds = new List<Guid> { new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c") },
                    FeeDetails = new List<CreateOrUpdateFeeDetailDto>
                    {
                        new CreateOrUpdateFeeDetailDto
                        {
                            ApartmentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                            FeeTypeId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                            UtilityReading = new CreateUtilityReadingDto
                            {
                                CurrentReading = 100,
                                ReadingDate = DateTime.Now
                            }
                        }
                    }
                }
            };

            await _service.CreateFeeNotice(requests);

            var noticeInDb = await _dbContext.FeeNotices.FirstOrDefaultAsync(x => x.ApartmentId == new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));
            Assert.That(noticeInDb, Is.Not.Null);
            Assert.That(noticeInDb.BillingCycle, Is.EqualTo("2025-11"));

            var detailInDb = await _dbContext.FeeDetails.FirstOrDefaultAsync(x => x.FeeNoticeId == noticeInDb.Id);
            Assert.That(detailInDb, Is.Not.Null);
        }


        [Test]
        public async Task GetFeeDetail_ExistingId_ReturnsDtoWithRelations()
        {
            var feeNotice = new FeeNotice
            {
                Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                BillingCycle = "01/2026",
                TotalAmount = 500000,
                Status = FeeNoticeStatus.Issued,
                PaymentStatus = FeeNoticeStatus.UnPaid
            };

            var detail = new FeeDetail
            {
                Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                FeeNoticeId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                SubTotal = 500000
            };

            _dbContext.FeeNotices.Add(feeNotice);
            _dbContext.FeeDetails.Add(detail);
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetFeeDetail(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c")));
            Assert.That(result.TotalAmount, Is.EqualTo(500000));
            Assert.That(result.FeeDetails, Is.Not.Null);
        }

        [Test]
        public void GetFeeNotices_FilterByBillingCycle_ReturnsCorrectData()
        {
            _dbContext.FeeNotices.AddRange(
                new FeeNotice { BillingCycle = "2026-01", TotalAmount = 100, Status = FeeNoticeStatus.Issued, PaymentStatus = FeeNoticeStatus.UnPaid },
                new FeeNotice { BillingCycle = "2026-02", TotalAmount = 200, Status = FeeNoticeStatus.Issued, PaymentStatus = FeeNoticeStatus.UnPaid },
                new FeeNotice { BillingCycle = "2026-03", TotalAmount = 300, Status = FeeNoticeStatus.Issued, PaymentStatus = FeeNoticeStatus.UnPaid }
            );
            _dbContext.SaveChanges();

            var request = new RequestQueryBaseDto<Guid>
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<FilterQuery>
                {
                    new FilterQuery
                    {
                        Code = "BillingCycle",
                        Operator = FilterOperator.Equals,
                        Value = "2026-01"
                    }
                }
            };

            var result = _service.GetFeeNotices(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Totals, Is.EqualTo(1));
            Assert.That(result.Items.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdatePaymentStatus_PendingToPaid_ShouldUpdateDb()
        {
            _dbContext.FeeNotices.Add(new FeeNotice
            {
                Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                TotalAmount = 100000,
                BillingCycle = "2025-12",
                Status = FeeNoticeStatus.Issued,
                PaymentStatus = FeeNoticeStatus.UnPaid
            });
            await _dbContext.SaveChangesAsync();

            await _service.UpdatePaymentStatusFeeNotice(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));

            var updatedEntity = await _dbContext.FeeNotices.FindAsync(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));
            Assert.That(updatedEntity.PaymentStatus, Is.EqualTo(FeeNoticeStatus.Paid));
        }

        [Test]
        public async Task DeleteFeeNotice_ValidIds_ShouldRemoveRecords()
        {
            // Arrange
            _dbContext.FeeNotices.AddRange(
                new FeeNotice
                {
                    Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                    BillingCycle = "2025-12",
                    Status = FeeNoticeStatus.Canceled,
                    PaymentStatus = FeeNoticeStatus.UnPaid
                },
                new FeeNotice
                {
                    Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d8352"),
                    BillingCycle = "2025-11",
                    Status = FeeNoticeStatus.Canceled,
                    PaymentStatus = FeeNoticeStatus.UnPaid
                }
            );
            await _dbContext.SaveChangesAsync();

            var idsToDelete = new List<string> { "8901c8ad-0d02-4d11-85c6-e1e3ba9d835c", "8901c8ad-0d02-4d11-85c6-e1e3ba9d8352" };

            await _service.DeletFeeNotice(idsToDelete);

            var count = await _dbContext.FeeNotices.CountAsync();
            Assert.That(count, Is.EqualTo(0));

            var deletedItem = await _dbContext.FeeNotices.FindAsync(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));
            Assert.That(deletedItem, Is.Null);
        }
    }
}