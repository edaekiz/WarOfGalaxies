using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Pluigns;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Assets.Scripts.Pluigns.MailEncoder;

public class MailDetailItemPanelController : BasePanelController
{
    [Serializable]
    public struct MailCategoryContent
    {
        /// <summary>
        /// Gösterilecek olan içerik türünün kategorisi.
        /// </summary>
        public MailCategories Category;

        /// <summary>
        /// Gösterileek olan içerik.
        /// </summary>
        public GameObject ContentObject;
    }

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

    [Header("Maillerin kategorilerine göre gösterilecek olan tasarımları.")]
    public List<MailCategoryContent> MailCategoryContents;

    /// <summary>
    /// Gösterilen maile ait data.
    /// </summary>
    public UserMailDTO MailData { get; set; }

    /// <summary>
    /// Mail detayının decode edilmiş hali.
    /// </summary>
    public MailDecodeDTO DecodedData { get; set; }

    public void LoadMaiLDetails(UserMailDTO mailData, MailDecodeDTO decodedData)
    {
        // Mail tüm datası.
        this.MailData = mailData;

        // Decode edilmiş hali.
        this.DecodedData = decodedData;

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

        // Mailin yüklenecek olan içeriği.
        MailCategoryContent mailContentTemplate = MailCategoryContents.Find(x => (int)x.Category == mailData.MailCategoryId);

        // Templateyi açıyoruz.
        mailContentTemplate.ContentObject.SetActive(true);

        // Ve şimdi dataları yüklüyoruz.
        mailContentTemplate.ContentObject.GetComponent<IMailDetailItem>().LoadContent(mailData, decodedData);

        // Eğer okunmamış ise okundu olarak işaretlemeliyiz.
        if (!mailData.IsReaded)
            SetAsRead();
    }

    public void SetAsRead()
    {
        StartCoroutine(ApiService.API.Post("SetMailAsRead", new MailReadRequestDTO { UserMailId = this.MailData.UserMailId }, (ApiResult response) =>
         {
             if (response.IsSuccess)
             {
                 MailData.IsReaded = true;
                 if (MailPanelController.MPC != null)
                     MailPanelController.MPC.ShowCategoryDetails(MailPanelController.MPC.CurrentShownCategory);
             }
         }));
    }

    public void DeleteMail()
    {
        // Onay panelini açıyoruz.
        GameObject yesNoPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.YesNoPanel);

        // Kontrolleri alıyoruz.
        YesNoPanelController ynpc = yesNoPanel.GetComponent<YesNoPanelController>();

        // Uyarı paneli açıyoruz. Eğer panelden evet denirse kayıdı siliyoruz sistemden.
        ynpc.LoadData(base.GetLanguageText("Uyarı"), base.GetLanguageText("Silmekİstemek"), () =>
        {
            StartCoroutine(ApiService.API.Post("DeleteMail", new MailDeleteRequestDTO { UserMailId = this.MailData.UserMailId }, (ApiResult response) =>
            {

                if (response.IsSuccess)
                {
                    // Maili siliyoruz listeden.
                    MailController.MC.DeleteMail(this.MailData);

                    // Paneli kapatıyoruz.
                    this.ClosePanel();

                }
            }));
        });
    }

}
