using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
        MailTypes mailType = decodedData.GetMailType();

        // Template yüklüyoruz dil dosyasından.
        string template = LanguageController.LC.GetText($"MT{(int)mailType}");

        // Bütün kayıtları dönüp dil dosyası üzerinden eşliyoruz.
        foreach (KeyValuePair<string, string> record in decodedData.Records)
        {
            // Eğer data saldıran gemi ise key value şeklinde dönüyoruz.
            if (record.Key == KEY_SHIPS_ATTACKER || record.Key == KEY_SHIPS_DEFENDER)
            {
                // Her bir gemiyi satır satır alt alta basıyoruz.
                string shipsAsLine = string.Join(Environment.NewLine, decodedData.GetManyItem<Ships>(record.Key).Select(e =>
                {
                    // Gemi ismini alıp miktar ile basıyoruz.
                    return $"{LanguageController.LC.GetText($"S{(int)e.Item1}")} : {e.Item2}";
                }));

                template = template.Replace($"{{{record.Key}}}", shipsAsLine);

            }
            else
            {
                template = template.Replace($"{{{record.Key}}}", decodedData.GetValue(record.Key));
            }
        }

        // Mail eğer fazla uzun ise kısa metin olarak gösteriyoruz.
        TXT_MailContent.text = template;
    }
}
