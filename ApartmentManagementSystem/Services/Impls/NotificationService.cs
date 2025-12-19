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
using Org.BouncyCastle.Ocsp;

namespace ApartmentManagementSystem.Services.Impls
{
    internal class NotificationService : INotificationService
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IApartmentBuildingRepository _apartmentBuildingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IFileAttachmentRepository _fileAttachmentRepository;
        public NotificationService(IAnnouncementRepository announcementRepository, IUnitOfWork unitOfWork, IApartmentBuildingRepository apartmentBuildingRepository, IAccountService accountService,
         IApartmentRepository apartmentRepository, IFileAttachmentRepository fileAttachmentRepository)
        {
            _announcementRepository = announcementRepository;
            _unitOfWork = unitOfWork;
            _apartmentBuildingRepository = apartmentBuildingRepository;
            _accountService = accountService;
            _apartmentRepository = apartmentRepository;
            _fileAttachmentRepository = fileAttachmentRepository;
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

        public async Task DeleteAnnouncements(List<string> ids)
        {
            var announcementIds = ids.Select(r => new Guid(r));
            var announcementEntites = _announcementRepository.List(r => announcementIds.Contains(r.Id)).Include(r => r.Files).ToList();
            foreach (var announcementEntity in announcementEntites)
            {
                var announcementFiles = announcementEntity.Files;
                if (announcementFiles != null)
                {
                    _fileAttachmentRepository.Delete(announcementFiles);
                }
            }

            _announcementRepository.Delete(announcementEntites);
            await _unitOfWork.CommitAsync();
        }

        public byte[] DownloadExcelTemplate(string fileName, string sheetName)
        {
            string jsonData = @"{
                            'header': [
                                {'ApartmentName': 'ApartmentName'}";
            jsonData += @"],
                        'body': []
                        }";
            return ExcelUtilityHelper.ExportToExcel(fileName, sheetName, jsonData);
        }

        public async Task<IEnumerable<ApartmentAnnouncementDto>> ImportApartmentIdResult(string apartmentBuildingId, IFormFile file)
        {
            ExcelUtilityHelper.ExcelData jsonData = ExcelUtilityHelper.ImportFromExcel(file);
            var apartmentNames = new List<string>();
            foreach (var row in jsonData.body)
            {
                var colApartmentName = row["ApartmentName"].ToString();
                if (string.IsNullOrEmpty(colApartmentName)) continue;
                apartmentNames.Add(colApartmentName);
            }
            var apartments = _apartmentRepository.List(a => a.ApartmentBuildingId.Equals(new Guid(apartmentBuildingId)) && apartmentNames.Contains(a.Name)).Select(a => new ApartmentAnnouncementDto()
            {
                Id = a.Id,
                Name = a.Name
            });
            return apartments.ToList();
        }

        public IEnumerable<ApartmentAnnouncementDto> GetApartmentData(string apartmentBuildingId)
        {
            var apartments = _apartmentRepository.List(a => a.ApartmentBuildingId.Equals(new Guid(apartmentBuildingId))).Select(a => new ApartmentAnnouncementDto()
            {
                Id = a.Id,
                Name = a.Name
            });
            return apartments.ToList();
        }

        public AnnouncementDto GetAnnouncement(Guid id)
        {
            var announcement = _announcementRepository.List(r => r.Id.Equals(id)).Include(r => r.Files).FirstOrDefault();
            if (announcement == null)
                throw new DomainException(ErrorCodeConsts.AnnouncementNotFound, ErrorMessageConsts.AnnouncementNotFound, System.Net.HttpStatusCode.NotFound);
            var announcementDto = new AnnouncementDto()
            {
                ApartmentBuildingId = announcement.ApartmentBuildingId,
                Body = announcement.Body,
                Id = announcement.Id,
                IsAll = announcement.IsAll,
                PublishDate = announcement.PublishDate,
                Status = announcement.Status,
                Title = announcement.Title
            };
            if (announcement.ApartmentIds != null)
            {
                var apartments = _apartmentRepository.List(a => announcement.ApartmentIds.Contains(a.Id.ToString())).Select(a => new ApartmentAnnouncementDto()
                {
                    Id = a.Id,
                    Name = a.Name
                });
                announcementDto.ApartmentIds = announcement.ApartmentIds.Select(a => new Guid(a));
                announcementDto.Apartments = apartments.ToList();
            }
            if (announcement.Files == null) return announcementDto;
            announcementDto.Files = announcement.Files.Select(r => new FileAttachmentDto()
            {
                Description = r.Description,
                FileType = r.FileType,
                Id = r.Id,
                Name = r.Name,
                Src = r.Src
            });
            return announcementDto;
        }

        public async Task<Pagination<AnnouncementDto>> GetAnnouncements(RequestQueryBaseDto<Guid> request)
        {
            List<Announcement> announcements = new List<Announcement>();
            var currentUser = await _accountService.GetAccountInfo();
            if (currentUser.RoleName.Equals(Consts.RoleDefaulConsts.Management))
            {
                announcements = _announcementRepository.List().Where(r => r.ApartmentBuildingId.Equals(request.Request)).ToList();
            }
            else if (currentUser.RoleName.Equals(Consts.RoleDefaulConsts.Resident))
            {
                var now = DateTime.UtcNow;
                var announcementsData = _announcementRepository.List(r => r.ApartmentBuildingId.Equals(request.Request)
                                                                        && now >= r.PublishDate && r.Status.Equals(StatusConsts.Publish));
                foreach (var announcement in announcementsData)
                {
                    if (announcement.IsAll)
                    {
                        announcements.Add(announcement);
                    }
                    else if (announcement.ApartmentIds == null)continue;
                    if (announcement.ApartmentIds.Contains(currentUser.ApartmentId))
                    {
                        announcements.Add(announcement);
                    }
                }
            }

            var data = announcements.Select(a => new AnnouncementDto()
            {
                Id = a.Id,
                ApartmentBuildingId = a.ApartmentBuildingId,
                Title = a.Title,
                Status = a.Status,
                IsAll = a.IsAll,
                Body = a.Body,
                PublishDate = a.PublishDate
            }).AsQueryable();
            if (request.Filters != null && request.Filters.Any())
            {
                data = FilterHelper.ApplyFilters(data, request.Filters);
            }
            if (request.Sorts != null && request.Sorts.Any())
            {
                data = SortHelper.ApplySort(data, request.Sorts);
            }
            return new Pagination<AnnouncementDto>()
            {
                Items = data.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = data.Count()
            };
        }

        private async Task CreateAnnouncement(AnnouncementDto request)
        {
            var announcementNew = new Announcement()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                Status = StatusConsts.UnPublish,
                Title = request.Title,
                Body = request.Body,
                IsAll = request.IsAll,
                PublishDate = DateTime.SpecifyKind(request.PublishDate, DateTimeKind.Utc),
            };
            if (request.ApartmentIds != null)
            {
                announcementNew.ApartmentIds = request.ApartmentIds.Select(a => a.ToString());
            }
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
            announcementEntity.PublishDate = DateTime.SpecifyKind(request.PublishDate, DateTimeKind.Utc);
            announcementEntity.Status = request.Status;
            var files = announcementEntity.Files;

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
            _announcementRepository.Update(announcementEntity);
        }
    }
}