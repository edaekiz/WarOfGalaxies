using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class UserPlanetDTO
    {
        public int UserPlanetId;
        public int UserId;
        public string PlanetCordinate;
        public int PlanetType;
        public string PlanetName;
        public int Metal;
        public int Crystal;
        public int Boron;
        public DateTime LastUpdateDate;
    }
}
