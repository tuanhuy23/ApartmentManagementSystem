using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Dtos;
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
    public class ApartmentServiceTests
    {
        private ServiceProvider _serviceProvider;
        private IServiceScope _scope;
        private IApartmentService _service;
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
            services.AddScoped<IApartmentService, ApartmentService>();

            _serviceProvider = services.BuildServiceProvider();

            _scope = _serviceProvider.CreateScope();
            _service = _scope.ServiceProvider.GetRequiredService<IApartmentService>();
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
        public async Task CreateApartment_ValidRequest_ShouldSaveToDb()
        {
            var buildingId = Guid.NewGuid();
            var request = new ApartmentDto
            {
                Id = null, 
                ApartmentBuildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "P101",
                Area = 85.5,
                Floor = 1
            };

            await _service.CreateApartment(request);

            var inDb = await _dbContext.Apartments.FirstOrDefaultAsync(x => x.Name == "P101");
            
            Assert.That(inDb, Is.Not.Null, "Apartment should be saved to DB");
            Assert.That(inDb.Area, Is.EqualTo(85.5));
            Assert.That(inDb.Floor, Is.EqualTo(1));
            Assert.That(inDb.ApartmentBuildingId, Is.EqualTo(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c")));
        }

        [Test]
        public async Task CreateApartment_NegativeArea_ShouldThrowException()
        {
            var request = new ApartmentDto
            {
                ApartmentBuildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "Invalid Area Room",
                Area = -50, 
                Floor = 1
            };

            var ex = Assert.ThrowsAsync<DomainException>(async () => 
                await _service.CreateApartment(request));
            Assert.That(ex.Message, Does.Contain(ErrorMessageConsts.ApartmentAreaGreaterThanZero)); 
        }

        [Test]
        public async Task CreateApartment_DuplicateNameInSameBuilding_ShouldThrowException()
        {
            var buildingId =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c");
            _dbContext.Apartments.Add(new Apartment 
            { 
                Id =  new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                ApartmentBuildingId = buildingId, 
                Name = "P101", 
                Area = 100 
            });
            await _dbContext.SaveChangesAsync();

            var request = new ApartmentDto
            {
                ApartmentBuildingId = buildingId,
                Name = "P101", 
                Area = 50,
                Floor = 1
            };

            var ex = Assert.ThrowsAsync<DomainException>(async () => 
                await _service.CreateApartment(request));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessageConsts.ApartmentNameIsDuplicate));
        }

        [Test]
        public async Task GetApartment_ExistingId_ShouldReturnCorrectDto()
        {
            var id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c");
            var buildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c");
            
            _dbContext.Apartments.Add(new Apartment 
            { 
                Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), 
                ApartmentBuildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "Penthouse A", 
                Area = 200, 
                Floor = 25 
            });
            await _dbContext.SaveChangesAsync();
            var result = await _service.GetApartment(id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Name, Is.EqualTo("Penthouse A"));
            Assert.That(result.ApartmentBuildingId, Is.EqualTo(buildingId));
        }

        [Test]
        public void GetApartment_NonExistingId_ShouldThrowNotFound()
        {
            var randomId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c");
            var ex = Assert.ThrowsAsync<DomainException>(async () => 
                await _service.GetApartment(randomId));
            
            Assert.That(ex.Message, Is.EqualTo(ErrorMessageConsts.ApartmentNotFound));
        }

        [Test]
        public async Task UpdateApartment_ValidRequest_ShouldUpdateFields()
        {
            var id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c");
            _dbContext.Apartments.Add(new Apartment 
            { 
                Id = id, 
                Name = "Old Name", 
                Area = 50, 
                Floor = 2 
            });
            await _dbContext.SaveChangesAsync();

            var request = new UpdateApartmentDto
            {
                Id = id,
                Name = "New Name",
                Area = 60.5,
                Floor = 3
            };

            await _service.UpdateApartment(request);

            var updatedEntity = await _dbContext.Apartments.FindAsync(id);
            
            Assert.That(updatedEntity, Is.Not.Null);
            Assert.That(updatedEntity.Name, Is.EqualTo("New Name"));
            Assert.That(updatedEntity.Area, Is.EqualTo(60.5));
            Assert.That(updatedEntity.Floor, Is.EqualTo(3));
        }

        [Test]
        public void UpdateApartment_NonExistingId_ShouldThrowKeyNotFoundException()
        {
            var request = new UpdateApartmentDto
            {
                Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "Ghost Apartment",
                Area = 100,
                Floor = 1
            };
             var ex = Assert.ThrowsAsync<DomainException>(async () =>
                await _service.UpdateApartment(request));
            Assert.That(ex.Message, Is.EqualTo(ErrorMessageConsts.ApartmentNotFound));
        }

        [Test]
        public async Task DeleteApartment_ExistingId_ShouldRemoveFromDb()
        {
            var id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c");
            _dbContext.Apartments.Add(new Apartment { Id = id, Name = "To Delete" });
            await _dbContext.SaveChangesAsync();

            await _service.DeleteApartment(id);

            var deletedEntity = await _dbContext.Apartments.FindAsync(id);
            var count = await _dbContext.Apartments.CountAsync();

            Assert.That(deletedEntity, Is.Null, "Entity should be null after delete");
            Assert.That(count, Is.EqualTo(0));
        }
    }
}