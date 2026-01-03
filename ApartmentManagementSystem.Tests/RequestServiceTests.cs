using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Consts;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.Services.Impls;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace ApartmentManagementSystem.Tests
{
    public class RequestServiceTests
    {
        private ServiceProvider _serviceProvider;
        private IServiceScope _scope;
        private IRequestService _service;
        private ApartmentManagementDbContext _dbContext;

        private Mock<IUserService> _mockUserService;
        private Mock<IAccountService> _mockAccountService;
        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApartmentManagementDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString()));

            _mockUserService = new Mock<IUserService>();
            _mockAccountService = new Mock<IAccountService>();

            services.AddTransient(_ => _mockUserService.Object);
            services.AddTransient(_ => _mockAccountService.Object);
            services.AddSingleton(new UserAudit()
            {
                UserDisplayName = "Unit Test",
                UserId = Guid.NewGuid().ToString(),
                UserName = "unittest"
            });

            services.RegisterDbContextApartmentManagementService();
            services.RegisterRepository();
            services.AddScoped<IRequestService, RequestService>();

            _serviceProvider = services.BuildServiceProvider();

            _scope = _serviceProvider.CreateScope();
            _service = _scope.ServiceProvider.GetRequiredService<IRequestService>();
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
        public async Task CreateRequest_ValidData_ShouldSaveToDb()
        {
            var userId = Guid.NewGuid().ToString();
            await _dbContext.Residents.AddAsync(new Resident
            {
                Id = Guid.NewGuid(),
                Name = "Nguyen Van A",
                IdentityNumber = "123456789",
                PhoneNumber = "0987654321",
                ApartmentBuildingId = Guid.NewGuid(),
                UserId = userId
            });
            await _dbContext.SaveChangesAsync();
            var buildingId = Guid.NewGuid();
            var requestDto = new RequestDto
            {
                Id = null,
                Title = "Sửa ống nước",
                Description = "Ống nước bị rò rỉ tại P205",
                Status = "Pending",
                RequestType = "Repair",
                ApartmentBuildingId = buildingId,
                CreatedDate = DateTime.Now
            };
            _mockAccountService.Setup(x => x.GetAccountInfo())
                            .ReturnsAsync(new AccountInfoResponseDto
                            {
                                Id = userId,
                                UserName = "Nguyen Van A",
                                RoleName = RoleDefaulConsts.Resident
                            });
            await _service.CreateOrUpdateRequest(requestDto);

            var inDb = await _dbContext.Requests.FirstOrDefaultAsync(x => x.Title == "Sửa ống nước");
            Assert.That(inDb, Is.Not.Null);
            Assert.That(inDb.Description, Is.EqualTo("Ống nước bị rò rỉ tại P205"));
            Assert.That(inDb.Status, Is.EqualTo(StatusConsts.New));
        }

        [Test]
        public async Task CreateRequest_WithFiles_ShouldSaveAttachments()
        {
            var userId = Guid.NewGuid().ToString();
            await _dbContext.Residents.AddAsync(new Resident
            {
                Id = Guid.NewGuid(),
                Name = "Nguyen Van A",
                IdentityNumber = "123456789",
                PhoneNumber = "0987654321",
                ApartmentBuildingId = Guid.NewGuid(),
                UserId = userId
            });
            await _dbContext.SaveChangesAsync();
            var buildingId = Guid.NewGuid();
            var requestDto = new RequestDto
            {
                Title = "Yêu cầu có ảnh",
                Files = new List<FileAttachmentDto>
                {
                    new FileAttachmentDto { Name = "anh1.jpg", Src = "/uploads/anh1.jpg", FileType = "Image", Description = "Ảnh 1" },
                    new FileAttachmentDto { Name = "doc.pdf", Src = "/uploads/doc.pdf", FileType = "Document", Description = "Tài liệu 1" }
                },
                ApartmentBuildingId = buildingId,
                Description = "Mô tả yêu cầu có ảnh",
                RequestType = "Repair",
            };
            _mockAccountService.Setup(x => x.GetAccountInfo())
                            .ReturnsAsync(new AccountInfoResponseDto
                            {
                                Id = userId,
                                UserName = "Nguyen Van A",
                                RoleName = RoleDefaulConsts.Resident
                            });
            await _service.CreateOrUpdateRequest(requestDto);

            var reqInDb = await _dbContext.Requests.FirstOrDefaultAsync(x => x.Title == "Yêu cầu có ảnh");
            Assert.That(reqInDb, Is.Not.Null);
            var filesInDb = await _dbContext.FileAttachments.Where(x => x.RequestId == reqInDb.Id).ToListAsync();
            Assert.That(filesInDb.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateRequest_ExistingId_ShouldUpdateFields()
        {
            var id = Guid.NewGuid();
            _dbContext.Requests.Add(new Request
            {
                Id = id,
                Title = "Old Title",
                Description = "Old Desc",
                Status = StatusConsts.New,
                RequestType = "Repair",
                ApartmentBuildingId = Guid.NewGuid()
            });
            var userId = Guid.NewGuid().ToString();
            _mockAccountService.Setup(x => x.GetAccountInfo())
                          .ReturnsAsync(new AccountInfoResponseDto
                          {
                              Id = userId,
                              UserName = "Nguyen Van A",
                              RoleName = RoleDefaulConsts.Resident
                          });
            await _dbContext.SaveChangesAsync();

            var updateDto = new RequestDto
            {
                Id = id,
                Title = "New Title",
                Description = "New Desc",
            };
            await _service.CreateOrUpdateRequest(updateDto);

            var updated = await _dbContext.Requests.FindAsync(id);
            Assert.That(updated.Title, Is.EqualTo("New Title"));
            Assert.That(updated.Status, Is.EqualTo(StatusConsts.New));
        }

        [Test]
        public async Task UpdateStatusAndAssign_ValidRequest_ShouldUpdateHandler()
        {
            var id = Guid.NewGuid();
            var handlerId = Guid.NewGuid().ToString();

            _dbContext.Requests.Add(new Request
            {
                Id = id,
                Status = StatusConsts.New,
                CurrentHandlerId = null,
                ApartmentBuildingId = Guid.NewGuid(),
                Description = "Test Request",
                Title = "Test Title",
                RequestType = "Repair"
            });
            await _dbContext.SaveChangesAsync();

            var requestDto = new UpdateStatusAndAssignRequestDto //
            {
                Id = id,
                Status = StatusConsts.InProgress,
                CurrentHandlerId = handlerId
            };
            var userId = Guid.NewGuid().ToString();
            _mockAccountService.Setup(x => x.GetAccountInfo())
                          .ReturnsAsync(new AccountInfoResponseDto
                          {
                              Id = userId,
                              UserName = "Nguyen Van A",
                              RoleName = RoleDefaulConsts.Management
                          });
            await _service.UpdateStatusAndAssignRequest(requestDto);

            var result = await _dbContext.Requests.FindAsync(id);
            Assert.That(result.Status, Is.EqualTo(StatusConsts.InProgress));
            Assert.That(result.CurrentHandlerId, Is.EqualTo(handlerId));
        }


        

        [Test]
        public async Task CreateRequestAction_NewHistory_ShouldAddRecord()
        {
            var requestId = Guid.NewGuid();
            _dbContext.Requests.Add(new Request
            {
                Id = requestId,
                Title = "Req 1",
                ApartmentBuildingId = Guid.NewGuid(),
                Description = "Desc 1",
                Status = StatusConsts.New,
                RequestType = "Repair",
            });
            await _dbContext.SaveChangesAsync();

            var historyDto = new RequestHistoryDto
            {
                RequestId = requestId,
                ActionType = "Comment",
                Note = "Đã tiếp nhận yêu cầu",
                CreatedDate = DateTime.Now
            };
            var userId = Guid.NewGuid().ToString();
            _mockAccountService.Setup(x => x.GetAccountInfo())
                          .ReturnsAsync(new AccountInfoResponseDto
                          {
                              Id = userId,
                              UserName = "Nguyen Van A",
                              RoleName = RoleDefaulConsts.Resident
                          });
            await _service.CreateOrUpdateRequestAction(historyDto);
            var history = await _dbContext.Requests.Include(r => r.RequestHistories).Where(x => x.Id == requestId)
                .SelectMany(r => r.RequestHistories)
                .FirstOrDefaultAsync(h => h.Note == "Đã tiếp nhận yêu cầu");
            Assert.That(history, Is.Not.Null);
            Assert.That(history.Note, Is.EqualTo("Đã tiếp nhận yêu cầu"));
        }


        [Test]
        public async Task GetRequests_FilterByStatus_ReturnsFilteredResult()
        {
            var userId = Guid.NewGuid().ToString();
            var apartmentBuidlingId = Guid.NewGuid();
            _dbContext.Requests.AddRange(
                new Request { Title = "A", Status = StatusConsts.New, ApartmentBuildingId = apartmentBuidlingId, Description = "Desc A", RequestType = "Repair", CurrentHandlerId = userId },
                new Request { Title = "B", Status = StatusConsts.InProgress, CurrentHandlerId = userId, ApartmentBuildingId = apartmentBuidlingId, Description = "Desc B", RequestType = "Repair", },
                new Request { Title = "C", Status = StatusConsts.New, CurrentHandlerId = userId, ApartmentBuildingId = apartmentBuidlingId, Description = "Desc C", RequestType = "Repair" }
            );
            _dbContext.SaveChanges();

            var request = new RequestQueryBaseDto<Guid> //
            {
                Request = apartmentBuidlingId,
                Page = 1,
                PageSize = 10,
                Filters = new List<FilterQuery>
                {
                    new FilterQuery { Code = "Status", Operator = FilterOperator.Equals, Value = StatusConsts.New }
                }
            };

            _mockAccountService.Setup(x => x.GetAccountInfo())
                          .ReturnsAsync(new AccountInfoResponseDto
                          {
                              Id = userId,
                              UserName = "Nguyen Van A",
                              RoleName = "Technician"
                          });
            var result = await _service.GetRequests(request);

            Assert.That(result.Totals, Is.EqualTo(2));
            Assert.That(result.Items.All(x => x.Status == StatusConsts.New), Is.True);
        }
    }
}