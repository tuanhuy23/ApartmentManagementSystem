using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class UtilityReading : AuditEntity<Guid>
    {
        public Guid ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        public FeeType FeeType { get; set; }
        public Guid FeeTypeId { get; set; }
        public double CurrentReading { get; set; }
        public DateTime? ReadingDate { get; set; }
        public Guid ApartmentBuildingId { get; set; }
    }
}
