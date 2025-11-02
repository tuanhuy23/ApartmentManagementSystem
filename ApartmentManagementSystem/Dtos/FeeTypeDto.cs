namespace ApartmentManagementSystem.Dtos
{
    public class FeeTypeDto
    {
        public Guid Id{ get; set; }
        public string Name { get; set; }
        public string CalculationType { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public bool IsVATApplicable { get; set; }
        public bool IsActive { get; set; }
        public decimal DefaultRate { get; set; }
        public string FeeRateConfigIdApplicable { get; set; }
    }
    public class FeeRateConfigDto
    {
        public Guid Id { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public Guid FeeTypeId { get; set; }
        public float VATRate { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public IEnumerable<FeeTierDto> FeeTiers { get; set; }
    }
    public class FeeTierDto
    {
        public Guid Id { get; set; }
        public Guid FeeRateConfigId { get; set; }
        public int TierOrder { get; set; }
        public int ConsumptionStart { get; set; }
        public int ConsumptionEnd { get; set;}
        public decimal UnitRate { get; set; }
        public string UnitName { get; set; }
    }
}