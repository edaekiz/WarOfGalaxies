using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class ResourcesDTO
    {
        public int UserPlanetID;
        public double Metal;
        public double Crystal;
        public double Boron;
        public ResourcesDTO()
        {

        }

        public ResourcesDTO(double metal, double crystal)
        {
            this.Metal = metal;
            this.Crystal = crystal;
        }

        public ResourcesDTO(int userPlanetId, double metal, double crystal, double boron)
        {
            this.UserPlanetID = userPlanetId;
            this.Metal = metal;
            this.Crystal = crystal;
            this.Boron = boron;
        }
        public ResourcesDTO(double metal, double crystal, double boron)
        {
            this.Metal = metal;
            this.Crystal = crystal;
            this.Boron = boron;
        }

        public static ResourcesDTO ResourceZero { get => new ResourcesDTO(0, 0, 0); }
    }
}
