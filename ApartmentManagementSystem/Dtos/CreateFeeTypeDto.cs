namespace ApartmentManagementSystem.Dtos
{
    public class CreateFeeTypeDto
    {
        public string Name { get; set; }
        public string CalculationType { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public bool IsVATApplicable { get; set; }
        public decimal DefaultRate { get; set; }
        public List<CreateFeeRateConfigDto> FeeRateConfigs{ get; set; }
        public List<CreateQuantityRateConfigDto> QuantityRateConfigs { get; set; }
    }
    public class CreateFeeRateConfigDto
    {
        public string Name { get; set; }
        public float VATRate {  get; set; }
        public List<CreateFeeRateTierDto> FeeTiers{ get; set; }
    }
    public class CreateFeeRateTierDto
    {
        public int TierOrder { get; set; }
        public int ConsumptionStart { get; set; }
        public int ConsumptionEnd { get; set;}
        public decimal UnitRate { get; set; }
        public string UnitName { get; set; }
    }
    public class CreateQuantityRateConfigDto
    {
        public float VATRate { get; set; }
        public bool IsActive { get; set; }
        public string ItemType { get; set; }
        public decimal UnitRate { get; set; }
    }
}