namespace ApartmentManagementSystem.Dtos
{
    public class UpdateApartmentDto
    {
        public Guid Id { get; set; }
        public double Area { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
    }
}