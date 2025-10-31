using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class ParkingRegistration : AuditEntity<Guid>
    {
        public Guid ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public string VehicleType { get; set; }
    }
}
