using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Pluigns;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Assets.Scripts.Pluigns.MailEncoder;

/// <summary>
/// Mailleri açtığımızda düz metin olarak tanımlanan genellikle gezegen ve özel ve oyun kategorisi için kullanılacak olan kontrol.
/// </summary>
public class MailDetailItemOnlyTextController : BasePanelController
{
    [Header("Gönderen gezegeni buraya basıyoruz.")]
    public TMP_Text TXT_SenderPlanetName;

    [Header("Gönderen gezegenin kordinatını buraya basıyoruz.")]
    public TMP_Text TXT_SenderPlanetCordinate;

    [Header("Aksiyon türünü basıyoruz.")]
    public TMP_Text TXT_ActionName;

    [Header("Hedef gezegeni buraya basıyoruz.")]
    public TMP_Text TXT_DestinationPlanetName;

    [Header("Hedef gezgenin kordinatı.")]
    public TMP_Text TXT_DestinationCordinate;

    [Header("Posta tarihi.")]
    public TMP_Text TXT_MailDate;

    [Header("Detayların basılacağı metin.")]
    public TMP_Text TXT_MailContent;

    /// <summary>
    /// Mail datasını tutar.
    /// </summary>
    public MailDecodeDTO MailData { get; set; }

    /// <summary>
    /// Tutulan mail bilgisi.
    /// </summary>
    public UserMailDTO UserMail { get; set; }

    public void LoadContent(UserMailDTO mailData, MailDecodeDTO decodedData)
    {
        // Mail datası lazım olacak.
        this.MailData = decodedData;

        // Kullanıcı maili.
        this.UserMail = mailData;

        // Gönderen gezenin ismini basıyoruz.
        TXT_SenderPlanetName.text = TextExtends.MakeItColorize(base.GetLanguageText("Gönderen"), ":", "orange", decodedData.GetValue(MailEncoder.KEY_SENDERPLANETNAME));

        // Gönderen gezegenin kordinatını basıyoruz.
        TXT_SenderPlanetCordinate.text = TextExtends.MakeItColorize(base.GetLanguageText("Koordinat"), ":", "orange", decodedData.GetValue(MailEncoder.KEY_SENDERPLANETCORDINATE));

        // Mail tarihini basıyoruz.
        TXT_MailDate.text = TextExtends.MakeItColorize(base.GetLanguageText("PostaTarihi"), ":", "orange", TimeExtends.UTCDateToString(mailData.MailDate));

        // Hedef gezegenin ismini basıyoruz.
        TXT_DestinationPlanetName.text = TextExtends.MakeItColorize(base.GetLanguageText("Hedef"), ":", "orange", decodedData.GetValue(MailEncoder.KEY_DESTINATIONPLANETNAME));

        // Gönderen gezegenin kordinatını basıyoruz.
        TXT_DestinationCordinate.text = TextExtends.MakeItColorize(base.GetLanguageText("Koordinat"), ":", "orange", decodedData.GetValue(MailEncoder.KEY_DESTINATIONPLANETCORDINATE));

        // Hareket türünü basıyoruz.
        TXT_ActionName.text = $"{base.GetLanguageText($"FT{(int)decodedData.GetMailAction()}")}";

        // Eğer dönüş maili ise devamına dönüş yazıyoruz.
        if (decodedData.IsReturnMail())
        {
            TXT_ActionName.text += Environment.NewLine;
            TXT_ActionName.text += TextExtends.MakeItColorize($"({base.GetLanguageText("Dönüş")})", "green");
        }

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

    public void OnClickDelete()
    {
        MailController.MC.DeleteMail(this.UserMail, () =>
        {
            base.ClosePanel();
        });
    }
}
