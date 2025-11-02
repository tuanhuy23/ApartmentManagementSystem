using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class FeeNotice : AuditEntity<Guid>
    {
        public Guid ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        public string BillingCycle { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public ICollection<FeeDetail> FeeDetails { get; set; }
    }
    public static class FeeNoticeStatus
    {
        public const string Draft = "DRAFT";
        public const string Issued = "ISSUED";
        public const string Paid = "PAID";
        public const string Canceled = "CANCELED";
    }
}