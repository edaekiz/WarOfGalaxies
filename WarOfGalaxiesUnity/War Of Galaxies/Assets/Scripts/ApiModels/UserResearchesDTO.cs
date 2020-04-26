using Assets.Scripts.ApiModels.Base;
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
    public class UserResearchProgDTO : ProgressModel
    {
        public Researches ResearchID;
        public int ResearchLevel;
        public int UserPlanetID;
        public ResourcesDTO Resources;
        public double LeftTime;
    }
    [Serializable]
    public class UserResearchUpgRequest
    {
        public Researches ResearchID;
        public int UserPlanetID;
    }
}
