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
            var apartmentId = Guid.NewGuid();
            var buildingId = Guid.NewGuid();
            _dbContext.Apartments.Add(new Apartment { Id = apartmentId, Name = "A101", ApartmentBuildingId = buildingId });

            var feeTypeId = Guid.NewGuid();
            var feeType = new FeeType { Id = feeTypeId, Name = "Điện", CalculationType = CalculationType.TIERED, IsActive = true };

            var rateConfig = new FeeRateConfig { Id = Guid.NewGuid(), FeeTypeId = feeTypeId, Name = "Giá điện", UnitName = "kWh", IsActive = true, ApplyDate = new DateTime(2024, 1, 1), VATRate = 5 };
            var tier = new FeeTier { Id = Guid.NewGuid(), FeeRateConfigId = rateConfig.Id, UnitRate = 2000, ConsumptionStart = 0, ConsumptionEnd = 100 };

            feeType.FeeRateConfigs = new List<FeeRateConfig> { rateConfig };
            rateConfig.FeeTiers = new List<FeeTier> { tier };

            _dbContext.FeeTypes.Add(feeType);
            _dbContext.BillingCycleSettings.Add(new BillingCycleSetting
            {
                Id = Guid.NewGuid(),
                ApartmentBuildingId = buildingId,
                ClosingDayOfMonth = 31,
                PaymentDueDate = 15
            });
            await _dbContext.SaveChangesAsync();

            var requests = new List<CreateOrUpdateFeeNoticeDto>
            {
                new CreateOrUpdateFeeNoticeDto
                {
                    Id = null,
                    ApartmentId = apartmentId,
                    ApartmentBuildingId = buildingId,
                    BillingCycle = "2025-11",
                    FeeTypeIds = new List<Guid> { feeTypeId },
                    FeeDetails = new List<CreateOrUpdateFeeDetailDto>
                    {
                        new CreateOrUpdateFeeDetailDto
                        {
                            ApartmentId = apartmentId,
                            FeeTypeId = feeTypeId,
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

            var noticeInDb = await _dbContext.FeeNotices.FirstOrDefaultAsync(x => x.ApartmentId == apartmentId);
            Assert.That(noticeInDb, Is.Not.Null);
            Assert.That(noticeInDb.BillingCycle, Is.EqualTo("2025-11"));

            var detailInDb = await _dbContext.FeeDetails.FirstOrDefaultAsync(x => x.FeeNoticeId == noticeInDb.Id);
            Assert.That(detailInDb, Is.Not.Null);
        }


        [Test]
        public async Task GetFeeDetail_ExistingId_ReturnsDtoWithRelations()
        {
            var noticeId = Guid.NewGuid();
            var feeNotice = new FeeNotice
            {
                Id = noticeId,
                BillingCycle = "01/2026",
                TotalAmount = 500000,
                Status = FeeNoticeStatus.Issued,
                PaymentStatus = FeeNoticeStatus.UnPaid
            };

            var detail = new FeeDetail
            {
                Id = Guid.NewGuid(),
                FeeNoticeId = noticeId,
                SubTotal = 500000
            };

            _dbContext.FeeNotices.Add(feeNotice);
            _dbContext.FeeDetails.Add(detail);
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetFeeDetail(noticeId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(noticeId));
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
            var id = Guid.NewGuid();
            _dbContext.FeeNotices.Add(new FeeNotice
            {
                Id = id,
                TotalAmount = 100000,
                BillingCycle = "2025-12",
                Status = FeeNoticeStatus.Issued,
                PaymentStatus = FeeNoticeStatus.UnPaid
            });
            await _dbContext.SaveChangesAsync();

            await _service.UpdatePaymentStatusFeeNotice(id);

            var updatedEntity = await _dbContext.FeeNotices.FindAsync(id);
            Assert.That(updatedEntity.PaymentStatus, Is.EqualTo(FeeNoticeStatus.Paid));
        }

        [Test]
        public async Task DeleteFeeNotice_ValidIds_ShouldRemoveRecords()
        {
            // Arrange
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            _dbContext.FeeNotices.AddRange(
                new FeeNotice
                {
                    Id = id1,
                    BillingCycle = "2025-12",
                    Status = FeeNoticeStatus.Canceled,
                    PaymentStatus = FeeNoticeStatus.UnPaid
                },
                new FeeNotice
                {
                    Id = id2,
                    BillingCycle = "2025-11",
                    Status = FeeNoticeStatus.Canceled,
                    PaymentStatus = FeeNoticeStatus.UnPaid
                }
            );
            await _dbContext.SaveChangesAsync();

            var idsToDelete = new List<string> { id1.ToString(), id2.ToString() };

            await _service.DeletFeeNotice(idsToDelete);

            var count = await _dbContext.FeeNotices.CountAsync();
            Assert.That(count, Is.EqualTo(0));

            var deletedItem = await _dbContext.FeeNotices.FindAsync(id1);
            Assert.That(deletedItem, Is.Null);
        }
    }
}