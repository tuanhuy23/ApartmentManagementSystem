using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class FeeDetail : AuditEntity<Guid>
    {
        public Guid FeeNoticeId { get; set; }
        public FeeNotice FeeNotice { get; set; }
        public FeeType FeeType { get; set; }
        public Guid FeeTypeId { get; set; }
        public decimal Consumption { get; set; }
        public decimal SubTotal { get; set; }
        public decimal PreviousReading { get; set; }
        public decimal CurrentReading { get; set; }
    }
}