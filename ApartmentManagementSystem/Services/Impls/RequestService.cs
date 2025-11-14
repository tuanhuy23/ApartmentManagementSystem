using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Services.Interfaces;

namespace ApartmentManagementSystem.Services.Impls
{
    internal class RequestService : IRequestService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IUserService _userService;
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RequestService (IFeedbackRepository feedbackRepository, IUserService userService, IRequestRepository requestRepository, IUnitOfWork unitOfWork)
        {
            _feedbackRepository = feedbackRepository;
            _userService = userService;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
        }
        public Task CreateOrUpdateFeedback(FeedbackDto request)
        {
            throw new NotImplementedException();
        }

        public Task CreateOrUpdateRequest(RequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFeedback(Guid feedbackId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public RequestDto GetRequest()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RequestDto> GetRequests()
        {
            throw new NotImplementedException();
        }
    }
}