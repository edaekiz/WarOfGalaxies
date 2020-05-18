using WarOfGalaxiesApi.DTO.Enums;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class FleetDTO
    {
        public int FleetId { get; set; }
        public int FleetActionTypeId { get; set; }
        public string SenderCordinate { get; set; }
        public int SenderUserId { get; set; }
        public int SenderUserPlanetId { get; set; }
        public string SenderPlanetName { get; set; }
        public int SenderPlanetTypeId { get; set; }
        public int? DestinationUserId { get; set; }
        public int? DestinationUserPlanetId { get; set; }
        public string DestinationPlanetName { get; set; }
        public string DestinationCordinate { get; set; }
        public int? DestinationPlanetTypeId { get; set; }
        public double BeginPassedTime { get; set; }
        public double EndLeftTime { get; set; }
        public bool IsReturning { get; set; }
        public double CarriedMetal { get; set; }
        public double CarriedCrystal { get; set; }
        public double CarriedBoron { get; set; }
    }

    public class GetLastFleetsDTO
    {
        public int LastFleetId { get; set; }
    }

    public class SendFleetFromPlanetDTO
    {
        public int SenderUserPlanetId { get; set; }
        public string Ships { get; set; }
        public int DestinationGalaxyIndex { get; set; }
        public int DestinationSolarIndex { get; set; }
        public int DestinationOrderIndex { get; set; }
        public int FleetType { get; set; }
        public double CarriedMetal { get; set; }
        public double CarriedCrystal { get; set; }
        public double CarriedBoron { get; set; }
        public float FleetSpeed { get; set; }
    }
}
