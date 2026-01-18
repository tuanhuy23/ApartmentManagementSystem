using System.ComponentModel.DataAnnotations;
using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class ParkingRegistration : AuditEntity<Guid>
    {
        public Guid ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        [MaxLength(100)]
        public string VehicleType { get; set; }
        [MaxLength(100)]
        public string? VehicleNumber { get; set; }
        [MaxLength(255)]
        public string VehicleDescription{ get; set; }
    }
}
