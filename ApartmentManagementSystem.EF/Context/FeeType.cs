using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class FeeType : AuditEntity<Guid>
    {
        public string Name { get; set; }
        public string CalculationType { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public bool IsVATApplicable { get; set; }
        public bool IsActive { get; set; } 
        public decimal DefaultRate { get; set; }
        public float DefaultVATRate { get; set; }
        public ICollection<FeeRateConfig> FeeRateConfigs { get; set; }
        public ICollection<QuantityRateConfig> QuantityRateConfigs { get; set; }
        public ICollection<UtilityReading> UtilityReadings { get; set; }
        public ICollection<FeeDetail> FeeDetails { get; set; }
    }
    public static class CalculationType
    {
        public const string Area = "AREA";
        public const string QUANTITY = "QUANTITY";
        public const string TIERED = "TIERED";
    }
}
