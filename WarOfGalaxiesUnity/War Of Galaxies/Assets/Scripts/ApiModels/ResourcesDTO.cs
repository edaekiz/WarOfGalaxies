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
        
        /// <summary>
        /// Kaynakların hepsini çarpıyoruz. verilen değer ile. Yeni bir nesne olarak geri döner.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static ResourcesDTO operator *(ResourcesDTO resource, double rate)
        {
            return new ResourcesDTO(resource.Metal * rate, resource.Crystal * rate, resource.Boron * rate);
        }

        public static ResourcesDTO ResourceZero { get => new ResourcesDTO(0, 0, 0); }
    }
}
