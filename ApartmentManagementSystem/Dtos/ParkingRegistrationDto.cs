namespace ApartmentManagementSystem.Dtos
{
    public class ParkingRegistrationDto
    {
        public Guid Id { get; set; }
        public Guid ApartmentId { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public string VehicleType { get; set; }
    }
}