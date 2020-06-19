using Assets.Scripts.Enums;
using System;
using UnityEngine.UI;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class TechnologyWithCategoryDTO
    {
        public Button CategoryButton;
        public TechnologyCategories TechonologyCategory;
    }
}
