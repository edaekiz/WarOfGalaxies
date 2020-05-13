using System.Collections.Generic;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class LoginStuffDTO
    {
        public UserDTO UserData { get; set; }
        public List<UserPlanetDTO> UserPlanets { get; set; }
        public List<UserPlanetBuildingDTO> UserPlanetsBuildings { get; set; }
        public List<UserPlanetBuildingUpgDTO> UserPlanetsBuildingsUpgs { get; set; }
        public List<UserResearchesDTO> UserResearches { get; set; }
        public List<UserResearchProgDTO> UserResearchProgs { get; set; }
        public List<UserPlanetShipProgDTO> UserPlanetShipProgs { get; set; }
        public List<UserPlanetShipDTO> UserPlanetShips { get; set; }
        public List<UserPlanetDefenseProgDTO> UserPlanetDefenseProgs { get; set; }
        public List<UserPlanetDefenseDTO> UserPlanetDefenses { get; set; }
        public List<UserPlanetCordinatesDTO> UserPlanetCordinates { get; set; }
        public LoginStuffDTO()
        {
            UserPlanetCordinates = new List<UserPlanetCordinatesDTO>();
            UserPlanets = new List<UserPlanetDTO>();
            UserPlanetsBuildings = new List<UserPlanetBuildingDTO>();
            UserPlanetsBuildingsUpgs = new List<UserPlanetBuildingUpgDTO>();
            UserResearches = new List<UserResearchesDTO>();
            UserResearchProgs = new List<UserResearchProgDTO>();
            UserPlanetShipProgs = new List<UserPlanetShipProgDTO>(); 
            UserPlanetShips = new List<UserPlanetShipDTO>();
            UserPlanetDefenseProgs = new List<UserPlanetDefenseProgDTO>();
            UserPlanetDefenses = new List<UserPlanetDefenseDTO>();
        }

    }
}
