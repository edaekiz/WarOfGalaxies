using System.Collections.Generic;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class LoginStuffDTO
    {
        public UserDTO UserData { get; set; }
        public List<UserPlanetDTO> UserPlanets { get; set; }
        public List<UserPlanetBuildingDTO> UserPlanetsBuildings { get; set; }
        public List<UserPlanetBuildingUpgDTO> UserPlanetsBuildingsUpgs { get; set; }
        public LoginStuffDTO()
        {
            UserPlanets = new List<UserPlanetDTO>();
            UserPlanetsBuildings = new List<UserPlanetBuildingDTO>();
            UserPlanetsBuildingsUpgs = new List<UserPlanetBuildingUpgDTO>();
        }

    }
}
