using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipInfoPanelController : BasePanelController
{
    [Header("Geminin resmi.")]
    public Image IMG_ShipImage;

    [Header("Geminin ismini basacağız.")]
    public TMP_Text TXT_ShipName;

    [Header("Geminin açıklamasını buraya basacağız.")]
    public TMP_Text TXT_ShipDescription;

    [Header("Geminin savunma değerini buraya basacağız.")]
    public TMP_Text TXT_ShipDefense;

    [Header("Geminin saldırı değerini buraya basacağız.")]
    public TMP_Text TXT_ShipDamage;

    [Header("Geminin aynı anda yapacağı saldırı.")]
    public TMP_Text TXT_ShipDamageQuantity;

    [Header("Geminin hızını buraya basacağız.")]
    public TMP_Text TXT_ShipSpeed;

    [Header("Geminin yakıt tüketimini buraya basacağız.")]
    public TMP_Text TXT_ShipFuelCost;

    [Header("Geminin taşıma kapasitesini buraya basacağız.")]
    public TMP_Text TXT_ShipCapacity;

    [Header("Gösterilen geminin bilgisi.")]
    public Ships CurrentShip;

    [Header("Butonlar ve onların temsil ettiği ikonlar.")]
    public List<ShipIconInfoDTO> IconsAndButtons;

    protected override void Start()
    {
        base.Start();

        // İkonlara tıkladığımızda verilecek olan info eventini ekliyoruz.
        IconsAndButtons.ForEach(e => e.IconInfoButton.onClick.AddListener(() => ShowIconsInfo(e)));
    }

    public void LoadShipData(Ships ship)
    {
        // Şuanki gemi.
        this.CurrentShip = ship;

        // Resmi buluyoruz.
        ShipImageDTO image = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == ship);

        // Eğer resim var ise resmi basıyoruz.
        if (image != null)
            IMG_ShipImage.sprite = image.ShipImage;

        // Geminin ismini yüklüyoruz.
        TXT_ShipName.text = base.GetLanguageText($"S{(int)ship}");

        // Geminin açıklamasını yüklüyoruz.
        TXT_ShipDescription.text = base.GetLanguageText($"SD{(int)ship}");

        // Gemi datasını alıyoruz.
        ShipDataDTO shipData = DataController.DC.GetShip(ship);

        // Geminin sağlık puanını basıyoruz.
        TXT_ShipDefense.text = ResourceExtends.ConvertToDottedResource(shipData.ShipHealth);

        // Geminin saldırı puanını basıyoruz.
        TXT_ShipDamage.text = ResourceExtends.ConvertToDottedResource(shipData.ShipAttackDamage);

        // Geminin saldırı sayısı.
        TXT_ShipDamageQuantity.text = ResourceExtends.ConvertToDottedResource(shipData.ShipAttackQuantity);

        // Geminin hızı.
        TXT_ShipSpeed.text = ResourceExtends.ConvertToDottedResource(shipData.ShipSpeed);

        // Geminin yakıt tüketimi.
        TXT_ShipFuelCost.text = ResourceExtends.ConvertToDottedResource(shipData.ShipFuelt);

        // Geminin depo kapasitesi.
        TXT_ShipCapacity.text = ResourceExtends.ConvertToDottedResource(shipData.CargoCapacity);
    }

    public void ShowTechPanel() => TechnologyController.TC.ShowTechnologyPanelWithItem(TechnologyCategories.Gemiler, (int)CurrentShip);

    public void ShowModicationPanel()
    {

    }

    public void ShowIconsInfo(ShipIconInfoDTO iconInfo)
    {
        string text = base.GetLanguageText($"ID{(int)iconInfo.InfoIconType}");
        GameObject infoPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.InfoPanel);
        infoPanel.GetComponent<InfoPanelController>().LoadText(text);
    }
}
