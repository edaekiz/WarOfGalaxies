using WarOfGalaxiesApi.DTO.Enums;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class UserResearchesDTO
    {
        public int ResearchID { get; set; }
        public int ResearchLevel { get; set; }
    }

    public class UserResearchProgDTO
    {
        public int ResearchID { get; set; }
        public int ResearchLevel { get; set; }
        public double LeftTime { get; set; }
        public int UserPlanetID { get; set; }
        public ResourcesDTO Resources { get; set; }
    }
    public class UserResearchUpgRequest
    {
        public Researches ResearchID { get; set; }
        public int UserPlanetID { get; set; }
    }
}
