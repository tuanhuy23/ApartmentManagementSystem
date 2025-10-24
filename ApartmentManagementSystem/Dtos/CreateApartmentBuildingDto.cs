namespace ApartmentManagementSystem.Dtos
{
    public class CreateApartmentBuildingDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string CurrencyUnit { get; set; }
        public string ApartmentBuildingImgUrl { get; set; }
        public List<AppartmentBuildingImageDto> Images { get; set; }
        public string ManagementDisplayName { get; set; }
        public string ManagementEmail { get; set; }
        public string ManagementUserName { get; set; }
        public string ManagementPhoneNumber { get; set; }
        public string ManagementPassword { get; set; }
    }
}
