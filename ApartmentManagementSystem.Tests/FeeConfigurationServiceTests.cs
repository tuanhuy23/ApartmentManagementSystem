using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF;
using ApartmentManagementSystem.EF.Context;
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
        public async Task CreateOrUpdate_NewId_ShouldCreateRecordInDb()
        {
            // Arrange
            var request = new CreateOrUpdateFeeTypeDto
            {
                Id = null, // ID Null => Create
                Name = "Tiền Điện",
                CalculationType = "Theo Đồng Hồ",
                IsActive = true
            };

            // Act
            await _service.CreateOrUpdateFeeType(request);

            // Assert
            var inDb = await _dbContext.FeeTypes.FirstOrDefaultAsync(x => x.Name == "Tiền Điện");
            Assert.That(inDb, Is.Not.Null);
            Assert.That(inDb.CalculationType, Is.EqualTo("Theo Đồng Hồ"));
        }

        [Test]
        public async Task CreateOrUpdate_ExistingId_ShouldUpdateRecord()
        {
            // Arrange (Seed Data)
            var id = Guid.NewGuid();
            _dbContext.FeeTypes.Add(new FeeType { Id = id, Name = "Cũ", CalculationType = "Type1" });
            await _dbContext.SaveChangesAsync();

            var request = new CreateOrUpdateFeeTypeDto
            {
                Id = id, // ID tồn tại => Update
                Name = "Mới",
                CalculationType = "Type2"
            };

            // Act
            await _service.CreateOrUpdateFeeType(request);

            // Assert
            var updated = await _dbContext.FeeTypes.FindAsync(id);
            Assert.That(updated.Name, Is.EqualTo("Mới"));
            Assert.That(updated.CalculationType, Is.EqualTo("Type2"));
        }

        [Test]
        public void CreateOrUpdate_NonExistingId_ShouldThrowException()
        {
            // Arrange
            var request = new CreateOrUpdateFeeTypeDto 
            { 
                Id = Guid.NewGuid(), // ID ảo
                Name = "Test Fail" 
            };

            // Act & Assert
            // Kiểm tra Service ném lỗi KeyNotFoundException khi update ID không tồn tại
            Assert.ThrowsAsync<KeyNotFoundException>(async () => 
                await _service.CreateOrUpdateFeeType(request));
        }

        // =========================================================================
        // Function 2: GetFeeType (Single) (2 Test Cases)
        // =========================================================================

        [Test]
        public async Task GetFeeType_ValidId_ReturnsDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            _dbContext.FeeTypes.Add(new FeeType 
            { 
                Id = id, 
                Name = "Tiền Nước", 
                IsActive = true 
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetFeeType(id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Tiền Nước"));
            Assert.That(result.Id, Is.EqualTo(id));
        }

        [Test]
        public async Task GetFeeType_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _service.GetFeeType(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.Null);
        }

        // =========================================================================
        // Function 3: GetFeeTypes (Pagination) (3 Test Cases)
        // =========================================================================

        [Test]
        public async Task GetFeeTypes_WithData_ReturnsPagedList()
        {
            // Arrange
            _dbContext.FeeTypes.AddRange(
                new FeeType { Id = Guid.NewGuid(), Name = "A_Fee" },
                new FeeType { Id = Guid.NewGuid(), Name = "B_Fee" },
                new FeeType { Id = Guid.NewGuid(), Name = "C_Fee" }
            );
            await _dbContext.SaveChangesAsync();

            var request = new RequestQueryBaseDto<string> 
            { 
                Page = 1, 
                PageSize = 2 
            };

            // Act
            var result = _service.GetFeeTypes(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Totals, Is.EqualTo(3)); // Tổng 3 bản ghi
            Assert.That(result.Items.Count, Is.EqualTo(2)); // Lấy trang 1 gồm 2 bản ghi
        }

        [Test]
        public void GetFeeTypes_EmptyDb_ReturnsEmptyResult()
        {
            // Arrange
            var request = new RequestQueryBaseDto<string> { Page = 1, PageSize = 10 };

            // Act
            var result = _service.GetFeeTypes(request);

            // Assert
            Assert.That(result.Items, Is.Empty);
            Assert.That(result.Totals, Is.EqualTo(0));
        }

        [Test]
        public async Task GetFeeTypes_SecondPage_ReturnsRemainingItems()
        {
            // Arrange: Seed 3 items
            _dbContext.FeeTypes.AddRange(
                new FeeType { Name = "1" }, new FeeType { Name = "2" }, new FeeType { Name = "3" }
            );
            await _dbContext.SaveChangesAsync();

            var request = new RequestQueryBaseDto<string> 
            { 
                Page = 2, 
                PageSize = 2 
            };

            // Act
            var result = _service.GetFeeTypes(request);

            // Assert
            Assert.That(result.Items.Count, Is.EqualTo(1)); // Trang 2 chỉ còn 1 item (item số 3)
            Assert.That(result.Totals, Is.EqualTo(3));
        }

        // =========================================================================
        // Function 4: DeleteFeeType (2 Test Cases)
        // =========================================================================

        [Test]
        public async Task DeleteFeeType_ValidList_RemovesFromDb()
        {
            // Arrange
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            _dbContext.FeeTypes.AddRange(
                new FeeType { Id = id1, Name = "Del1" },
                new FeeType { Id = id2, Name = "Del2" },
                new FeeType { Id = Guid.NewGuid(), Name = "Keep" }
            );
            await _dbContext.SaveChangesAsync();

            var idsToDelete = new List<string> { id1.ToString(), id2.ToString() };

            // Act
            await _service.DeleteFeeType(idsToDelete);

            // Assert
            var count = await _dbContext.FeeTypes.CountAsync();
            Assert.That(count, Is.EqualTo(1)); // Chỉ còn lại item "Keep"
            
            var deletedItem = await _dbContext.FeeTypes.FindAsync(id1);
            Assert.That(deletedItem, Is.Null);
        }

        [Test]
        public void DeleteFeeType_NullIds_ThrowsArgumentNullException()
        {
            // Act & Assert
            // Kiểm tra validate input đầu vào
            Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await _service.DeleteFeeType(null));
        }
    }
}