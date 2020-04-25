using Assets.Scripts.Enums;
using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class UserResearchesDTO
    {
        public Researches ResearchID;
        public int ResearchLevel;
    }
    [Serializable]
    public class UserResearchProgDTO
    {
        public Researches ResearchID;
        public int ResearchLevel;
        public double LeftTime;
    }
    [Serializable]
    public class UserResearchUpgRequest
    {
        public Researches ResearchID;
        public int UserPlanetID;
    }
}
