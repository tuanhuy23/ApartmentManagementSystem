namespace ApartmentManagementSystem.Dtos
{
    public class ApartmentBuildingDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Status { get; set; }
        public string CurrencyUnit { get; set; }
        public string ApartmentBuildingImgUrl { get; set; }
        public IEnumerable<AppartmentBuildingImageDto> Images { get; set; }
    }
}
