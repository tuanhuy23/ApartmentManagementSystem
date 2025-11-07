using ApartmentManagementSystem.EF.Context.Base;
namespace ApartmentManagementSystem.EF.Context
{
    public class FeeDetailTier : AuditEntity<Guid>
    {
        public Guid FeeDetailId { get; set; }
        public FeeDetail FeeDetail { get; set; }
        public int TierOrder { get; set; }
        public double ConsumptionStart { get; set; }
        public double ConsumptionEnd { get; set; }
        public decimal UnitRate { get; set; }
        public string UnitName { get; set; }
        public double Consumption { get; set; }
        public double ConsumptionStartOriginal { get; set; }
        public double ConsumptionEndOriginal { get; set; }
    }
}
