namespace ApartmentManagementSystem.Dtos
{
    public class ApartmentDto
    {
        public Guid? Id { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public double Area { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
    }
}