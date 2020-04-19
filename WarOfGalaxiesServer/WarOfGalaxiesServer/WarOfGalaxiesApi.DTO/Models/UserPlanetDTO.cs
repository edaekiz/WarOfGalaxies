namespace WarOfGalaxiesApi.DTO.Models
{
    public class UserPlanetDTO
    {
        public int UserPlanetId { get; set; }
        public int UserId { get; set; }
        public string PlanetCordinate { get; set; }
        public int PlanetType { get; set; }
        public string PlanetName { get; set; }
        public long Metal { get; set; }
        public long Crystal { get; set; }
        public long Boron { get; set; }
    }

    public class VerifyResourceDTO
    {
        public int UserPlanetID;
    }
}
