using System;
using System.Collections.Generic;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class LoginStuffDTO
    {
        public UserDTO UserData;
        public List<UserPlanetDTO> UserPlanets;
        public List<UserPlanetBuildingDTO> UserPlanetsBuildings;
        public List<UserPlanetBuildingUpgDTO> UserPlanetsBuildingsUpgs;
        public LoginStuffDTO()
        {
            UserPlanets = new List<UserPlanetDTO>();
            UserPlanetsBuildings = new List<UserPlanetBuildingDTO>();
            UserPlanetsBuildingsUpgs = new List<UserPlanetBuildingUpgDTO>();
        }
    }
}
