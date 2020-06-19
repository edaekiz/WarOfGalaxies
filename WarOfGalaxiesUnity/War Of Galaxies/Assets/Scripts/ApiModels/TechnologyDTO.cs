using System;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class TechnologyDTO
    {
        public int TechnologyCategoryId;
        public int IndexId;
        public int RequiredIndexId;
        public int RequiredLevel;
        public int RequiredTechnologyCategoryId;
    }
}
