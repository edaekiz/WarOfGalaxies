using Assets.Scripts.Models;
using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class UserPlanetCordinatesDTO
    {
        public int UserPlanetId;
        public int GalaxyIndex;
        public int SolarIndex;
        public int OrderIndex;

        public bool Same(CordinateDTO obj)
        {
            return GalaxyIndex == obj.GalaxyIndex && SolarIndex == obj.SolarIndex && OrderIndex == obj.OrderIndex;
        }

    }
}
