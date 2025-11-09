namespace ApartmentManagementSystem.Dtos
{
    public class UtilityReadingDto
    {
        public Guid Id { get; set; }
        public Guid ApartmentId { get; set; }
        public Guid FeeTypeId { get; set; }
        public string FeeTypeName { get; set; }
        public double CurrentReading { get; set; }
        public DateTime ReadingDate { get; set; }
    }
}