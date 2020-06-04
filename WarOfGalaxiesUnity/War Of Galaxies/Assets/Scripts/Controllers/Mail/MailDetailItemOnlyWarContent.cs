using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Pluigns;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailDetailItemOnlyWarContent : MonoBehaviour, IMailDetailItem
{
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
    public GameObject ContentShipDefenseItem;

    [Header("Saldırana ait savaş gemilerini buraya basacağız.")]
    public Transform AttackerCombatShipsContent;

    [Header("Saldırana ait sivil gemilerini buraya basacağız.")]
    public Transform AttackerCivilShipsContent;

    [Header("Savunana ait sivil gemilerini buraya basacağız.")]
    public Transform DefenderCivilShipsContent;

    [Header("Savunana ait savaş gemilerini buraya basacağız.")]
    public Transform DefenderCombatShipsContent;

    [Header("Savunana ait savunma ünitelerini buraya basacağız.")]
    public Transform DefenderDefensesContent;

    public void LoadContent(UserMailDTO mailData, MailEncoder.MailDecodeDTO decodedData)
    {
        // Kazanan tarafı basıyoruz.
        string winnerSide = TextExtends.MakeItColorize(decodedData.GetValue("KEY_WINNER") == "0" ? LanguageController.LC.GetText("Saldıran") : LanguageController.LC.GetText("Savunan"), "orange");

        // Kazanan tarafı basıyoruz.
        TXT_MailContent.text = LanguageController.LC.GetText($"MT{(int)decodedData.GetMailType()}", winnerSide);

        // Saldıran gezegenin ismini basıyoruz.
        TXT_AttackerName.text = $"{LanguageController.LC.GetText("Saldıran")} {decodedData.GetValue(MailEncoder.KEY_SENDERPLANETNAME)}";

        // Saldıran gezegenin ismini basıyoruz.
        TXT_DefenderName.text = $"{LanguageController.LC.GetText("Savunan")} {decodedData.GetValue(MailEncoder.KEY_DESTINATIONPLANETNAME)}";

        // Saldırı gemilerini basıyoruz.
        List<Tuple<Ships, int>> attackerShips = decodedData.GetManyItem<Ships>(MailEncoder.KEY_SHIPS_ATTACKER);

        // Yok edilen gemileri buluyoruz.
        List<Tuple<Ships, int>> lostAttackerShips = decodedData.GetManyItem<Ships>(MailEncoder.KEY_DESTROYED_ATTACKER_SHIPS);

        // Her bir gemiyi gerekli alanlara basıyoruz.
        attackerShips.ForEach(e =>
        {
            // Bu bir sivil gemi mi?
            bool shipIsCivil = DataController.DC.GetShip(e.Item1).IsCivil;

            // Basılacak alan.
            Transform content = null;

            // Sivil bir gemi ise sivil gemiler alanına basacağız.
            if (!shipIsCivil)
                content = AttackerCombatShipsContent;
            else
                content = AttackerCivilShipsContent;

            // Gemiyi oluşturuyoruz.
            GameObject ship = Instantiate(ContentShipDefenseItem, content);

            // İsmini ikonunu ve miktarını basıyoruz.
            ship.transform.Find("ItemImage").GetComponent<Image>().sprite = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == e.Item1).ShipImage;
            ship.transform.Find("ItemName").GetComponent<TMP_Text>().text = LanguageController.LC.GetText($"S{(int)e.Item1}");

            // Döngüdeki gemiden kaybedilen miktarı buluyoruzç
            Tuple<Ships, int> lostData = lostAttackerShips.Find(x => x.Item1 == e.Item1);

            if (lostData != null && lostData.Item2 > 0)
                ship.transform.Find("ItemCount").GetComponent<TMP_Text>().text = $"-{lostData.Item2}        <color=orange>{e.Item2 - lostData.Item2}</color>";
            else
                ship.transform.Find("ItemCount").GetComponent<TMP_Text>().text = e.Item2.ToString();
        });

        // Savunma gemilerini basıyoruz.
        List<Tuple<Ships, int>> defenderShips = decodedData.GetManyItem<Ships>(MailEncoder.KEY_SHIPS_DEFENDER);

        // Yok edilen gemileri buluyoruz.
        List<Tuple<Ships, int>> lostDefenseShips = decodedData.GetManyItem<Ships>(MailEncoder.KEY_DESTROYED_DEFENDER_SHIPS);

        // Her bir gemiyi gerekli alanlara basıyoruz.
        defenderShips.ForEach(e =>
        {
            // Bu bir sivil gemi mi?
            bool shipIsCivil = DataController.DC.GetShip(e.Item1).IsCivil;

            // Basılacak alan.
            Transform content = null;

            // Sivil bir gemi ise sivil gemiler alanına basacağız.
            if (!shipIsCivil)
                content = DefenderCombatShipsContent;
            else
                content = DefenderCivilShipsContent;

            // Gemiyi oluşturuyoruz.
            GameObject ship = Instantiate(ContentShipDefenseItem, content);

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

        // Savunma gemilerini basıyoruz.
        List<Tuple<Defenses, int>> defenderDefenses = decodedData.GetManyItem<Defenses>(MailEncoder.KEY_DEFENSES);

        // Yok edilen gemileri buluyoruz.
        List<Tuple<Defenses, int>> lostDefenseDefenses = decodedData.GetManyItem<Defenses>(MailEncoder.KEY_DESTROYED_DEFENDER_DEFENSES);

        // Her bir savunmayı gerekli alanlara basıyoruz.
        defenderDefenses.ForEach(e =>
        {
            // Savunmayı oluşturuyoruz.
            GameObject defense = Instantiate(ContentShipDefenseItem, DefenderDefensesContent);

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
}
