namespace ApartmentManagementSystem.Dtos
{
    public class CreateOrUpdateFeeTypeDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string CalculationType { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public bool IsVATApplicable { get; set; }
        public decimal DefaultRate { get; set; }
        public float DefaultVATRate { get; set; }
        public bool IsActive { get; set; }
        public List<CreateOrUpdateFeeRateConfigDto> FeeRateConfigs{ get; set; }
        public List<CreateOrUpdateQuantityRateConfigDto> QuantityRateConfigs { get; set; }
    }
    public class CreateOrUpdateFeeRateConfigDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public float VATRate { get; set; }
        public bool IsActive { get; set; }
        public List<CreateOrUpdateFeeRateTierDto> FeeTiers{ get; set; }
    }
    public class CreateOrUpdateFeeRateTierDto
    {
        public Guid? Id{ get; set; }
        public int TierOrder { get; set; }
        public int ConsumptionStart { get; set; }
        public int ConsumptionEnd { get; set;}
        public decimal UnitRate { get; set; }
        public string UnitName { get; set; }
    }
    public class CreateOrUpdateQuantityRateConfigDto
    {
        public Guid? Id { get; set; }
        public bool IsActive { get; set; }
        public string ItemType { get; set; }
        public decimal UnitRate { get; set; }
    }
}