namespace WarOfGalaxiesApi.DTO.Models
{
    public class UserPlanetBuildingDTO
    {
        public int UserPlanetId { get; set; }
        public int BuildingId { get; set; }
        public int BuildingLevel { get; set; }
    }

    public class UserPlanetBuildingUpgDTO
    {
        public ResourcesDTO PlanetResources { get; set; }
        public int UserPlanetId { get; set; }
        public int BuildingId { get; set; }
        public int BuildingLevel { get; set; }
        public double LeftTime { get; set; }
    }

    public class UserPlanetUpgradeBuildingDTO
    {
        public int UserPlanetID { get; set; }
        public int BuildingID { get; set; }
    }

}
