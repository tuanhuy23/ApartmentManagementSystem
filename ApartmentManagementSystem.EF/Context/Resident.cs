using System.ComponentModel.DataAnnotations;
using ApartmentManagementSystem.EF.Context.Base;


namespace ApartmentManagementSystem.EF.Context
{
    public class Resident : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        public DateTime? BrithDay { get; set; }
        [MaxLength(10)]
        public string? IdentityNumber { get; set; }
        public string? UserId { get; set; }
        [MaxLength(50)]
        public string? PhoneNumber  {get; set; }
        public ICollection<ApartmentResident> ApartmentResidents { get; set; }
    }
}
