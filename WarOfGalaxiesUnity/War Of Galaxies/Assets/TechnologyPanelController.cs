using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
    public ScrollRect TechnologyContent;

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
        foreach (Transform child in TechnologyContent.content)
            Destroy(child.gameObject);
    }

    public void LoadShips()
    {
        foreach (ShipDataDTO ship in DataController.DC.SystemData.Ships)
        {
            // Teknolojiyi ekrana basıyoruz.
            GameObject techItem = Instantiate(TechnologyItem, TechnologyContent.content);

            // Butona tıkladığımız da panel açılacak.
            techItem.GetComponent<Button>().onClick.AddListener(() => ShowTechnologyDetail(TechnologyCategories.Gemiler, ship.ShipId));

            // İsmini basıyoruz.
            techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"S{ship.ShipId}");

            // Resim bilgisini buluyoruz.
            ShipImageDTO shipImage = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == (Ships)ship.ShipId);

            // Resmi var ise resmi yüklüyoruz.
            if (shipImage != null)
                techItem.transform.Find("ItemImage").GetComponent<Image>().sprite = shipImage.ShipImage;

            // Keşfedildi mi?
            techItem.transform.Find("IsInvented").gameObject.SetActive(TechnologyController.TC.IsInvented(TechnologyCategories.Gemiler, ship.ShipId));
        }
    }

    public void LoadDefenses()
    {
        foreach (DefenseDataDTO defense in DataController.DC.SystemData.Defenses)
        {
            // Teknolojiyi ekrana basıyoruz.
            GameObject techItem = Instantiate(TechnologyItem, TechnologyContent.content);

            // Butona tıkladığımız da panel açılacak.
            techItem.GetComponent<Button>().onClick.AddListener(() => ShowTechnologyDetail(TechnologyCategories.Savunmalar, defense.DefenseId));

            // İsmini basıyoruz.
            techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"D{defense.DefenseId}");

            // Resim bilgisini buluyoruz.
            DefenseImageDTO defenseImage = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == (Defenses)defense.DefenseId);

            // Resmi var ise resmi yüklüyoruz.
            if (defenseImage != null)
                techItem.transform.Find("ItemImage").GetComponent<Image>().sprite = defenseImage.DefenseImage;

            // Keşfedildi mi?
            techItem.transform.Find("IsInvented").gameObject.SetActive(TechnologyController.TC.IsInvented(TechnologyCategories.Savunmalar, defense.DefenseId));
        }
    }

    public void LoadBuildings()
    {
        foreach (BuildingDataDTO building in DataController.DC.SystemData.Buildings)
        {
            // Teknolojiyi ekrana basıyoruz.
            GameObject techItem = Instantiate(TechnologyItem, TechnologyContent.content);

            // Butona tıkladığımız da panel açılacak.
            techItem.GetComponent<Button>().onClick.AddListener(() => ShowTechnologyDetail(TechnologyCategories.Binalar, building.BuildingId));

            // İsmini basıyoruz.
            techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"B{building.BuildingId}");

            // Resim bilgisini buluyoruz.
            BuildingsWithImage buildingImage = GlobalBuildingController.GBC.BuildingWithImages.Find(x => x.Building == (Buildings)building.BuildingId);

            // Resmi var ise resmi yüklüyoruz.
            if (buildingImage != null)
                techItem.transform.Find("ItemImage").GetComponent<Image>().sprite = buildingImage.BuildingImage;

            // Keşfedildi mi?
            techItem.transform.Find("IsInvented").gameObject.SetActive(TechnologyController.TC.IsInvented(TechnologyCategories.Binalar, building.BuildingId));
        }
    }

    public void LoadResearches()
    {
        foreach (ResearchDataDTO researches in DataController.DC.SystemData.Researches)
        {
            // Teknolojiyi ekrana basıyoruz.
            GameObject techItem = Instantiate(TechnologyItem, TechnologyContent.content);

            // Butona tıkladığımız da panel açılacak.
            techItem.GetComponent<Button>().onClick.AddListener(() => ShowTechnologyDetail(TechnologyCategories.Araştırmalar, researches.ResearchId));

            // İsmini basıyoruz.
            techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"R{researches.ResearchId}");

            // Resim bilgisini buluyoruz.
            ResearchImageDTO researchImage = ResearchController.RC.ResearchWithImages.Find(x => x.Research == (Researches)researches.ResearchId);

            // Resmi var ise resmi yüklüyoruz.
            if (researchImage != null)
                techItem.transform.Find("ItemImage").GetComponent<Image>().sprite = researchImage.ResearchImage;

            // Keşfedildi mi?
            techItem.transform.Find("IsInvented").gameObject.SetActive(TechnologyController.TC.IsInvented(TechnologyCategories.Araştırmalar, researches.ResearchId));
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
        GameObject panel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.TechnologyDetailPanel);

        // Panele detayları yüklüyoruz.
        panel.GetComponent<TechnologyDetailPanelController>().LoadDetails(category, indexId);

    }

}
