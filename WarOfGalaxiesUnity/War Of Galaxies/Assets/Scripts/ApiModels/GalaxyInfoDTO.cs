using System;
using System.Collections.Generic;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class GalaxyInfoRequestDTO
    {
        public int GalaxyIndex;
        public int SolarIndex;
    }

    [Serializable]
    public class GalaxyInfoResponseDTO
    {
        public int GalaxyIndex;
        public int SolarIndex;
        public List<SolarPlanetDTO> SolarPlanets;
        public GalaxyInfoResponseDTO()
        {
            SolarPlanets = new List<SolarPlanetDTO>();
        }
    }

    [Serializable]
    public class SolarPlanetDTO
    {
        public UserPlanetDTO UserPlanet;
        public int OrderIndex;
    }
}
