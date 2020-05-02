using System;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class ResourcesDTO
    {
        public int UserPlanetID { get; set; }
        public double Metal { get; set; }
        public double Crystal { get; set; }
        public double Boron { get; set; }
        public ResourcesDTO()
        {

        }

        public ResourcesDTO(double metal, double crystal)
        {
            this.Metal = metal;
            this.Crystal = crystal;
        }

        public ResourcesDTO(double metal, double crystal, double boron)
        {
            this.Metal = metal;
            this.Crystal = crystal;
            this.Boron = boron;
        }

        public ResourcesDTO(int userPlanetId, double metal, double crystal, double boron)
        {
            this.UserPlanetID = userPlanetId;
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


        public static bool operator !=(ResourcesDTO resource, ResourcesDTO target)
        {
            return resource.Metal != target.Metal || resource.Crystal != target.Crystal && resource.Boron != target.Boron;
        }

        public static bool operator ==(ResourcesDTO resource, ResourcesDTO target)
        {
            return resource.Metal == target.Metal && resource.Crystal == target.Crystal && resource.Boron == target.Boron;
        }

        public static bool operator >=(ResourcesDTO resource, ResourcesDTO target)
        {
            return resource.Metal >= target.Metal && resource.Crystal >= target.Crystal && resource.Boron >= target.Boron;
        }

        public static bool operator <=(ResourcesDTO resource, ResourcesDTO target)
        {
            return resource.Metal <= target.Metal && resource.Crystal <= target.Crystal && resource.Boron <= target.Boron;
        }

        public static bool operator >(ResourcesDTO resource, ResourcesDTO target)
        {
            return resource.Metal > target.Metal && resource.Crystal > target.Crystal && resource.Boron > target.Boron;
        }

        public static bool operator <(ResourcesDTO resource, ResourcesDTO target)
        {
            return resource.Metal < target.Metal && resource.Crystal < target.Crystal && resource.Boron < target.Boron;
        }

        public static ResourcesDTO ResourceZero { get => new ResourcesDTO(0, 0, 0); }
    }
}
