using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class FeeRateConfig : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public Guid FeeTypeId { get; set; }
        public FeeType FeeType { get; set; }
        public float VATRate {  get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string UnitName{ get; set; }
        public ICollection<FeeTier> FeeTiers { get; set; }
    }
}
