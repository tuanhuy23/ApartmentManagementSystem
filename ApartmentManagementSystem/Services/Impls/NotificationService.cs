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
    internal class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IApartmentBuildingRepository _apartmentBuildingRepository;
        private readonly IUnitOfWork _unitOfWork;
        public NotificationService(INotificationRepository notificationRepository, IAnnouncementRepository announcementRepository, IUnitOfWork unitOfWork, IApartmentBuildingRepository apartmentBuildingRepository)
        {
            _notificationRepository = notificationRepository;
            _announcementRepository = announcementRepository;
            _unitOfWork = unitOfWork;
            _apartmentBuildingRepository = apartmentBuildingRepository;
        }
        public async Task CreateNotification(CreateNotificationDto request)
        {
            var notification = new Notification()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                IsRead = false,
                RelatedEntityID = request.RelatedEntityID,
                Title = request.Title,
                UserId = request.UserId
            };
            await _notificationRepository.Add(notification);
            await _unitOfWork.CommitAsync();
        }

        public async Task CreateOrUpdateAnnouncements(AnnouncementDto request)
        {
            if (request.Id == null)
            {
                await CreateAnnouncement(request);
            }
            else
            {
                await UpdateAnnouncement(request);
            }
            await _unitOfWork.CommitAsync();
        }

        public Task DeleteAnnouncements(List<string> ids)
        {
            throw new NotImplementedException();
        }

        public Task DeleteNotification(List<DeleteNotificationDto> request)
        {
            throw new NotImplementedException();
        }

        public AnnouncementDto GetAnnouncement(Guid id)
        {
            throw new NotImplementedException();
        }

        public Pagination<AnnouncementDto> GetAnnouncements(RequestQueryBaseDto<Guid> request)
        {
            var announcements = _announcementRepository.List().Where(a => a.ApartmentBuildingId.Equals(request.Request)).Select(a => new AnnouncementDto()
            {
                ApartmentBuildingId = a.ApartmentBuildingId,
                Title = a.Title,
                Status = a.Status,
                IsAll = a.IsAll,
                Body = a.Body
            });
            if (request.Filters!= null && request.Filters.Any())
            {
                announcements = FilterHelper.ApplyFilters(announcements, request.Filters);
            }
            if (request.Sorts!= null && request.Sorts.Any())
            {
                announcements = SortHelper.ApplySort(announcements, request.Sorts);
            }
            return new Pagination<AnnouncementDto>()
            {
                Items = announcements.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = announcements.Count()
            };
        }

        public async Task<IEnumerable<NotificationDto>> GetNotifications(string userId)
        {
            return null; 
        }

        public Task MarkNotificationIsRead(List<string> ids)
        {
            throw new NotImplementedException();
        }

        private async Task CreateAnnouncement(AnnouncementDto request)
        {
            var announcementNew = new Announcement()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                Status = StatusConsts.UnPublish,
                Title = request.Title,
                Body = request.Body,
                IsAll = request.IsAll
            };
            
            if (request.Files != null)
            {
                announcementNew.Files = request.Files.Select(f => new FileAttachment()
                {
                    Description = f.Description,
                    FileType = f.FileType,
                    Src = f.Src,
                    Name = f.Name
                }).ToList();
            }
            await _announcementRepository.Add(announcementNew);
        }

        private async Task UpdateAnnouncement(AnnouncementDto request)
        {
            var announcementEntity = _announcementRepository.List().Include(f => f.Files).FirstOrDefault(r => r.Id.Equals(request.Id));
            if (announcementEntity == null)
                throw new DomainException(ErrorCodeConsts.AnnouncementNotFound, ErrorMessageConsts.AnnouncementNotFound, System.Net.HttpStatusCode.NotFound);
            if (!announcementEntity.Status.Equals(StatusConsts.UnPublish))
                throw new DomainException(ErrorCodeConsts.AnnouncementAlreadyDone, ErrorMessageConsts.AnnouncementAlreadyDone, System.Net.HttpStatusCode.BadRequest);
            announcementEntity.Title = request.Title;
            announcementEntity.Body = request.Body;
            announcementEntity.IsAll = request.IsAll;
           
            var files = announcementEntity.Files;

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
            _announcementRepository.Update(announcementEntity);
        }
    }
}