using Assets.Scripts.Enums;
using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class DefenseAddQueueRequestDTO
    {
        public int UserPlanetID;
        public Defenses DefenseID;
        public int Quantity;
    }

    [Serializable]
    public class DefenseAddQueueResponseDTO
    {
        public Defenses DefenseID;
        public int Quantity;
        public ResourcesDTO PlanetResources;
        public int UserPlanetID;
    }

    [Serializable]
    public class UserPlanetDefenseProgDTO
    {
        public int UserPlanetId;
        public Defenses DefenseId;
        public int DefenseCount;
        public double OffsetTime;
        public DateTime LastVerifyDate;
        public UserPlanetDefenseProgDTO()
        {
            LastVerifyDate = DateTime.UtcNow;
        }
    }

    [Serializable]
    public class UserPlanetDefenseDTO
    {
        public int UserPlanetId;
        public Defenses DefenseId;
        public int DefenseCount;
    }
}
