using Assets.Scripts.ApiModels;
using Assets.Scripts.Extends;
using Assets.Scripts.Pluigns;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Pluigns.MailEncoder;

public class MailPanelItemController : MonoBehaviour
{
    /// <summary>
    /// Gösterilen veri.
    /// </summary>
    public UserMailDTO MailData { get; set; }

    /// <summary>
    /// Mailin decode edilmiş datası.
    /// </summary>
    public MailDecodeDTO MailDecodedData { get; set; }

    [Header("Mail içeriğini buraya basacağız.")]
    public TMP_Text MailContent;

    [Header("Mail tarihini buraya basıyoruz.")]
    public TMP_Text MailDate;

    [Header("Eğer mail okunmuş ise renk tonu bu olacak.")]
    public Color ReadColor;

    public void LoadMailData(UserMailDTO mailData)
    {
        this.MailData = mailData;

        // Gelen maili decode ediyoruz.
        MailDecodedData = MailEncoder.DecodeMail(mailData.MailContent);

        // Mailin türünü arıyoruz. Ona göre Dil dosyasından alacağız datayı.
        string mailType = MailDecodedData.GetMailType();

        // Template yüklüyoruz dil dosyasından.
        string template = LanguageController.LC.GetText($"MT{mailType}");

        // Bütün kayıtları dönüp dil dosyası üzerinden eşliyoruz.
        foreach (KeyValuePair<string, string> record in MailDecodedData.Records)
            template = template.Replace($"{{{record.Key}}}", MailDecodedData.GetValue(record.Key));

        // Mail eğer fazla uzun ise kısa metin olarak gösteriyoruz.
        MailContent.text = template;

        // Mail tarihini basıyoruz.
        MailDate.text = TimeExtends.UTCDateToString(mailData.MailDate);

        // Eğer okunduysa okundu olarak değiştiriyoruz.
        if (mailData.IsReaded)
            SetAsRead();

    }

    public void SetAsRead() => GetComponent<Image>().color = ReadColor;

    public void ShowMailDetails()
    {
        GameObject mdp = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.MailDetailPanel);
        mdp.GetComponent<MailDetailItemPanelController>().LoadMaiLDetails(this.MailData, this.MailDecodedData);
    }

}
