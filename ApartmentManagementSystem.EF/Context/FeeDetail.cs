using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class FeeDetail : AuditEntity<Guid>
    {
        public Guid FeeNoticeId { get; set; }
        public FeeNotice FeeNotice { get; set; }
        public FeeType FeeType { get; set; }
        public Guid FeeTypeId { get; set; }
        public double? Consumption { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrossCost { get; set; }
        public float VATRate { get; set; }
        public decimal VATCost { get; set; }
        public DateTime? PreviousReadingDate{ get; set; }
        public double? PreviousReading { get; set; }
        public DateTime? CurrentReadingDate{ get; set; }
        public double? CurrentReading { get; set; }
        public double? Proration { get; set; }
        public ICollection<FeeDetailTier>? FeeDetailTiers { get; set; }
    }
}