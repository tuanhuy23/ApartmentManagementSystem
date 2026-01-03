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
            var apartmentId = Guid.NewGuid();
            var buildingId = Guid.NewGuid();

            _dbContext.Apartments.Add(new Apartment { Id = apartmentId, ApartmentBuildingId = buildingId, Name = "A101" });
            await _dbContext.SaveChangesAsync();

            var request = new ResidentDto
            {
                Id = null,
                ApartmentId = apartmentId,
                ApartmentBuildingId = buildingId,
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
                                           .FirstOrDefaultAsync(x => x.ResidentId == residentInDb.Id && x.ApartmentId == apartmentId);
            Assert.That(relation, Is.Not.Null, "Relation Apartment-Resident should be created");
        }

        [Test]
        public async Task UpdateResident_ExistingId_ShouldUpdateFields()
        {
            var residentId = Guid.NewGuid();
            var apartmentId = Guid.NewGuid();
            var oldResident = new Resident
            {
                Id = residentId,
                Name = "Old Name",
                PhoneNumber = "000",
            };
            _dbContext.Residents.Add(oldResident);
            _dbContext.Apartments.Add(new Apartment { Id = apartmentId, Name = "A101", ApartmentBuildingId = Guid.NewGuid() });
            _dbContext.ApartmentResidents.Add(new ApartmentResident { ResidentId = residentId, ApartmentId = apartmentId, MemberType = MemberType.Member });
            await _dbContext.SaveChangesAsync();

            var request = new ResidentDto
            {
                Id = residentId,
                ApartmentId = apartmentId,
                Name = "New Name",
                PhoneNumber = "999",
                Email = "new@mail.com",
                MemberType = "Thành viên"
            };

            await _service.CreateOrUpdateResident(request);

            var updatedEntity = await _dbContext.Residents.FindAsync(residentId);
            Assert.That(updatedEntity.Name, Is.EqualTo("New Name"));
            Assert.That(updatedEntity.PhoneNumber, Is.EqualTo("999"));
        }

        [Test]
        public async Task GetResident_ValidId_ReturnsDto()
        {
            var residentId = Guid.NewGuid();
            var apartmentId = Guid.NewGuid();

            _dbContext.Residents.Add(new Resident
            {
                Id = residentId,
                Name = "Target Resident",
                IdentityNumber = "ID001"
            });
            _dbContext.ApartmentResidents.Add(new ApartmentResident
            {
                ResidentId = residentId,
                ApartmentId = apartmentId,
                MemberType = MemberType.Member
            });
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetResident(residentId, apartmentId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(residentId));
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
            var rId1 = Guid.NewGuid();
            var rId2 = Guid.NewGuid();
            var apartmentId = Guid.NewGuid();

            _dbContext.Residents.AddRange(
                new Resident { Id = rId1, Name = "Del 1" },
                new Resident { Id = rId2, Name = "Del 2" }
            );
            _dbContext.ApartmentResidents.AddRange(
                new ApartmentResident { ResidentId = rId1, ApartmentId = apartmentId, MemberType = MemberType.Member },
                new ApartmentResident { ResidentId = rId2, ApartmentId = apartmentId, MemberType = MemberType.Member }
            );
            await _dbContext.SaveChangesAsync();

            var idsToDelete = new List<string> { rId1.ToString(), rId2.ToString() };

            await _service.DeleteResident(apartmentId, idsToDelete);


            var countRelation = await _dbContext.ApartmentResidents.CountAsync(x => x.ApartmentId == apartmentId);
            Assert.That(countRelation, Is.EqualTo(0));

            var res1 = await _dbContext.Residents.FindAsync(rId1);
            Assert.That(res1, Is.Null);
        }
    }
}