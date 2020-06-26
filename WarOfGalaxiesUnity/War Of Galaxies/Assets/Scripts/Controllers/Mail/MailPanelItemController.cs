using Assets.Languages;
using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Pluigns;
using System;
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
    public TMP_Text TXT_MailContent;

    [Header("Mail tarihini buraya basıyoruz.")]
    public TMP_Text TXT_MailDate;

    [Header("Eğer mail okunmuş ise renk tonu bu olacak.")]
    public Color ReadColor;

    public void LoadMailData(UserMailDTO mailData)
    {
        this.MailData = mailData;

        // Gelen maili decode ediyoruz.
        MailDecodedData = MailEncoder.DecodeMail(mailData.MailContent);

        // Mailin türünü arıyoruz. Ona göre Dil dosyasından alacağız datayı.
        MailTypes mailType = MailDecodedData.GetMailType();

        // Template yüklüyoruz dil dosyasından.
        string template = LanguageController.LC.GetText($"MT{(int)mailType}");

        // Bütün kayıtları dönüp dil dosyası üzerinden eşliyoruz.
        foreach (KeyValuePair<string, string> record in MailDecodedData.Records)
            template = template.Replace($"{{{record.Key}}}", MailDecodedData.GetValue(record.Key));

        // Mail eğer fazla uzun ise kısa metin olarak gösteriyoruz.
        TXT_MailContent.text = template;

        // Mail tarihini basıyoruz.
        TXT_MailDate.text = TimeExtends.UTCDateToString(mailData.MailDate);

        // Eğer okunduysa okundu olarak değiştiriyoruz.
        if (mailData.IsReaded)
            MakeReadColor();
    }

    public void MakeReadColor()
    {
        // Okundu rengini veriyoruz.
        GetComponent<Image>().color = ReadColor;

        // Sönük hale getiriyoruz. Okunmamış olanlar parlak olacak.
        TXT_MailContent.alpha = .66f;

        // Mail tarihini de saydam yapıyoruz.
        TXT_MailDate.alpha = .66f;
    }

    public void ShowMailDetails()
    {
        // Mail okunmamış ise okundu olarak set ediyoruz.
        if (!MailData.IsReaded)
            ReadMail();

        switch (this.MailDecodedData.GetMailType())
        {
            case MailTypes.SavaşRaporu:
                GameObject battlePanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.MailBattleReport);
                battlePanel.GetComponent<MailDetailItemOnlyWarContent>().LoadContent(this.MailData, this.MailDecodedData);
                break;
            case MailTypes.CasusRaporu:
                GameObject spyPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.MailSpyReport);
                spyPanel.GetComponent<MailDetailItemOnlySpyController>().LoadContent(this.MailData, this.MailDecodedData);
                break;
            default:
                GameObject textPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.MailTextReport);
                textPanel.GetComponent<MailDetailItemOnlyTextController>().LoadContent(this.MailData, this.MailDecodedData);
                break;
        }
    }

    public void ReadMail()
    {
        ApiService.API.Post("SetMailAsRead", new MailReadRequestDTO { UserMailId = this.MailData.UserMailId }, (ApiResult response) =>
        {
            if (response.IsSuccess)
            {
                // Mail okundu olarak ayarlıyoruz.
                MailData.IsReaded = true;

                // Tasarımı sönük hale getiriyoruz.
                MakeReadColor();

                // Eğer bir mail okunduysa ozaman kategori de yazan okunmamış miktarı güncelliyoruz.
                if (MailPanelController.MPC != null)
                    MailPanelController.MPC.RefreshCategoryButtonNames();

                // Mail Miktarını yeniliyoruz.
                MailController.MC.RefreshMailIconQuantity();
            }
        });
    }

}
