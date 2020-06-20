using UnityEngine;
using Assets.Scripts.ApiModels;
using Assets.Scripts.Pluigns;
using Assets.Scripts.Enums;
using TMPro;
using Assets.Scripts.Extends;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Models;
using static Assets.Scripts.Pluigns.MailEncoder;

public class MailDetailItemOnlySpyController : BasePanelController
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

    [Header("Mail içeriği buraya basılacak.")]
    public TMP_Text TXT_MailContent;

    [Header("Dataları basarken kullanılacak olan template resimli metin.")]
    public GameObject SpyMailTemplateWithImage;

    [Header("Dataları basarken kullanılacak olan template sadece metin.")]
    public GameObject SpyMailTemplateItemText;

    [Header("Kaynakları basacağımız alan.")]
    public Transform ResourceContent;

    [Header("Binaları basacağımız alan.")]
    public Transform BuildingContent;

    [Header("Araştırmaları basacağımız alan.")]
    public Transform ResearchContent;

    [Header("Gemileri basacağımız alan.")]
    public Transform ShipsContent;

    [Header("Savunmaları basacağımız alan.")]
    public Transform DefensesContent;

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
            template = template.Replace($"{{{record.Key}}}", decodedData.GetValue(record.Key));

        // Metni basıyoruz.
        TXT_MailContent.text = template;

        #region Metal Kristal ve Bor datalarını basıyoruz.

        string metalQuantity = decodedData.GetValue(MailEncoder.KEY_NEW_METAL);
        string crystalQuantity = decodedData.GetValue(MailEncoder.KEY_NEW_CRYSTAL);
        string boronQuantity = decodedData.GetValue(MailEncoder.KEY_NEW_BORON);

        GameObject metalItem = Instantiate(SpyMailTemplateItemText, ResourceContent);
        metalItem.GetComponent<TMP_Text>().text = $"{TextExtends.MakeItColorize(LanguageController.LC.GetText("Metal"), "orange")}{Environment.NewLine}{metalQuantity}";

        GameObject crystalItem = Instantiate(SpyMailTemplateItemText, ResourceContent);
        crystalItem.GetComponent<TMP_Text>().text = $"{TextExtends.MakeItColorize(LanguageController.LC.GetText("Kristal"), "orange")}{Environment.NewLine}{crystalQuantity}";

        GameObject boronItem = Instantiate(SpyMailTemplateItemText, ResourceContent);
        boronItem.GetComponent<TMP_Text>().text = $"{TextExtends.MakeItColorize(LanguageController.LC.GetText("Bor"), "orange")}{Environment.NewLine}{boronQuantity}";

        #endregion

        #region Binalar

        decodedData.GetManyItem<Buildings>(MailEncoder.KEY_BUILDING_DEFENDER).OrderBy(x => x.Item1).ToList().ForEach(e =>
        {
            GameObject item = Instantiate(SpyMailTemplateWithImage, BuildingContent);
            BuildingsWithImage buildingImage = GlobalBuildingController.GBC.BuildingWithImages.Find(x => x.Building == e.Item1);
            if (buildingImage != null)
                item.transform.Find("ItemImage").GetComponent<Image>().sprite = buildingImage.BuildingImage;
            item.transform.Find("ItemName").GetComponent<TMP_Text>().text = LanguageController.LC.GetText($"B{(int)e.Item1}");
            item.transform.Find("ItemCount").GetComponent<TMP_Text>().text = e.Item2.ToString();
        });

        #endregion

        #region Araştırmalar

        // Raporda yer alana araştırmaları basıyoruz.
        decodedData.GetManyItem<Researches>(MailEncoder.KEY_RESEARCHES).ForEach(e =>
        {
            // Araştırmayı oluşturuyoruz.
            GameObject item = Instantiate(SpyMailTemplateWithImage, ResearchContent);

            // İkon bilgisini buluyoruz.
            ResearchImageDTO researchImage = ResearchController.RC.ResearchWithImages.Find(x => x.Research == e.Item1);

            // Eğer bir ikonu var ise ikonu basıyoruz.
            if (researchImage != null)
                item.transform.Find("ItemImage").GetComponent<Image>().sprite = researchImage.ResearchImage;

            // Araştırma ismini basıyoruz.
            item.transform.Find("ItemName").GetComponent<TMP_Text>().text = LanguageController.LC.GetText($"R{(int)e.Item1}");

            // Araştırma seviyesini basıyoruz.
            item.transform.Find("ItemCount").GetComponent<TMP_Text>().text = e.Item2.ToString();
        });

        #endregion

        #region Ships

        decodedData.GetManyItem<Ships>(MailEncoder.KEY_SHIPS_DEFENDER).ForEach(e =>
        {

            GameObject item = Instantiate(SpyMailTemplateWithImage, ShipsContent);
            item.transform.Find("ItemImage").GetComponent<Image>().sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == e.Item1).ShipImage;
            item.transform.Find("ItemName").GetComponent<TMP_Text>().text = LanguageController.LC.GetText($"S{(int)e.Item1}");
            item.transform.Find("ItemCount").GetComponent<TMP_Text>().text = e.Item2.ToString();

        });

        #endregion

        #region Defenses

        decodedData.GetManyItem<Defenses>(MailEncoder.KEY_DEFENSES).ForEach(e =>
        {

            GameObject item = Instantiate(SpyMailTemplateWithImage, DefensesContent);
            item.transform.Find("ItemImage").GetComponent<Image>().sprite = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == e.Item1).DefenseImage;
            item.transform.Find("ItemName").GetComponent<TMP_Text>().text = LanguageController.LC.GetText($"D{(int)e.Item1}");
            item.transform.Find("ItemCount").GetComponent<TMP_Text>().text = e.Item2.ToString();

        });

        #endregion
    }

    public void OnClickAction()
    {
        // Hedef kordinat bilgisini açıyoruz.
        CordinateDTO destinationCordinate = MailData.GetValue(KEY_DESTINATIONPLANETCORDINATE).ToCordinate();

        // Paneli açıyoruz.
        GameObject panel = GlobalPanelController.GPC.ShowPanel(PanelTypes.GalaxyPlanetActionPanel);

        // Mail datasını yüklüyoruz.
        panel.GetComponent<PlanetActionController>().Load(new SolarPlanetDTO
        {
            UserPlanet = new UserPlanetDTO
            {
                PlanetName = MailData.GetValue(KEY_DESTINATIONPLANETNAME)
            }
        }, destinationCordinate);

        // Saldırı seçili olarak açmalıyız.
        panel.GetComponent<PlanetActionController>().OnActionChanged(FleetTypes.Saldır);
    }

    public void OnClickDelete()
    {
        MailController.MC.DeleteMail(this.UserMail, () =>
        {
            base.ClosePanel();
        });
    }

}
