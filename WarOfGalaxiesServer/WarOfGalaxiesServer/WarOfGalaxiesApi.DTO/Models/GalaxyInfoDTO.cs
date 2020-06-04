using System.Collections.Generic;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class GalaxyInfoRequestDTO
    {
        public int GalaxyIndex { get; set; }
        public int SolarIndex { get; set; }
    }

    public class GalaxyInfoResponseDTO
    {
        public int GalaxyIndex { get; set; }
        public int SolarIndex { get; set; }
        public List<SolarPlanetDTO> SolarPlanets { get; set; }
        public GalaxyInfoResponseDTO()
        {
            SolarPlanets = new List<SolarPlanetDTO>();
        }
    }

    public class SolarPlanetDTO
    {
        public UserPlanetDTO UserPlanet { get; set; }
        public int OrderIndex { get; set; }
        public double GarbageMetal { get; set; }
        public double GarbageCrystal { get; set; }
        public double GarbageBoron { get; set; }
    }

}
