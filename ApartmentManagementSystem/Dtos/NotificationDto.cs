using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Dtos
{
    public class NotificationDto
    {
        public Guid? Id {get;set;}
        public Guid ApartmentBuildingId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public Guid RelatedEntityID { get; set; }
        public string RelatedEntityType { get; set; }
        public bool IsRead { get; set; }
    }
}