namespace ApartmentManagementSystem.Dtos
{
    public class CreateOrUpdateFeeNoticeDto
    {
        public Guid? Id { get; set; }
        public Guid ApartmentId { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public string BillingCycle { get; set; }
        public IEnumerable<Guid> FeeTypeIds{ get; set; }
        public IEnumerable<CreateOrUpdateFeeDetailDto> FeeDetails { get; set; }
    }
    public class CreateOrUpdateFeeDetailDto
    {
        public Guid ApartmentId { get; set; }
        public Guid FeeTypeId { get; set; }
        public int QuantityUseChange { get; set; }
        public UtilityReadingDto? UtilityReading { get; set; }
    }
    public class UtilityReadingDto
    {
         public Guid? UtilityCurentReadingId{ get; set; }
        public double CurrentReading { get; set; }
        public DateTime ReadingDate { get; set; }
    }
}