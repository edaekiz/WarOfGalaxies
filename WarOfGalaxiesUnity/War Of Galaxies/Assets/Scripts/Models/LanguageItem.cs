using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class LanguageItem
    {
        public string Keyword;
        public string Value;
    }

    [Serializable]
    public class Language
    {
        public string Version;
        public string Culture;
        public List<LanguageItem> Items;

        public Language()
        {
            Items = new List<LanguageItem>();
        }

    }
}
