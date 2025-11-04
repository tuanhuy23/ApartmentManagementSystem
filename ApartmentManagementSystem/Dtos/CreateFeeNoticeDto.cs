namespace ApartmentManagementSystem.Dtos
{
    public class CreateFeeNoticeDto
    {
        public Guid ApartmentId { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public string BillingCycle { get; set; }
        public IEnumerable<Guid> FeeTypeIds{ get; set; }
        public IEnumerable<UtilityReadingDto> UtilityReadings { get; set; }
    }
    public class UtilityReadingDto
    {
        public Guid ApartmentId{ get; set; }
        public Guid FeeTypeId { get; set; }
        public double CurrentReading { get; set; }
        public DateTime ReadingDate { get; set; }
    }
}