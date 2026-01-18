using System.ComponentModel.DataAnnotations;
using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class FeeNotice : AuditEntity<Guid>
    {
        public Guid ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        [MaxLength(50)]
        public string BillingCycle { get; set; }
        [MaxLength(25)]
        public string Status { get; set; }
        [MaxLength(25)]
        public string PaymentStatus { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public ICollection<FeeDetail> FeeDetails { get; set; }
    }
}