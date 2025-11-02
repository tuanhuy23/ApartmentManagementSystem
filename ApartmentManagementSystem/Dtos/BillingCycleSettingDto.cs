namespace ApartmentManagementSystem.Dtos
{
    public class BillingCycleSettingDto
    {
        public Guid Id { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public int ClosingDayOfMonth { get; set; }
        public int PaymentDueDate { get; set; }
    }
}