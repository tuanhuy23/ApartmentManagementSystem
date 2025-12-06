using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class QuantityRateConfig : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public Guid FeeTypeId { get; set; }
        public FeeType FeeType { get; set; }
        public string ItemType { get; set; }
        public decimal UnitRate {  get; set; }
        public bool IsActive { get; set; }
         public DateTime ApplyDate{ get; set; }
    }
}
