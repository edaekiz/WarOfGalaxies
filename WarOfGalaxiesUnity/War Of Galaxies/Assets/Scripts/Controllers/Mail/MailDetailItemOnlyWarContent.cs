using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using Assets.Scripts.Pluigns;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Pluigns.MailEncoder;

public class MailDetailItemOnlyWarContent : BasePanelController
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

    [Header("Saldıranın ismini buraya basacağız.")]
    public TMP_Text TXT_AttackerName;

    [Header("Savunanın ismini buraya basacağız.")]
    public TMP_Text TXT_DefenderName;

    [Header("Mail içeriği buraya basılacak.")]
    public TMP_Text TXT_MailContent;

    [Header("Kazanılan metal ganimeti.")]
    public TMP_Text TXT_MetalReward;

    [Header("Kazanılan kristal ganimeti.")]
    public TMP_Text TXT_CrystalReward;

    [Header("Kazanılan boron ganimeti.")]
    public TMP_Text TXT_BoronReward;

    [Header("Oluşan metal enkazı.")]
    public TMP_Text TXT_MetalGarbage;

    [Header("Oluşan kristal enkazı.")]
    public TMP_Text TXT_CrystalGarbage;

    [Header("Oluşan kristal enkazı.")]
    public TMP_Text TXT_BoronGarbage;

    [Header("Gemileri ve savunmaları doldurmak için bu modeli kullanacağız.")]
    public GameObject ContentSideItem;

    [Header("Sivil gemiler, savunma gemileri ve savunma sistemlerinin ayracı..")]
    public GameObject ContentSideCategoryItem;

    [Header("Saldıran birimleri basacağımız alan.")]
    public Transform ContentAttackerArea;

    [Header("Savunan birimleri basacağımız alan.")]
    public Transform ContentDefenderArea;

    /// <summary>
    /// Mail ile ilgili gönderen bilgisini tutar.
    /// </summary>
    public MailDecodeDTO MailData { get; set; }

    /// <summary>
    /// Gelen mail bilgisi.
    /// </summary>
    public UserMailDTO UserMail { get; set; }

    public void LoadContent(UserMailDTO mailData, MailDecodeDTO decodedData)
    {
        // Mail içeriğini tutuyoruz.
        this.MailData = decodedData;

        // Mail datasını tutuyoruz.
        this.UserMail = mailData;

        // Template yüklüyoruz dil dosyasından.
        string template = LanguageController.LC.GetText($"MT{(int)decodedData.GetMailType()}");

        // Gönderen gezenin ismini basıyoruz.
        TXT_SenderPlanetName.text = TextExtends.MakeItColorize(base.GetLanguageText("Saldıran"), ":", "orange", decodedData.GetValue(MailEncoder.KEY_SENDERPLANETNAME));

        // Gönderen gezegenin kordinatını basıyoruz.
        TXT_SenderPlanetCordinate.text = TextExtends.MakeItColorize(base.GetLanguageText("Koordinat"), ":", "orange", decodedData.GetValue(MailEncoder.KEY_SENDERPLANETCORDINATE));

        // Mail tarihini basıyoruz.
        TXT_MailDate.text = TextExtends.MakeItColorize(base.GetLanguageText("PostaTarihi"), ":", "orange", TimeExtends.UTCDateToString(mailData.MailDate));

        // Hedef gezegenin ismini basıyoruz.
        TXT_DestinationPlanetName.text = TextExtends.MakeItColorize(base.GetLanguageText("Savunan"), ":", "orange", decodedData.GetValue(MailEncoder.KEY_DESTINATIONPLANETNAME));

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

        // Kazanan dilini replace ediyoruz.
        TXT_MailContent.text = template.Replace($"{{{MailEncoder.KEY_WINNER}}}", decodedData.GetValue(MailEncoder.KEY_WINNER));

        // Saldıran gezegenin ismini basıyoruz.
        TXT_AttackerName.text = $"{LanguageController.LC.GetText("Saldıran")} {decodedData.GetValue(MailEncoder.KEY_SENDERPLANETNAME)}";

        // Saldıran gezegenin ismini basıyoruz.
        TXT_DefenderName.text = $"{LanguageController.LC.GetText("Savunan")} {decodedData.GetValue(MailEncoder.KEY_DESTINATIONPLANETNAME)}";

        // Saldırı gemilerini basıyoruz.
        List<Tuple<Ships, int>> attackerShips = decodedData.GetManyItem<Ships>(MailEncoder.KEY_SHIPS_ATTACKER);

        // Yok edilen gemileri buluyoruz.
        List<Tuple<Ships, int>> lostAttackerShips = decodedData.GetManyItem<Ships>(MailEncoder.KEY_DESTROYED_ATTACKER_SHIPS);

        #region Saldıran tarafa ait savaş gemileri.

        // Sadece savaş gemilerini alıyoruz.
        List<Tuple<Ships, int>> attackerCombatShips = attackerShips.Where(x => !DataController.DC.GetShip(x.Item1).IsCivil).ToList();

        // Saldıran filonun savaş gemisi var ise savaş gemiler kategorisini koyuyoruz..
        if (attackerCombatShips.Count > 0)
        {
            // Gemiyi oluşturuyoruz.
            GameObject seperator = Instantiate(ContentSideCategoryItem, ContentAttackerArea);
            seperator.GetComponentInChildren<TMP_Text>().text = LanguageController.LC.GetText("SavaşGemileri");
        }

        // Her bir savaş gemisini basıyoruz.
        attackerCombatShips.ForEach(e =>
        {
            // Gemiyi oluşturuyoruz.
            GameObject ship = Instantiate(ContentSideItem, ContentAttackerArea);

            // İsmini ikonunu ve miktarını basıyoruz.
            ship.transform.Find("ItemImage").GetComponent<Image>().sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == e.Item1).ShipImage;

            // GEminin ismini basıyoruz.
            ship.transform.Find("ItemName").GetComponent<TMP_Text>().text = LanguageController.LC.GetText($"S{(int)e.Item1}");

            // Döngüdeki gemiden kaybedilen miktarı buluyoruzç
            Tuple<Ships, int> lostData = lostAttackerShips.Find(x => x.Item1 == e.Item1);

            if (lostData != null && lostData.Item2 > 0)
                ship.transform.Find("ItemCount").GetComponent<TMP_Text>().text = $"-{lostData.Item2}        <color=orange>{e.Item2 - lostData.Item2}</color>";
            else
                ship.transform.Find("ItemCount").GetComponent<TMP_Text>().text = e.Item2.ToString();
        });

        #endregion

        #region Saldıran tarafa ait sivil gemiler.

        // Sadece sivil gemilerini alıyoruz.
        List<Tuple<Ships, int>> attackerCivilShips = attackerShips.Where(x => DataController.DC.GetShip(x.Item1).IsCivil).ToList();

        // Saldıran filonun sivil gemisi var ise sivil gemiler kategorisini koyuyoruz..
        if (attackerCivilShips.Count > 0)
        {
            // Gemiyi oluşturuyoruz.
            GameObject seperator = Instantiate(ContentSideCategoryItem, ContentAttackerArea);
            seperator.GetComponentInChildren<TMP_Text>().text = LanguageController.LC.GetText("SivilGemiler");
        }

        // Her bir sivil gemisiyi basıyoruz.
        attackerCivilShips.ForEach(e =>
        {
            // Gemiyi oluşturuyoruz.
            GameObject ship = Instantiate(ContentSideItem, ContentAttackerArea);

            // İsmini ikonunu ve miktarını basıyoruz.
            ship.transform.Find("ItemImage").GetComponent<Image>().sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == e.Item1).ShipImage;

            // GEminin ismini basıyoruz.
            ship.transform.Find("ItemName").GetComponent<TMP_Text>().text = LanguageController.LC.GetText($"S{(int)e.Item1}");

            // Döngüdeki gemiden kaybedilen miktarı buluyoruzç
            Tuple<Ships, int> lostData = lostAttackerShips.Find(x => x.Item1 == e.Item1);

            if (lostData != null && lostData.Item2 > 0)
                ship.transform.Find("ItemCount").GetComponent<TMP_Text>().text = $"-{lostData.Item2}        <color=orange>{e.Item2 - lostData.Item2}</color>";
            else
                ship.transform.Find("ItemCount").GetComponent<TMP_Text>().text = e.Item2.ToString();
        });

        #endregion

        // Savunma gemilerini basıyoruz.
        List<Tuple<Ships, int>> defenderShips = decodedData.GetManyItem<Ships>(MailEncoder.KEY_SHIPS_DEFENDER);

        // Yok edilen gemileri buluyoruz.
        List<Tuple<Ships, int>> lostDefenseShips = decodedData.GetManyItem<Ships>(MailEncoder.KEY_DESTROYED_DEFENDER_SHIPS);

        #region Savunan tarafa ait savaş gemileri.

        // Savunan tarafa ait savaş gemileri.
        List<Tuple<Ships, int>> defenderCombatShips = defenderShips.Where(x => !DataController.DC.GetShip(x.Item1).IsCivil).ToList();

        // Saldıran filonun savaş gemisi var ise savaş gemiler kategorisini koyuyoruz..
        if (defenderCombatShips.Count > 0)
        {
            // Gemiyi oluşturuyoruz.
            GameObject seperator = Instantiate(ContentSideCategoryItem, ContentDefenderArea);
            seperator.GetComponentInChildren<TMP_Text>().text = LanguageController.LC.GetText("SavaşGemileri");
        }

        // Her bir gemiyi gerekli alanlara basıyoruz.
        defenderCombatShips.ForEach(e =>
        {
            // Gemiyi oluşturuyoruz.
            GameObject ship = Instantiate(ContentSideItem, ContentDefenderArea);

            // İsmini ikonunu ve miktarını basıyoruz.
            ship.transform.Find("ItemImage").GetComponent<Image>().sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == e.Item1).ShipImage;
            ship.transform.Find("ItemName").GetComponent<TMP_Text>().text = LanguageController.LC.GetText($"S{(int)e.Item1}");

            // Döngüdeki gemiden kaybedilen miktarı buluyoruzç
            Tuple<Ships, int> lostData = lostDefenseShips.Find(x => x.Item1 == e.Item1);

            if (lostData != null && lostData.Item2 > 0)
                ship.transform.Find("ItemCount").GetComponent<TMP_Text>().text = $"-{lostData.Item2}        <color=orange>{e.Item2 - lostData.Item2}</color>";
            else
                ship.transform.Find("ItemCount").GetComponent<TMP_Text>().text = e.Item2.ToString();
        });

        #endregion

        #region Savunan tarafa ait sivil gemiler.

        // Savunan tarafa ait sivil gemiler.
        List<Tuple<Ships, int>> defenderCivilShips = defenderShips.Where(x => DataController.DC.GetShip(x.Item1).IsCivil).ToList();

        // Saldıran filonun sivil gemisi var ise sivil gemiler kategorisini koyuyoruz..
        if (defenderCivilShips.Count > 0)
        {
            // Gemiyi oluşturuyoruz.
            GameObject seperator = Instantiate(ContentSideCategoryItem, ContentDefenderArea);
            seperator.GetComponentInChildren<TMP_Text>().text = LanguageController.LC.GetText("SivilGemiler");
        }

        // Her bir gemiyi gerekli alanlara basıyoruz.
        defenderCivilShips.ForEach(e =>
        {
            // Gemiyi oluşturuyoruz.
            GameObject ship = Instantiate(ContentSideItem, ContentDefenderArea);

            // İsmini ikonunu ve miktarını basıyoruz.
            ship.transform.Find("ItemImage").GetComponent<Image>().sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == e.Item1).ShipImage;
            ship.transform.Find("ItemName").GetComponent<TMP_Text>().text = LanguageController.LC.GetText($"S{(int)e.Item1}");

            // Döngüdeki gemiden kaybedilen miktarı buluyoruzç
            Tuple<Ships, int> lostData = lostDefenseShips.Find(x => x.Item1 == e.Item1);

            if (lostData != null && lostData.Item2 > 0)
                ship.transform.Find("ItemCount").GetComponent<TMP_Text>().text = $"-{lostData.Item2}        <color=orange>{e.Item2 - lostData.Item2}</color>";
            else
                ship.transform.Find("ItemCount").GetComponent<TMP_Text>().text = e.Item2.ToString();
        });

        #endregion

        // Savunma gemilerini basıyoruz.
        List<Tuple<Defenses, int>> defenderDefenses = decodedData.GetManyItem<Defenses>(MailEncoder.KEY_DEFENSES);

        // Yok edilen gemileri buluyoruz.
        List<Tuple<Defenses, int>> lostDefenseDefenses = decodedData.GetManyItem<Defenses>(MailEncoder.KEY_DESTROYED_DEFENDER_DEFENSES);

        #region Savunan tarafa ait savunma birimleri.

        // Saldıran filonun sivil gemisi var ise sivil gemiler kategorisini koyuyoruz..
        if (defenderDefenses.Count > 0)
        {
            // Gemiyi oluşturuyoruz.
            GameObject seperator = Instantiate(ContentSideCategoryItem, ContentDefenderArea);
            seperator.GetComponentInChildren<TMP_Text>().text = LanguageController.LC.GetText("Savunmalar");
        }

        // Her bir savunmayı gerekli alanlara basıyoruz.
        defenderDefenses.ForEach(e =>
        {
            // Savunmayı oluşturuyoruz.
            GameObject defense = Instantiate(ContentSideItem, ContentDefenderArea);

            // İsmini ikonunu ve miktarını basıyoruz.
            defense.transform.Find("ItemImage").GetComponent<Image>().sprite = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == e.Item1).DefenseImage;
            defense.transform.Find("ItemName").GetComponent<TMP_Text>().text = LanguageController.LC.GetText($"D{(int)e.Item1}");

            // Döngüdeki gemiden kaybedilen miktarı buluyoruzç
            Tuple<Defenses, int> lostData = lostDefenseDefenses.Find(x => x.Item1 == e.Item1);

            if (lostData != null && lostData.Item2 > 0)
                defense.transform.Find("ItemCount").GetComponent<TMP_Text>().text = $"-{lostData.Item2}         <color=orange>{e.Item2 - lostData.Item2}</color>";
            else
                defense.transform.Find("ItemCount").GetComponent<TMP_Text>().text = e.Item2.ToString();
        });

        #endregion

        // Kazanılan metali basıyoruz.
        TXT_MetalReward.text = decodedData.GetValue(MailEncoder.KEY_NEW_METAL, "0");

        // Kazanılan kristali basıyoruz.
        TXT_CrystalReward.text = decodedData.GetValue(MailEncoder.KEY_NEW_CRYSTAL, "0");

        // Kazanılan kristali basıyoruz.
        TXT_BoronReward.text = decodedData.GetValue(MailEncoder.KEY_NEW_BORON, "0");

        // Oluşan metal enkazını basıyoruz.
        TXT_MetalGarbage.text = decodedData.GetValue(MailEncoder.KEY_GARBAGE_METAL, "0");

        // Oluşan kristal enkazını basıyoruz.
        TXT_CrystalGarbage.text = decodedData.GetValue(MailEncoder.KEY_GARBAGE_CRYSTAL, "0");

        // Oluşan boron enkazını basıyoruz.
        TXT_BoronGarbage.text = decodedData.GetValue(MailEncoder.KEY_GARBAGE_BORON, "0");
    }

    public void OnClickAction()
    {
        // Hedef kordinat bilgisini açıyoruz.
        CordinateDTO destinationCordinate = MailData.GetValue(KEY_DESTINATIONPLANETCORDINATE).ToCordinate();

        // Paneli açıyoruz.
        GameObject panel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.GalaxyPlanetActionPanel);

        // Mail datasını yüklüyoruz.
        panel.GetComponent<PlanetActionController>().Load(new SolarPlanetDTO
        {
            UserPlanet = new UserPlanetDTO
            {
                PlanetName = MailData.GetValue(KEY_DESTINATIONPLANETNAME)
            }
        }, destinationCordinate);
    }

    public void OnClickDelete()
    {
        MailController.MC.DeleteMail(this.UserMail, () =>
        {
            base.ClosePanel();
        });
    }
}
