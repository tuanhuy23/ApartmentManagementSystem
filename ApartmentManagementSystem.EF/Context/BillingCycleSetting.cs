using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class BillingCycleSetting : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public int ClosingDayOfMonth { get; set; }
        public int PaymentDueDate { get; set; }
    }
}
