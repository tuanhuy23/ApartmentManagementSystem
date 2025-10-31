using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class Apartment : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public double Area {  get; set; }
        public ICollection<ParkingRegistration> ParkingRegistrations { get; set; }

    }
}
