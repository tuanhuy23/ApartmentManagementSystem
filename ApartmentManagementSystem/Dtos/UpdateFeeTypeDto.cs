namespace ApartmentManagementSystem.Dtos
{
    public class UpdateFeeTypeDto
    {
        public string Name { get; set; }
        public string CalculationType { get; set; }
        public bool IsVATApplicable { get; set; }
        public decimal DefaultRate { get; set; }
    }
    public class UpdateFeeRateConfigDto
    {
        public Guid Id { get; set; }
        public float VATRate { get; set; }
        public string Name { get; set; }
        public List<UpdateteFeeRateTierDto> FeeTiers { get; set; }
    }
    public class UpdateteFeeRateTierDto
    {
        public Guid Id { get; set; }
        public int TierOrder { get; set; }
        public int ConsumptionStart { get; set; }
        public int ConsumptionEnd { get; set;}
        public decimal UnitRate { get; set; }
        public string UnitName { get; set; }
    }
}