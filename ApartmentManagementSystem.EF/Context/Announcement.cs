using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class Announcement : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public DateTime PublishDate { get; set; }
        
    }
}