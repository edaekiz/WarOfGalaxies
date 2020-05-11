using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controllers.Base
{
    public class BaseLanguageBehaviour : MonoBehaviour
    {
        private readonly List<LanguageItem> cachedLanguages = new List<LanguageItem>();

        public string GetLanguageText(string keyword, params string[] replaceData)
        {
            // Dil bilgisi mevcut mu diye kontrol ediyoruz.
            LanguageItem languageItem = cachedLanguages.Find(x => x.Keyword == keyword);

            // Default değer keyword.
            string value = keyword;

            // Dil bilgisi yok ise normal dönücez.
            if (languageItem != null)
            {
                // Cacheden alıyoruz.
                value = languageItem.Value;

                // Her bir data için dönüyoruz.
                for (int ii = 0; ii < replaceData.Length; ii++)
                    value = value.Replace($"{{{ii}}}", replaceData[ii]);
            }
            else
            {
                // Dil bilgisini alıyoruz.
                LanguageItem langItem = LanguageController.LC.GetTextItem(keyword);

                // Eğer dil var ise.
                if (langItem != null)
                {
                    // Değeri değiştiriyoruz.
                    value = LanguageController.LC.GetText(langItem, replaceData);

                    // Çıkan keyi cache ekliyoruz.
                    cachedLanguages.Add(langItem);
                }
            }

            // Sonucu dönüyoruz.
            return value;
        }
    }
}
