using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_IOS
using System.IO;
#endif

public class LanguageController : MonoBehaviour
{
    [Serializable]
    public struct LanguageXml
    {
        public string id;
        public string text;
    }

    public static LanguageController LC { get; set; }

    [Header("Seçili olan dile ait key ve valueler.")]
    [HideInInspector]
    public Language LanguageItems;

    [Header("Player prefe kayıt ederken kullanıyoruz.")]
    public static string LANGUAGE_KEY = "LANGUAGE";

    [Header("Dilleri buraya yazıyoruz.")]
    public List<string> Cultures = new List<string>();

    [Header("Varsayılan dili yazıyoruz.")]
    public string DefaultCulture;

    [Header("Şuan ki dil.")]
    public string CurrentCulture = "en";


    [Header("Dil versiyonu.")]
    public string LanguageVersion = "1.0.0";

    [Header("Dil yüklendiğinde.")]
    public bool IsInitialized = false;

    public string SHORT_DAY;
    public string SHORT_HOUR;
    public string SHORT_MINUTE;
    public string SHORT_SECONDS;

    private void Awake()
    {
        if (LC == null)
            LC = this;
        else
        {
            Destroy(gameObject);
            return;
        }

#if UNITY_ANDROID || UNITY_EDITOR
        LoadLanguageAndroid();
#endif
#if UNITY_IOS
        LoadLanguageiOS();
#endif
        LoadTimeLetters();
    }

    /// <summary>
    /// Verilen keyworde ait stringi döner.
    /// </summary>
    /// <param name="keyword">Bulunacak olan data</param>
    /// <param name="replaceData">Eğer keyword value içerisinde {0} var ise bu replacedata daki ilk iteme denk gelir.</param>
    /// <returns></returns>
    public string GetText(string keyword, params string[] replaceData)
    {
        // Dil bilgisi mevcut mu diye kontrol ediyoruz.
        LanguageItem languageItem = LanguageItems.Items.Find(x => x.Keyword == keyword);

        // Eğer yok ise keywordünü dönüyoruz.
        if (languageItem == null)
            return keyword;

        // Var ise bu değeri replace data kadar değiştireceğiz.
        string value = languageItem.Value;

        // Her bir data için dönüyoruz.
        for (int ii = 0; ii < replaceData.Length; ii++)
            value = value.Replace($"{ii}", replaceData[ii]);

        // Var ise karşılık gelen değeri.
        return value;
    }

    /// <summary>
    /// Datası verilen keywordü replace edilmiş olarak döner.
    /// </summary>
    /// <param name="languageItem">Bulunmuş olan data</param>
    /// <param name="replaceData">Eğer keyword value içerisinde {0} var ise bu replacedata daki ilk iteme denk gelir.</param>
    /// <returns></returns>
    public string GetText(LanguageItem languageItem, params string[] replaceData)
    {
        // Var ise bu değeri replace data kadar değiştireceğiz.
        string value = languageItem.Value;

        // Her bir data için dönüyoruz.
        for (int ii = 0; ii < replaceData.Length; ii++)
            value = value.Replace($"{{{ii}}}", replaceData[ii]);

        // Var ise karşılık gelen değeri.
        return value;
    }

    public LanguageItem GetTextItem(string keyword) => LanguageItems.Items.Find(x => x.Keyword == keyword);

    /// <summary>
    /// Android işletim sistemi için dilleri pars eder.
    /// </summary>
    public void LoadLanguageAndroid()
    {
        // Resx dosyasını okuyoruz.
        string xml = GetLanguage(CurrentCulture);

        List<LanguageXml> Texts = new List<LanguageXml>();

        try
        {
            Texts = XElement.Parse(xml).Elements("data").Select(el => new LanguageXml { id = el.Attribute("name").Value, text = el.Element("value").Value.Trim() }).ToList();
        }
        catch (System.Exception)
        {
            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (xml.StartsWith(_byteOrderMarkUtf8))
                xml = xml.Remove(0, _byteOrderMarkUtf8.Length);
            Texts = XElement.Parse(xml).Elements("data").Select(el => new LanguageXml { id = el.Attribute("name").Value, text = el.Element("value").Value.Trim() }).ToList();
        }

        // Dili buraya yükleyeceğiz.
        LanguageItems = new Language
        {
            Culture = CurrentCulture,
            Items = new List<LanguageItem>(),
            Version = LanguageVersion
        };

        // Bütün dilleri dönüyoruz.
        foreach (LanguageXml o in Texts)
        {
            LanguageItems.Items.Add(new LanguageItem
            {
                Keyword = o.id,
                Value = o.text
            });
        }

        IsInitialized = true;
    }

    /// <summary>
    /// IOS işletim sistemi için dilleri pars eder.
    /// </summary>
    public void LoadLanguageiOS()
    {
        string xml = GetLanguage(CurrentCulture);

        // Ve datalarını alıyoruz.
        var obj = new
        {
            Texts = XElement.Parse(xml).Elements("data").Select(el => new { id = el.Attribute("name").Value, text = el.Element("value").Value.Trim() }).ToList()
        };

        LanguageItems = new Language
        {
            Culture = CurrentCulture,
            Items = new List<LanguageItem>(),
            Version = LanguageVersion
        };

        // Bütün dilleri dönüyoruz.
        foreach (var o in obj.Texts)
        {
            LanguageItems.Items.Add(new LanguageItem
            {
                Keyword = o.id,
                Value = o.text
            });
        }

        IsInitialized = true;
    }

    /// <summary>
    /// Oyun içerisinde sıkça kullanacağımız dil dosyası zaman değerlerini yükler.
    /// </summary>
    public void LoadTimeLetters()
    {
        SHORT_DAY = GetText("KISA_GUN");
        SHORT_HOUR = GetText("KISA_SAAT");
        SHORT_MINUTE = GetText("KISA_DAKIKA");
        SHORT_SECONDS = GetText("KISA_SANIYE");
    }

    /// <summary>
    /// Verilen dile ait culture dosyasını yükler
    /// </summary>
    /// <param name="culture">tr/en gibi.</param>
    /// <returns></returns>
    public string GetLanguage(string culture)
    {

#if UNITY_ANDROID || UNITY_EDITOR
        string path = $"{Application.dataPath}/Languages/Language.{culture}.resx";
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            UnityWebRequestAsyncOperation request = www.SendWebRequest();
            while (!request.isDone)
                continue;
            string data = www.downloadHandler.text;
            return data;
        }
#endif

#if UNITY_IOS
        string path = $"{Application.dataPath}/Languages/Language.{culture}.resx";
        return File.ReadAllText(path);
#endif

    }
}
