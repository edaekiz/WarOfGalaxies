using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechnologyPanelController : BasePanelController
{
    public static TechnologyPanelController TPC { get; set; }

    private void Awake()
    {
        if (TPC == null)
            TPC = this;
        else
            Destroy(gameObject);
    }

    [Header("Teknolojiler ve kategori butonları.")]
    public List<TechnologyWithCategoryDTO> TechnologyAndCategories;

    [Header("Teknoloji datası.")]
    public GameObject TechnologyItem;

    [Header("Teknoloji itemlerini buraya basacağız.")]
    public Transform TechnologyContent;

    [Header("Keşfedilmemiş ise bu renge boyuyacağız ikonu.")]
    public Color32 NotInventedItemColor;

    protected override void Start()
    {
        base.Start();

        // Teknolojileri ve kategorilerine click event yüklüyoruz.
        TechnologyAndCategories.ForEach(e => e.CategoryButton.onClick.AddListener(() => LoadTechnologies(e.TechonologyCategory)));

    }

    public void LoadTechnologies(TechnologyCategories category)
    {
        // Son kategoriyi gösteriyoruz.
        TechnologyController.TC.LastSelectedCategory = category;

        // Önceki dataları temizliyoruzç
        ClearOldData();

        switch (category)
        {
            case TechnologyCategories.Araştırmalar:
                LoadResearches();
                break;
            case TechnologyCategories.Gemiler:
                LoadShips();
                break;
            case TechnologyCategories.Savunmalar:
                LoadDefenses();
                break;
            case TechnologyCategories.Binalar:
                LoadBuildings();
                break;
        }

        // Butonu kitliyoruz.
        VerifyCategoryButtons(category);
    }

    public void ClearOldData()
    {
        foreach (Transform child in TechnologyContent)
            Destroy(child.gameObject);
    }

    public void LoadShips()
    {
        foreach (ShipDataDTO ship in DataController.DC.SystemData.Ships)
        {
            // Teknolojiyi ekrana basıyoruz.
            GameObject techItem = Instantiate(TechnologyItem, TechnologyContent);

            // Butonu buluyoruz.
            Button button = techItem.GetComponent<Button>();

            // Butona tıkladığımız da panel açılacak.
            button.onClick.AddListener(() => ShowTechnologyDetail(TechnologyCategories.Gemiler, ship.ShipId));

            // İsmini basıyoruz.
            techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"S{ship.ShipId}");

            // Resim bilgisini buluyoruz.
            ShipImageDTO shipImage = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == (Ships)ship.ShipId);
            
            // Resmi buraya yükleyeceğiz.
            Image targetImage = techItem.transform.Find("ItemImage").GetComponent<Image>();

            // Resmi var ise resmi yüklüyoruz.
            if (shipImage != null)
                targetImage.sprite = shipImage.ShipImage;

            // Eğer keşfedilmemiş ise uyarı çıkartacağız.
            SetColorIfNotInvented(TechnologyCategories.Gemiler, ship.ShipId, button, targetImage);
        }
    }

    public void LoadDefenses()
    {
        foreach (DefenseDataDTO defense in DataController.DC.SystemData.Defenses)
        {
            // Teknolojiyi ekrana basıyoruz.
            GameObject techItem = Instantiate(TechnologyItem, TechnologyContent);

            // Butonu buluyoruz.
            Button button = techItem.GetComponent<Button>();

            // Butona tıkladığımız da panel açılacak.
            button.onClick.AddListener(() => ShowTechnologyDetail(TechnologyCategories.Savunmalar, defense.DefenseId));

            // İsmini basıyoruz.
            techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"D{defense.DefenseId}");

            // Resim bilgisini buluyoruz.
            DefenseImageDTO defenseImage = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == (Defenses)defense.DefenseId);

            // Resmi buraya yükleyeceğiz.
            Image targetImage = techItem.transform.Find("ItemImage").GetComponent<Image>();

            // Resmi var ise resmi yüklüyoruz.
            if (defenseImage != null)
                targetImage.sprite = defenseImage.DefenseImage;

            // Eğer keşfedilmemiş ise uyarı çıkartacağız.
            SetColorIfNotInvented(TechnologyCategories.Savunmalar, defense.DefenseId, button, targetImage);
        }
    }

    public void LoadBuildings()
    {
        foreach (BuildingDataDTO building in DataController.DC.SystemData.Buildings)
        {
            // Teknolojiyi ekrana basıyoruz.
            GameObject techItem = Instantiate(TechnologyItem, TechnologyContent);

            // Butonu buluyoruz.
            Button button = techItem.GetComponent<Button>();

            // Butona tıkladığımız da panel açılacak.
            button.onClick.AddListener(() => ShowTechnologyDetail(TechnologyCategories.Binalar, building.BuildingId));

            // İsmini basıyoruz.
            techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"B{building.BuildingId}");

            // Resim bilgisini buluyoruz.
            BuildingsWithImage buildingImage = GlobalBuildingController.GBC.BuildingWithImages.Find(x => x.Building == (Buildings)building.BuildingId);

            // Resmi buraya yükleyeceğiz.
            Image targetImage = techItem.transform.Find("ItemImage").GetComponent<Image>();

            // Resmi var ise resmi yüklüyoruz.
            if (buildingImage != null)
                targetImage.GetComponent<Image>().sprite = buildingImage.BuildingImage;

            // Eğer keşfedilmemiş ise uyarı çıkartacağız.
            SetColorIfNotInvented(TechnologyCategories.Binalar, building.BuildingId, button, targetImage);
        }
    }

    public void LoadResearches()
    {
        foreach (ResearchDataDTO researches in DataController.DC.SystemData.Researches)
        {
            // Teknolojiyi ekrana basıyoruz.
            GameObject techItem = Instantiate(TechnologyItem, TechnologyContent);

            // Buton componentini alıyoruz.
            Button button = techItem.GetComponent<Button>();

            // Butona görev ekliyoruz.
            button.onClick.AddListener(() => ShowTechnologyDetail(TechnologyCategories.Araştırmalar, researches.ResearchId));

            // İsmini basıyoruz.
            techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"R{researches.ResearchId}");

            // Resim bilgisini buluyoruz.
            ResearchImageDTO researchImage = ResearchController.RC.ResearchWithImages.Find(x => x.Research == (Researches)researches.ResearchId);

            // Resmi buraya yükleyeceğiz.
            Image targetImage = techItem.transform.Find("ItemImage").GetComponent<Image>();

            // Resmi var ise resmi yüklüyoruz.
            if (researchImage != null)
                targetImage.sprite = researchImage.ResearchImage;

            // Eğer keşfedilmemiş ise uyarı çıkartacağız.
            SetColorIfNotInvented(TechnologyCategories.Araştırmalar, researches.ResearchId, button, targetImage);
        }
    }

    public void SetColorIfNotInvented(TechnologyCategories category, int indexId, Button itemButton, Image itemImage)
    {
        // Keşfedilmedi ise keşfedilmedi uyarısını çıkaraağız.
        if (!TechnologyController.TC.IsInvented(category, indexId))
        {
            // Disabled rengine boyuyoruz.
            itemButton.GetComponent<Image>().color = NotInventedItemColor;

            // Disabled rengine boyuyoruz gemiyi.
            itemImage.color = NotInventedItemColor;

            // Kilitli ikonunu açıyoruz.
            itemButton.transform.Find("Locked").gameObject.SetActive(true);
        }
    }

    public void VerifyCategoryButtons(TechnologyCategories category)
    {
        TechnologyAndCategories.ForEach(e =>
        {
            if (e.TechonologyCategory == category)
                e.CategoryButton.interactable = false;
            else
                e.CategoryButton.interactable = true;
        });
    }

    public void ShowTechnologyDetail(TechnologyCategories category, int indexId)
    {
        // Paneli açıyoruz.
        GameObject panel = GlobalPanelController.GPC.ShowPanel(PanelTypes.TechnologyDetailPanel);

        // Panele detayları yüklüyoruz.
        panel.GetComponent<TechnologyDetailPanelController>().LoadDetails(category, indexId);

    }

}
