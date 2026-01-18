using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.Services.Impls;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ApartmentManagementSystem.Tests
{
    public class ResidentServiceTests
    {
        private ServiceProvider _serviceProvider;
        private IServiceScope _scope;
        private IResidentService _service;
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
            services.AddSingleton(new UserAudit()
            {
                UserDisplayName = "Unit Test",
                UserId = Guid.NewGuid().ToString(),
                UserName = "unittest"
            });

            services.RegisterDbContextApartmentManagementService();
            services.RegisterRepository();
            services.AddScoped<IResidentService, ResidentService>();

            _serviceProvider = services.BuildServiceProvider();

            _scope = _serviceProvider.CreateScope();
            _service = _scope.ServiceProvider.GetRequiredService<IResidentService>();
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
        public async Task CreateResident_ValidRequest_ShouldSaveToDb_AndCallUserService()
        {

            _dbContext.Apartments.Add(new Apartment { Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), ApartmentBuildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), Name = "A101" });
            await _dbContext.SaveChangesAsync();

            var request = new ResidentDto
            {
                Id = null,
                ApartmentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                ApartmentBuildingId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "Nguyen Van A",
                PhoneNumber = "0988888888",
                Email = "test@email.com",
                IdentityNumber = "123456789",
                MemberType = "Chủ hộ",
                UserName = "userA",
                Password = "Password123"
            };

            await _service.CreateOrUpdateResident(request);

            var residentInDb = await _dbContext.Residents.FirstOrDefaultAsync(x => x.Name == "Nguyen Van A");
            Assert.That(residentInDb, Is.Not.Null, "Resident should be saved");
            Assert.That(residentInDb.PhoneNumber, Is.EqualTo("0988888888"));

            var relation = await _dbContext.ApartmentResidents
                                           .FirstOrDefaultAsync(x => x.ResidentId == residentInDb.Id && x.ApartmentId == new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));
            Assert.That(relation, Is.Not.Null, "Relation Apartment-Resident should be created");
        }

        [Test]
        public async Task UpdateResident_ExistingId_ShouldUpdateFields()
        {
            var oldResident = new Resident
            {
                Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "Old Name",
                PhoneNumber = "000",
            };
            _dbContext.Residents.Add(oldResident);
            _dbContext.Apartments.Add(new Apartment { Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), Name = "A101", ApartmentBuildingId = Guid.NewGuid() });
            _dbContext.ApartmentResidents.Add(new ApartmentResident { ResidentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), ApartmentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), MemberType = MemberType.Member });
            await _dbContext.SaveChangesAsync();

            var request = new ResidentDto
            {
                Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                ApartmentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "New Name",
                PhoneNumber = "999",
                Email = "new@mail.com",
                MemberType = "Thành viên"
            };

            await _service.CreateOrUpdateResident(request);

            var updatedEntity = await _dbContext.Residents.FindAsync(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));
            Assert.That(updatedEntity.Name, Is.EqualTo("New Name"));
            Assert.That(updatedEntity.PhoneNumber, Is.EqualTo("999"));
        }

        [Test]
        public async Task GetResident_ValidId_ReturnsDto()
        {

            _dbContext.Residents.Add(new Resident
            {
                Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                Name = "Target Resident",
                IdentityNumber = "ID001"
            });
            _dbContext.ApartmentResidents.Add(new ApartmentResident
            {
                ResidentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                ApartmentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"),
                MemberType = MemberType.Member
            });
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetResident(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c")));
            Assert.That(result.Name, Is.EqualTo("Target Resident"));
            Assert.That(result.IdentityNumber, Is.EqualTo("ID001"));
        }

        [Test]
        public async Task GetResidents_WithData_ReturnsPagedResult()
        {
            var apartmentId = Guid.NewGuid();
            _dbContext.Apartments.Add(new Apartment { Id = apartmentId, Name = "A101", ApartmentBuildingId = Guid.NewGuid() });
            for (int i = 1; i <= 3; i++)
            {
                var rId = Guid.NewGuid();
                _dbContext.Residents.Add(new Resident { Id = rId, Name = $"Res {i}" });
                _dbContext.ApartmentResidents.Add(new ApartmentResident { ResidentId = rId, ApartmentId = apartmentId, MemberType = MemberType.Member });
            }
            await _dbContext.SaveChangesAsync();

            var request = new RequestQueryBaseDto<Guid>
            {
                Page = 1,
                PageSize = 2,
                Request = apartmentId
            };

            var result = _service.GetResidents(request);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Totals, Is.EqualTo(3));
            Assert.That(result.Items.Count, Is.EqualTo(2));
            Assert.That(result.Items.First().Name, Does.Contain("Res"));
        }


        [Test]
        public async Task DeleteResident_ValidIds_ShouldRemoveFromDb()
        {

            _dbContext.Residents.AddRange(
                new Resident { Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), Name = "Del 1" },
                new Resident { Id = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835d"), Name = "Del 2" }
            );
            _dbContext.ApartmentResidents.AddRange(
                new ApartmentResident { ResidentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), ApartmentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), MemberType = MemberType.Member },
                new ApartmentResident { ResidentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835d"), ApartmentId = new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), MemberType = MemberType.Member }
            );
            await _dbContext.SaveChangesAsync();

            var idsToDelete = new List<string> { new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c").ToString(), new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835d").ToString() };

            await _service.DeleteResident(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"), idsToDelete);


            var countRelation = await _dbContext.ApartmentResidents.CountAsync(x => x.ApartmentId == new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));
            Assert.That(countRelation, Is.EqualTo(0));

            var res1 = await _dbContext.Residents.FindAsync(new Guid("8901c8ad-0d02-4d11-85c6-e1e3ba9d835c"));
            Assert.That(res1, Is.Null);
        }
    }
}