using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class UtilityReading : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public Guid ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        public FeeType FeeType { get; set; }
        public Guid FeeTypeId { get; set; }
        public decimal CurrentReading { get; set; }
    }
}
