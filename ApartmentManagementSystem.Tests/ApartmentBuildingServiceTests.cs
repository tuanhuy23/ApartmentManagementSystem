using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Services;
using ApartmentManagementSystem.Services.Impls;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using ApartmentManagementSystem.EF;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Consts;

namespace ApartmentManagementSystem.Tests
{
    [TestFixture]
    public class ApartmentBuildingServiceTests
    {
        private ServiceProvider _serviceProvider;
        private IServiceScope _scope;
        private IApartmentBuildingService _service;
        private ApartmentManagementDbContext _dbContext;

        private Mock<IUserService> _mockUserService;
        private Mock<IRoleService> _mockRoleService;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApartmentManagementDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString()));

            _mockUserService = new Mock<IUserService>();
            _mockRoleService = new Mock<IRoleService>();

            services.AddTransient(_ => _mockUserService.Object);
            services.AddTransient(_ => _mockRoleService.Object);
            services.AddTransient(_ => new ApartmentBuildingData());
            services.AddSingleton(new UserAudit()
            {
                UserDisplayName = "Unit Test",
                UserId = Guid.NewGuid().ToString(),
                UserName = "unittest"
            });

            services.RegisterDbContextApartmentManagementService();
            services.RegisterRepository();
            services.AddScoped<IApartmentBuildingService, ApartmentBuildingService>();

            _serviceProvider = services.BuildServiceProvider();

            _scope = _serviceProvider.CreateScope();
            _service = _scope.ServiceProvider.GetRequiredService<IApartmentBuildingService>();
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
        public async Task CreateOrUpdate_NewId_ShouldCreateRecord()
        {
            var request = new CreateOrUpdateApartmentBuildingDto
            {
                Id = null,
                Name = "Grand Marina",
                Address = "District 1, HCMC",
                ContactEmail = "email@example.com",
                ContactPhone = "123456789",
                CurrencyUnit = "USD",
                ApartmentBuildingImgUrl = "http://example.com/image.jpg",
                ManagementDisplayName = "Manager Name",
                ManagementEmail = "emal@emample.com",
                ManagementUserName = "manageruser",
                ManagementPhoneNumber = "987654321",
                ManagementPassword = "SecurePassword123!",
                Description = "A luxury apartment building.",
            };
            await _service.CreateOrUpdateApartmentBuilding(request);

            var inDb = await _dbContext.ApartmentBuildings.FirstOrDefaultAsync(x => x.Name == "Grand Marina");
            Assert.That(inDb, Is.Not.Null);
            Assert.That(inDb.Address, Is.EqualTo("District 1, HCMC"));
        }

        [Test]
        public async Task CreateOrUpdate_ExistingId_ShouldUpdateRecord()
        {
            var id = Guid.NewGuid();
            _dbContext.ApartmentBuildings.Add(new ApartmentBuilding
            {
                Id = id,
                Name = "A",
                Address = "District 1, HCMC",
                ContactEmail = "email@example.com",
                ContactPhone = "123456789",
                CurrencyUnit = "USD",
                ApartmentBuildingImgUrl = "http://example.com/image.jpg",
                Description = "Old Description",
                Status = StatusConsts.Active
            });
            await _dbContext.SaveChangesAsync();

            var request = new CreateOrUpdateApartmentBuildingDto
            {
                Id = id,
                Name = "New Name",
                Address = "New Address"
            };

            await _service.CreateOrUpdateApartmentBuilding(request);

            var updated = await _dbContext.ApartmentBuildings.FindAsync(id);
            Assert.That(updated.Name, Is.EqualTo("New Name"));
            Assert.That(updated.Address, Is.EqualTo("New Address"));
        }

        [Test]
        public void CreateOrUpdate_NonExistingId_ShouldThrowException()
        {
            var request = new CreateOrUpdateApartmentBuildingDto
            {
                Id = Guid.NewGuid(),
                Name = "Fail"
            };
            var exception = Assert.ThrowsAsync<Exceptions.DomainException>(async () => await _service.CreateOrUpdateApartmentBuilding(request));
            Assert.That(exception.Message, Is.EqualTo(ErrorMessageConsts.ApartmentBuildingNotFound));
            Assert.That(exception.Code, Is.EqualTo(ErrorCodeConsts.ApartmentBuildingNotFound));
        }

        [Test]
        public async Task GetApartmentBuildings_WithData_ReturnsCorrectPagination()
        {
            _dbContext.ApartmentBuildings.AddRange(
                new ApartmentBuilding
                {
                    Id = Guid.NewGuid(),
                    Name = "A",
                    Address = "District 1, HCMC",
                    ContactEmail = "email@example.com",
                    ContactPhone = "123456789",
                    CurrencyUnit = "USD",
                    ApartmentBuildingImgUrl = "http://example.com/image.jpg",
                    Description = "Old Description",
                    Status = StatusConsts.Active,
                },
                new ApartmentBuilding
                {
                    Id = Guid.NewGuid(),
                    Name = "B",
                    Address = "District 1, HCMC",
                    ContactEmail = "email@example.com",
                    ContactPhone = "123456789",
                    CurrencyUnit = "USD",
                    ApartmentBuildingImgUrl = "http://example.com/image.jpg",
                    Description = "Old Description",
                    Status = StatusConsts.Active
                },
                new ApartmentBuilding
                {
                    Id = Guid.NewGuid(),
                    Name = "C",
                    Address = "District 1, HCMC",
                    ContactEmail = "email@example.com",
                    ContactPhone = "123456789",
                    CurrencyUnit = "USD",
                    ApartmentBuildingImgUrl = "http://example.com/image.jpg",
                    Description = "Old Description",
                    Status = StatusConsts.Active
                }
            );
            await _dbContext.SaveChangesAsync();

            var request = new RequestQueryBaseDto<object>
            {
                Page = 1,
                PageSize = 2
            };

            var result = _service.GetApartmentBuildings(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Totals, Is.EqualTo(3));
            Assert.That(result.Items.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetApartmentBuildings_EmptyDb_ReturnsEmptyList()
        {
            var request = new RequestQueryBaseDto<object> { Page = 1, PageSize = 10 };

            var result = _service.GetApartmentBuildings(request);

            Assert.That(result.Items, Is.Empty);
            Assert.That(result.Totals, Is.EqualTo(0));
        }


        [Test]
        public async Task GetById_ValidId_ReturnsDto()
        {
            var id = Guid.NewGuid();
            _dbContext.ApartmentBuildings.Add(new ApartmentBuilding
            {
                Id = id,
                Name = "Target",
                Address = "District 1, HCMC",
                ContactEmail = "email@example.com",
                ContactPhone = "123456789",
                CurrencyUnit = "USD",
                ApartmentBuildingImgUrl = "http://example.com/image.jpg",
                Description = "Old Description",
                Status = StatusConsts.Active
            });
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetApartmentBuilding(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Target"));
        }

        [Test]
        public async Task GetById_InvalidId_ReturnsNull()
        {
            var exception = Assert.ThrowsAsync<Exceptions.DomainException>(async () => await _service.GetApartmentBuilding(Guid.NewGuid()));
            Assert.That(exception.Message, Is.EqualTo(ErrorMessageConsts.ApartmentBuildingNotFound));
            Assert.That(exception.Code, Is.EqualTo(ErrorCodeConsts.ApartmentBuildingNotFound));
        }


        [Test]
        public async Task Delete_ValidList_RemovesItems()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            _dbContext.ApartmentBuildings.AddRange(
                new ApartmentBuilding
                {
                    Id = id1,
                    Name = "Del1",
                    Address = "District 1, HCMC",
                    ContactEmail = "email@example.com",
                    ContactPhone = "123456789",
                    CurrencyUnit = "USD",
                    ApartmentBuildingImgUrl = "http://example.com/image.jpg",
                    Description = "Old Description",
                    Status = StatusConsts.Active
                },
                new ApartmentBuilding
                {
                    Id = id2,
                    Name = "Del2",
                    Address = "District 1, HCMC",
                    ContactEmail = "email@example.com",
                    ContactPhone = "123456789",
                    CurrencyUnit = "USD",
                    ApartmentBuildingImgUrl = "http://example.com/image.jpg",
                    Description = "Old Description",
                    Status = StatusConsts.Active
                }
            );
            await _dbContext.SaveChangesAsync();

            var ids = new List<string> { id1.ToString(), id2.ToString() };
            await _service.DeleteApartmentBuilding(ids);

            var count = await _dbContext.ApartmentBuildings.CountAsync();
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Delete_NullIds_ThrowsException()
        {
            var exception = Assert.ThrowsAsync<Exceptions.DomainException>(async () => await _service.DeleteApartmentBuilding(null));
            Assert.That(exception.Message, Is.EqualTo(ErrorMessageConsts.ApartmentBuildingNotFound));
            Assert.That(exception.Code, Is.EqualTo(ErrorCodeConsts.ApartmentBuildingNotFound));
        }

        [Test]
        public async Task UpdateStatus_Valid_ChangesStatus()
        {
            var id = Guid.NewGuid();
            _dbContext.ApartmentBuildings.Add(new ApartmentBuilding
            {
                Id = id,
                Name = "Del2",
                Address = "District 1, HCMC",
                ContactEmail = "email@example.com",
                ContactPhone = "123456789",
                CurrencyUnit = "USD",
                ApartmentBuildingImgUrl = "http://example.com/image.jpg",
                Description = "Old Description",
                Status = StatusConsts.Active
            });
            await _dbContext.SaveChangesAsync();

            var req = new UpdateStatusApartmentBuildingDto { Status = StatusConsts.InActive };

            await _service.UpdateApartmentBuildingStatus(id, req);

            var entity = await _dbContext.ApartmentBuildings.FindAsync(id);
            Assert.That(entity.Status, Is.EqualTo(StatusConsts.InActive));
        }
    }
}