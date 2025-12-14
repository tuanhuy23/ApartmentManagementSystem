namespace ApartmentManagementSystem.Dtos
{
    public class FeeNoticeDto
    {
        public Guid Id { get; set; }
        public Guid ApartmentId { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public string BillingCycle { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public IEnumerable<FeeDetailDto> FeeDetails { get; set; }
    }

    public class FeeDetailDto
    {
        public Guid FeeNoticeId { get; set; }
        public Guid FeeTypeId { get; set; }
        public double? Consumption { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrossCost { get; set; }
        public float VATRate { get; set; }
        public decimal VATCost { get; set; }
        public DateTime? PreviousReadingDate { get; set; }
        public double? PreviousReading { get; set; }
        public DateTime? CurrentReadingDate { get; set; }
        public double? CurrentReading { get; set; }
        public Guid? UtilityCurentReadingId{ get; set; }
        public double? Proration { get; set; }
        public IEnumerable<FeeTierDetail>? FeeTierDetails { get; set; }
    }
    public class FeeTierDetail
    {
        public int TierOrder { get; set; }
        public double ConsumptionStart { get; set; }
        public double ConsumptionEnd { get; set; }
        public double ConsumptionStartOriginal { get; set; }
        public double ConsumptionEndOriginal { get; set; }
        public decimal UnitRate { get; set; }
        public string UnitName { get; set; }
        public double Consumption { get; set; }
    }
    public class ImportFeeNoticeResult
    {
        public string ApartmentName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
