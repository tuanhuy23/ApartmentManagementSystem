using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Consts;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Services.Impls
{
    internal class RequestService : IRequestService
    {
        private readonly IRequestHistoryRepository _requestHistoryRepository;
        private readonly IUserService _userService;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly IResidentRepository _residentRepository;
        private readonly IFileAttachmentRepository _fileAttachmentRepository;
        public RequestService(IRequestHistoryRepository requestHistoryRepository, IUserService userService, IRequestRepository requestRepository, IUnitOfWork unitOfWork, IAccountService accountService, IResidentRepository residentRepository, IFileAttachmentRepository fileAttachmentRepository)
        {
            _requestHistoryRepository = requestHistoryRepository;
            _userService = userService;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _residentRepository = residentRepository;
            _fileAttachmentRepository = fileAttachmentRepository;
        }

        public async Task CreateOrUpdateRequest(RequestDto request)
        {
            if (request.Id == null)
            {
                await CreateRequest(request);
            }
            else
            {
                await UpdateRequest(request);
            }
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteRequest(List<string> requestId)
        {
            var requestIds = requestId.Select(r => new Guid(r));
            var requestEntites = _requestRepository.List(r => requestIds.Contains(r.Id)).Include(r => r.Files).ToList();
            foreach (var requestEntity in requestEntites)
            {
                var requestFiles = requestEntity.Files;
                if (requestFiles != null)
                {
                    _fileAttachmentRepository.Delete(requestFiles);
                }
            }
            var requestHistorys = _requestHistoryRepository.List(r => requestIds.Contains(r.RequestId)).Include(r => r.Files).ToList();
            foreach (var requestHistory in requestHistorys)
            {
                var requestFiles = requestHistory.Files;
                if (requestFiles != null)
                {
                    _fileAttachmentRepository.Delete(requestFiles);
                }
            }
            _requestRepository.Delete(requestEntites);
            _requestHistoryRepository.Delete(requestHistorys);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateStatusAndAssignRequest(UpdateStatusAndAssignRequestDto request)
        {
            var currentUser = await _accountService.GetAccountInfo();
            var requestEntity = _requestRepository.List().Include(f => f.Files).Include(r => r.RequestHistories).ThenInclude(r => r.Files).FirstOrDefault(r => r.Id.Equals(request.Id));
            if (requestEntity == null)
                throw new DomainException(ErrorCodeConsts.RequestNotFound, ErrorMessageConsts.RequestNotFound, System.Net.HttpStatusCode.NotFound);
            if (currentUser.RoleName.Equals(Consts.RoleDefaulConsts.Resident))
                throw new DomainException(ErrorCodeConsts.NoPermissionUpdateStatusAndAssignRequest, ErrorMessageConsts.NoPermissionUpdateStatusAndAssignRequest, System.Net.HttpStatusCode.Forbidden);
            var newRequestHistory = new List<RequestHistory>();

            if (!string.IsNullOrEmpty(request.Status) && !requestEntity.Status.Equals(request.Status))
            {
                newRequestHistory.Add(new RequestHistory()
                {
                    ApartmentBuildingId = requestEntity.ApartmentBuildingId,
                    ActionType = Consts.ActionType.StatusChange,
                    NewStatus = request.Status,
                    OldStatus = requestEntity.Status,
                    RequestId = request.Id
                });
                requestEntity.Status = request.Status;
            }

            if (!string.IsNullOrEmpty(request.CurrentHandlerId) && ((requestEntity.CurrentHandlerId == null) || (!requestEntity.CurrentHandlerId.Equals(request.CurrentHandlerId))))
            {
                newRequestHistory.Add(new RequestHistory()
                {
                    ApartmentBuildingId = requestEntity.ApartmentBuildingId,
                    ActionType = Consts.ActionType.Assign,
                    NewUserAssignId = request.CurrentHandlerId,
                    RequestId = request.Id
                });
                requestEntity.CurrentHandlerId = request.CurrentHandlerId;
            }
            if (newRequestHistory.Count == 0) return;
            _requestRepository.Update(requestEntity);
            await _requestHistoryRepository.Add(newRequestHistory);
            await _unitOfWork.CommitAsync();
        }
        public async Task<IEnumerable<UserDto>> GetUserHandlers(string apartmentBuidlingId)
        {
            return await _userService.GetAllUsers(apartmentBuidlingId);
        }
        public RequestDto GetRequest(Guid requestId)
        {
            var request = _requestRepository.List().Include(r => r.Files).Include(r => r.RequestHistories).ThenInclude(r => r.Files).FirstOrDefault(r => r.Id.Equals(requestId));
            if (request == null)
                throw new DomainException(ErrorCodeConsts.RequestNotFound, ErrorMessageConsts.RequestNotFound, System.Net.HttpStatusCode.NotFound);
            var requestDto = new RequestDto()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                Description = request.Description,
                Id = request.Id,
                Status = request.Status,
                Title = request.Title,
                CurrentHandlerId = request.CurrentHandlerId,
                RequestType = request.RequestType,
                CreatedDate = request.CreatedDate,
                CreatedUserId = request.CreatedBy,
                CreatedDisplayUser = request.CreatedByUserDisplayName
            };
            if (request.Files == null) return requestDto;
            requestDto.Files = request.Files.Select(r => new FileAttachmentDto()
            {
                Description = r.Description,
                FileType = r.FileType,
                Id = r.Id,
                Name = r.Name,
                Src = r.Src
            });
            if (request.RequestHistories == null) return requestDto;
            var reqHistoryDtos = new List<RequestHistoryDto>();
            foreach (var reqHistory in request.RequestHistories)
            {
                var reqHistoryDto = new RequestHistoryDto()
                {
                    Note = reqHistory.Note,
                    Id = reqHistory.Id,
                    RequestId = reqHistory.RequestId,
                    CreatedDate = reqHistory.CreatedDate,
                    CreatedUserId = reqHistory.CreatedBy,
                    CreatedDisplayUser = reqHistory.CreatedByUserDisplayName
                };
                if (reqHistory.Files != null)
                {
                    reqHistoryDto.Files = reqHistory.Files.Select(f => new FileAttachmentDto()
                    {
                        Description = f.Description,
                        FileType = f.FileType,
                        Id = f.Id,
                        Name = f.Name,
                        Src = f.Src
                    });
                }
                reqHistoryDtos.Add(reqHistoryDto);
            }
            requestDto.RequestHistories = reqHistoryDtos;
            if (request.Files != null)
            {
                requestDto.Files = request.Files.Select(f => new FileAttachmentDto()
                {
                    Description = f.Description,
                    FileType = f.FileType,
                    Id = f.Id,
                    Name = f.Name,
                    Src = f.Src
                });
            }
            return requestDto;
        }

        public async Task<Pagination<RequestDto>> GetRequests(RequestQueryBaseDto<Guid> request)
        {
            IQueryable<Request> requestEntites = null;
            var currentUser = await _accountService.GetAccountInfo();
            if (currentUser.RoleName.Equals(Consts.RoleDefaulConsts.Management))
            {
                requestEntites =  _requestRepository.List().Where(r => r.ApartmentBuildingId.Equals(request.Request));
            }
            else if (currentUser.RoleName.Equals(Consts.RoleDefaulConsts.Resident))
            {
                requestEntites =  _requestRepository.List().Where(r => r.ApartmentBuildingId.Equals(request.Request) && r.CreatedBy.Equals(currentUser.Id));
            }
            else
            {
                requestEntites =  _requestRepository.List().Where(r => r.ApartmentBuildingId.Equals(request.Request) && r.CurrentHandlerId.Equals(currentUser.Id));
            }
           
            var requestDtos = new List<RequestDto>();
            foreach (var requestEntity in requestEntites)
            {
                var requestDto = new RequestDto()
                {
                    ApartmentBuildingId = requestEntity.ApartmentBuildingId,
                    Description = requestEntity.Description,
                    Id = requestEntity.Id,
                    Status = requestEntity.Status,
                    Title = requestEntity.Title,
                    CurrentHandlerId = requestEntity.CurrentHandlerId,
                    CreatedDate = requestEntity.CreatedDate,
                    CreatedUserId = requestEntity.CreatedBy,
                    CreatedDisplayUser = requestEntity.CreatedByUserDisplayName
                };
                requestDtos.Add(requestDto);
            }
            var requestDtoQuery = requestDtos.AsQueryable();
            if (request.Filters != null && request.Filters.Any())
            {
                requestDtoQuery = FilterHelper.ApplyFilters(requestDtoQuery, request.Filters);
            }
            if (request.Sorts != null && request.Sorts.Any())
            {
                requestDtoQuery = SortHelper.ApplySort(requestDtoQuery, request.Sorts);
            }
            return new Pagination<RequestDto>()
            {
                Items = requestDtoQuery.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = requestDtoQuery.Count()
            };
        }

        public async Task CreateOrUpdateRequestAction(RequestHistoryDto request)
        {
            var requestEntity = _requestRepository.List().Include(r => r.RequestHistories).ThenInclude(r => r.Files).FirstOrDefault(r => r.Id.Equals(request.RequestId));
            if (requestEntity == null)
                throw new DomainException(ErrorCodeConsts.RequestNotFound, ErrorMessageConsts.RequestNotFound, System.Net.HttpStatusCode.NotFound);

            if (request.Id == null)
            {
                await CreateRequestHistory(request, requestEntity);
            }
            else
            {
                await UpdateRequestHistory(request, requestEntity);
            }
            await _unitOfWork.CommitAsync();
        }

        public async Task RattingRequest(RattingRequestDto request)
        {
            var requestEntity = _requestRepository.List().FirstOrDefault(r => r.Id.Equals(request.Id));
            if (requestEntity == null)
                throw new DomainException(ErrorCodeConsts.RequestNotFound, ErrorMessageConsts.RequestNotFound, System.Net.HttpStatusCode.NotFound);
            if (!requestEntity.Status.Equals(Consts.StatusConsts.Completed))
                throw new DomainException(ErrorCodeConsts.RequestNotYetCompleted, ErrorMessageConsts.RequestNotYetCompleted, System.Net.HttpStatusCode.BadRequest);
            requestEntity.Rate = request.Ratting;
            _requestRepository.Update(requestEntity);
            await _unitOfWork.CommitAsync();
        }

        private async Task CreateRequest(RequestDto request)
        {
            var currentUser = await _accountService.GetAccountInfo();
            if (!currentUser.RoleName.Equals(Consts.RoleDefaulConsts.Resident))
                throw new DomainException(ErrorCodeConsts.UserNotAllowCreateRequest, ErrorMessageConsts.UserNotAllowCreateRequest, System.Net.HttpStatusCode.Forbidden);

            var requestNew = new Request()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                Status = StatusConsts.New,
                Title = request.Title,
                Description = request.Description,
                RequestType = request.RequestType
            };
            var resident = _residentRepository.List(r => r.UserId.Equals(currentUser.Id)).FirstOrDefault();
            if (resident == null)
                throw new DomainException(ErrorCodeConsts.ResidentNotFound, ErrorMessageConsts.ResidentNotFound, System.Net.HttpStatusCode.NotFound);
            requestNew.ResidentId = resident.Id;
            if (request.Files != null)
            {
                requestNew.Files = request.Files.Select(f => new FileAttachment()
                {
                    Description = f.Description,
                    FileType = f.FileType,
                    Src = f.Src,
                    Name = f.Name
                }).ToList();
            }
            var resultRequestNew = await _requestRepository.Add(requestNew);
            await _requestHistoryRepository.Add(new EF.Context.RequestHistory()
            {
                ActionType = Consts.ActionType.Create,
                ApartmentBuildingId = request.ApartmentBuildingId,
                RequestId = resultRequestNew.Id
            });
        }

        private async Task UpdateRequest(RequestDto request)
        {
            var currentUser = await _accountService.GetAccountInfo();
            if (!currentUser.RoleName.Equals(Consts.RoleDefaulConsts.Resident))
                throw new DomainException(ErrorCodeConsts.UserNotAllowCreateRequest, ErrorMessageConsts.UserNotAllowCreateRequest, System.Net.HttpStatusCode.Forbidden);

            var requestEntity = _requestRepository.List().Include(f => f.Files).FirstOrDefault(r => r.Id.Equals(request.Id));
            if (requestEntity == null)
                throw new DomainException(ErrorCodeConsts.RequestNotFound, ErrorMessageConsts.RequestNotFound, System.Net.HttpStatusCode.NotFound);

            if (!requestEntity.Status.Equals(Consts.StatusConsts.New))
                throw new DomainException(ErrorCodeConsts.RequestAlreadyProcess, ErrorMessageConsts.RequestAlreadyProcess, System.Net.HttpStatusCode.BadRequest);

            requestEntity.Title = request.Title;
            requestEntity.Description = request.Description;
            requestEntity.RequestType = request.RequestType;
            requestEntity.Status = request.Status;
            var files = requestEntity.Files;

            if (request.Files == null)
            {
                request.Files = new List<FileAttachmentDto>();
            }
            foreach (var file in request.Files)
            {
                if (file.Id == null)
                {
                    files.Add(new FileAttachment()
                    {
                        Src = file.Src,
                        Name = file.Name,
                        Description = file.Description,
                        FileType = file.FileType
                    });
                    continue;
                }
                var fileEntity = files.FirstOrDefault(f => f.Id.Equals(file.Id.Value));
                if (fileEntity == null) continue;
                fileEntity.Src = file.Src;
                fileEntity.Name = file.Name;
                fileEntity.Description = file.Description;
                fileEntity.FileType = file.FileType;
            }
            _requestRepository.Update(requestEntity);
        }

        private async Task CreateRequestHistory(RequestHistoryDto request, Request requestEntity)
        {
            var requestHistory = new RequestHistory()
            {
                ApartmentBuildingId = requestEntity.ApartmentBuildingId,
                Note = request.Note,
                RequestId = requestEntity.Id,
                ActionType = Consts.ActionType.Comment
            };
            if (request.Files != null)
            {
                requestHistory.Files = request.Files.Select(f => new FileAttachment()
                {
                    Description = f.Description,
                    FileType = f.FileType,
                    Src = f.Src,
                    Name = f.Name
                }).ToList();
            }
            await _requestHistoryRepository.Add(requestHistory);
        }

        private async Task UpdateRequestHistory(RequestHistoryDto request, Request requestEntity)
        {
            var requestHistoryEntity = _requestHistoryRepository.List().Include(f => f.Files).FirstOrDefault(r => r.Id.Equals(request.Id));

            if (requestHistoryEntity == null)
                throw new DomainException(ErrorCodeConsts.RequestHistoryNotFound, ErrorMessageConsts.RequestHistoryNotFound, System.Net.HttpStatusCode.NotFound);
            requestHistoryEntity.Note = request.Note;

            var files = requestHistoryEntity.Files;

            if (request.Files == null)
            {
                request.Files = new List<FileAttachmentDto>();
            }
            foreach (var file in request.Files)
            {
                if (file.Id == null)
                {
                    files.Add(new FileAttachment()
                    {
                        Src = file.Src,
                        Name = file.Name,
                        Description = file.Description,
                        FileType = file.FileType
                    });
                    continue;
                }
                var fileEntity = files.FirstOrDefault(f => f.Id.Equals(file.Id.Value));
                if (fileEntity == null) continue;
                fileEntity.Src = file.Src;
                fileEntity.Name = file.Name;
                fileEntity.Description = file.Description;
                fileEntity.FileType = file.FileType;
            }
            _requestHistoryRepository.Update(requestHistoryEntity);
        }


    }
}