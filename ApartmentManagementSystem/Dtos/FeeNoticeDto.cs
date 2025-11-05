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
        public Guid Id { get; set; }
        public Guid FeeNoticeId { get; set; }
        public Guid FeeTypeId { get; set; }
        public double? Consumption { get; set; }
        public decimal SubTotal { get; set; }
        public bool IsFeeTypeActive { get; set; }
        public int QuantityUseChange { get; set; }
        public DateTime? PreviousReadingDate { get; set; }
        public double? PreviousReading { get; set; }
        public DateTime? CurrentReadingDate { get; set; }
        public double? CurrentReading { get; set; }
    }
}
