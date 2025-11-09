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
        public float DefaultVATRate { get; set; }
        public IEnumerable<FeeRateConfigDto> FeeRateConfigs { get; set; }
        public IEnumerable<QuantityRateConfigDto> QuantityRateConfigs{ get; set; }
    }
    public class FeeRateConfigDto
    {
        public Guid Id { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public Guid FeeTypeId { get; set; }
        public float VATRate { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string UnitName{ get; set; }
        public IEnumerable<FeeTierDto> FeeTiers { get; set; }
    }
    public class FeeTierDto
    {
        public Guid Id { get; set; }
        public Guid FeeRateConfigId { get; set; }
        public int TierOrder { get; set; }
        public double ConsumptionStart { get; set; }
        public double ConsumptionEnd { get; set;}
        public decimal UnitRate { get; set; }
        public string UnitName { get; set; }
    }

    public class QuantityRateConfigDto
    {
        public Guid Id { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public Guid FeeTypeId { get; set; }
        public bool IsActive { get; set; }
        public string ItemType { get; set; }
        public decimal UnitRate { get; set; }
    }
}