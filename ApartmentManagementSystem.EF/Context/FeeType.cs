using System.ComponentModel.DataAnnotations;
using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class FeeType : AuditEntity<Guid>
    {
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(25)]
        public string CalculationType { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public bool IsVATApplicable { get; set; }
        public bool IsActive { get; set; } 
        public decimal DefaultRate { get; set; }
        public float DefaultVATRate { get; set; }
        public DateTime? ApplyDate { get; set; }
        public ICollection<FeeRateConfig> FeeRateConfigs { get; set; }
        public ICollection<QuantityRateConfig> QuantityRateConfigs { get; set; }
        public ICollection<UtilityReading> UtilityReadings { get; set; }
        public ICollection<FeeDetail> FeeDetails { get; set; }
    }
}
