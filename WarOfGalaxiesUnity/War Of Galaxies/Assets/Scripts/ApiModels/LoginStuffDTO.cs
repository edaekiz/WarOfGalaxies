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
        public List<UserResearchesDTO> UserResearches;
        public List<UserResearchProgDTO> UserResearchProgs;
        public LoginStuffDTO()
        {
            UserPlanets = new List<UserPlanetDTO>();
            UserPlanetsBuildings = new List<UserPlanetBuildingDTO>();
            UserPlanetsBuildingsUpgs = new List<UserPlanetBuildingUpgDTO>();
            UserResearches = new List<UserResearchesDTO>();
            UserResearchProgs = new List<UserResearchProgDTO>();
        }
    }
}
