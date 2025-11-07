using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class FeeTier : AuditEntity<Guid>
    {
        public Guid FeeRateConfigId { get; set; }
        public FeeRateConfig FeeRateConfig { get; set; }
        public int TierOrder { get; set; }
        public double ConsumptionStart { get; set; }
        public double ConsumptionEnd { get; set;}
        public decimal UnitRate { get; set; }
        public string UnitName { get; set; }
    }
}
