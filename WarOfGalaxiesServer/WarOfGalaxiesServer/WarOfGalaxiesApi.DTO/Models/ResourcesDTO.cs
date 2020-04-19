namespace WarOfGalaxiesApi.DTO.Models
{
    public class ResourcesDTO
    {
        public long Metal { get; set; }
        public long Crystal { get; set; }
        public long Boron { get; set; }
        public ResourcesDTO(long metal, long crystal, long boron)
        {
            this.Metal = metal;
            this.Crystal = crystal;
            this.Boron = boron;
        }

        public static ResourcesDTO ResourceZero { get => new ResourcesDTO(0, 0, 0); }
    }
}
