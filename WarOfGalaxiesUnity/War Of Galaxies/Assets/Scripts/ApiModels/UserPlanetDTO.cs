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
        public long Metal;
        public long Crystal;
        public long Boron;
        public DateTime LastUpdateDate;
    }
}
