using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class Apartment : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
        public double Area { get; set; }
        public string Building { get; set; }
        public ICollection<ParkingRegistration> ParkingRegistrations { get; set; }
        public ICollection<UtilityReading> UtilityReadings { get; set; }
        public ICollection<FeeNotice> FeeNotices { get; set; }
        public ICollection<ApartmentMember> ApartmentMembers { get; set; }
    }
}
