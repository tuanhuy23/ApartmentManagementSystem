using ApartmentManagementSystem.Consts;
using ApartmentManagementSystem.Dtos;
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
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IUserService _userService;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApartmentBuildingRepository _apartmentBuildingRepository;
        private readonly IAccountService _accountService;
        public RequestService (IFeedbackRepository feedbackRepository, IUserService userService, IRequestRepository requestRepository, IUnitOfWork unitOfWork, IApartmentBuildingRepository apartmentBuildingRepository, IAccountService accountService)
        {
            _feedbackRepository = feedbackRepository;
            _userService = userService;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _apartmentBuildingRepository = apartmentBuildingRepository;
            _accountService = accountService;
        }
        public async Task CreateOrUpdateFeedback(FeedbackDto request)
        {
            var requestEntity = _requestRepository.List().FirstOrDefault(a => a.Id.Equals(request.RequestId));
            if (requestEntity == null)
                throw new DomainException(ErrorCodeConsts.RequestNotFound, ErrorMessageConsts.RequestNotFound, System.Net.HttpStatusCode.NotFound); 
            if (request.Id == null)
            {
                await CreateFeedback(request, requestEntity);
            }
            else
            {
                await UpdateFeedback(request, requestEntity);
            }
            await _unitOfWork.CommitAsync();
        }

        public async Task CreateOrUpdateRequest(RequestDto request)
        {
            var apartmentBuilding = _apartmentBuildingRepository.List().FirstOrDefault(a => a.Id.Equals(request.ApartmentBuildingId));
            if (apartmentBuilding == null)
                throw new DomainException(ErrorCodeConsts.ApartmentBuildingNotFound, ErrorMessageConsts.ApartmentBuildingNotFound, System.Net.HttpStatusCode.NotFound);
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

        public Task DeleteFeedback(Guid feedbackId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public RequestDto GetRequest(Guid requestId)
        {
            var request = _requestRepository.List().Include(r => r.Files).Include(r => r.Feedbacks).ThenInclude(r => r.Files).FirstOrDefault(r => r.Id.Equals(requestId));
            if (request == null) 
                throw new DomainException(ErrorCodeConsts.RequestNotFound, ErrorMessageConsts.RequestNotFound, System.Net.HttpStatusCode.NotFound);
            var requestDto = new RequestDto()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                Description = request.Description,
                Id = request.Id,
                Status = request.Status,
                Title = request.Title,
                UserId = request.UserId
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
            if (request.Feedbacks == null) return requestDto;
            var feedbackDtos = new List<FeedbackDto>();
            foreach(var feedback in request.Feedbacks)
            {
                var feedbackDto = new FeedbackDto()
                {
                    Description = feedback.Description,
                    Id = feedback.Id,
                    Rate = feedback.Rate,
                    RequestId = feedback.RequestId
                };
                if (feedback.Files != null)
                {
                    feedbackDto.Files = feedback.Files.Select(f => new FileAttachmentDto()
                    {
                        Description = f.Description,
                        FileType = f.FileType,
                        Id = f.Id,
                        Name = f.Name,
                        Src = f.Src
                    });
                }
                feedbackDtos.Add(feedbackDto);
            }
            requestDto.Feedbacks = feedbackDtos;
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

        public IEnumerable<RequestDto> GetRequests(Guid apartmentBuildingId)
        {
            var requests = _requestRepository.List().Where(r => r.ApartmentBuildingId.Equals(apartmentBuildingId));
            var requestDtos = new List<RequestDto>();
            foreach(var request in requests)
            {
                var requestDto = new RequestDto()
                {
                    ApartmentBuildingId = request.ApartmentBuildingId,
                    Description = request.Description,
                    Id = request.Id,
                    Status = request.Status,
                    Title = request.Title,
                    UserId = request.UserId
                };
                requestDtos.Add(requestDto);
            }
            return requestDtos;
        }
        private async Task CreateRequest(RequestDto request)
        {
            var requestNew = new Request()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                Status = StatusConsts.New,
                Title = request.Title,
                Description = request.Description,
                UserId = request.UserId
            };
            if (!string.IsNullOrEmpty(request.UserId))
            {
                var user = _userService.GetUser(request.UserId);
                if (user == null)
                    throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            }
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
            await _requestRepository.Add(requestNew);
        }

        private async Task UpdateRequest(RequestDto request)
        {
            var requestEntity = _requestRepository.List().Include(f => f.Files).FirstOrDefault(r => r.Id.Equals(request.Id));
            if (requestEntity == null)
                throw new DomainException(ErrorCodeConsts.RequestNotFound, ErrorMessageConsts.RequestNotFound, System.Net.HttpStatusCode.NotFound);
            if (!requestEntity.Status.Equals(StatusConsts.Done))
                throw new DomainException(ErrorCodeConsts.RequestAlreadyDone, ErrorMessageConsts.RequestAlreadyDone, System.Net.HttpStatusCode.BadRequest);
            requestEntity.Title = request.Title;
            requestEntity.Description = request.Description;
            requestEntity.Status = request.Status;
            if (!string.IsNullOrEmpty(request.UserId))
            {
                var user = _userService.GetUser(request.UserId);
                if (user == null)
                    throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            }
            requestEntity.UserId = request.UserId;
            var files = requestEntity.Files;

            if (request.Files == null)
            {
                request.Files = new List<FileAttachmentDto>();
            }
            foreach(var file in request.Files)
            {
                if(file.Id == null)
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
        private async Task CreateFeedback(FeedbackDto request, Request requestEntity)
        {
            if (!requestEntity.Status.Equals(StatusConsts.Done))
                throw new DomainException(ErrorCodeConsts.RequestNotCompleted, ErrorMessageConsts.RequestNotCompleted, System.Net.HttpStatusCode.BadRequest);
            var feedback = new Feedback()
            {
                ApartmentBuildingId = requestEntity.ApartmentBuildingId,
                Rate = request.Rate,
                Description = request.Description
            };
            if (request.Files != null)
            {
                feedback.Files = request.Files.Select(f => new FileAttachment()
                {
                    Description = f.Description,
                    FileType = f.FileType,
                    Src = f.Src,
                    Name = f.Name
                }).ToList();
            }
            await _feedbackRepository.Add(feedback);
        }

        private async Task UpdateFeedback(FeedbackDto request, Request requestEntity)
        {
            if (!requestEntity.Status.Equals(StatusConsts.Done))
                throw new DomainException(ErrorCodeConsts.RequestNotCompleted, ErrorMessageConsts.RequestNotCompleted, System.Net.HttpStatusCode.BadRequest);
            var feedback = _feedbackRepository.List().Include(f => f.Files).FirstOrDefault(r => r.Id.Equals(request.Id));
            if (feedback == null)
                throw new DomainException(ErrorCodeConsts.FeedbackNotFound, ErrorMessageConsts.FeedbackNotFound, System.Net.HttpStatusCode.NotFound);
            feedback.Description = request.Description;
            feedback.Rate = request.Rate;
           
            var files = feedback.Files;

            if (request.Files == null)
            {
                request.Files = new List<FileAttachmentDto>();
            }
            foreach(var file in request.Files)
            {
                if(file.Id == null)
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
            _feedbackRepository.Update(feedback);
        }
    }
}