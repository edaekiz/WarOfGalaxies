using Assets.Scripts.ApiModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Assets.Scripts.Pluigns.MailEncoder;

/// <summary>
/// Mailleri açtığımızda düz metin olarak tanımlanan genellikle gezegen ve özel ve oyun kategorisi için kullanılacak olan kontrol.
/// </summary>
public class MailDetailItemOnlyTextController : MonoBehaviour, IMailDetailItem
{
    [Header("Detayların basılacağı metin.")]
    public TMP_Text TXT_MailContent;

    public void LoadContent(UserMailDTO mailData, MailDecodeDTO decodedData)
    {
        // Mailin türünü arıyoruz. Ona göre Dil dosyasından alacağız datayı.
        string mailType = decodedData.GetMailType();

        // Template yüklüyoruz dil dosyasından.
        string template = LanguageController.LC.GetText($"MT{mailType}");

        // Bütün kayıtları dönüp dil dosyası üzerinden eşliyoruz.
        foreach (KeyValuePair<string, string> record in decodedData.Records)
            template = template.Replace($"{{{record.Key}}}", decodedData.GetValue(record.Key));

        // Mail eğer fazla uzun ise kısa metin olarak gösteriyoruz.
        TXT_MailContent.text = template;
    }
}
