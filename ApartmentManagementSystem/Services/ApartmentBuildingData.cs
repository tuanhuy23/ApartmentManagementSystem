using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services
{
    class ApartmentBuildingData
    {
        public Dictionary<string, ApartmentBuildingDto> ApartmentBuildings { get; set; }
        public object obj = new object();
        public ApartmentBuildingData()
        {
            ApartmentBuildings = new Dictionary<string, ApartmentBuildingDto>();
        }
        public void AddApartmentBuilding(ApartmentBuildingDto data)
        {
            lock (obj)
            {
                ApartmentBuildings.TryAdd(data.Id, data);
            }
        }
        public void RemoveApartmentBuilding(List<string> ids)
        {
            lock (obj)
            {
                foreach(var id in ids)
                {
                    ApartmentBuildings.Remove(id);
                }
            }
        }
        public bool CheckIsExist(string id)
        {
           lock (obj)
            {
                return ApartmentBuildings.TryGetValue(id, out ApartmentBuildingDto data);
            } 
        }
    }
}