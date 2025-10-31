using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class ApartmentBuildingImage : EntityBase<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Src { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
    }
}
