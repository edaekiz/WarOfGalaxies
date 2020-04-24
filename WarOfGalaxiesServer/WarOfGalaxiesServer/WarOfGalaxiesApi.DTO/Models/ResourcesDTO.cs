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

        public static ResourcesDTO ResourceZero { get => new ResourcesDTO(0, 0, 0); }
    }
}
