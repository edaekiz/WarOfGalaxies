using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechnologyDetailPanelController : BasePanelController
{
    [Header("Teknolojileri basarken bunu kullanacağız.")]
    public GameObject TechItem;

    [Header("Seçilmiş olan teknolojiyi buraya basacağız.")]
    public Transform SelectedTechContent;

    [Header("Seçilmiş olan teknolojinin gereksinimleri buraya basacağız.")]
    public Transform SelectedTechReqsContent;

    [Header("Teknoloji adını buraya basacağız.")]
    public TMP_Text TXT_TechName;

    [Header("Eğer herhangi bir gereksinimi yok ise bu uyarıyı basacağız.")]
    public GameObject TXT_TechNoReq;

    [Header("Eğer yeterli değil ise seviye bu renge dönüşecek.")]
    public Color CLR_NotEnoughLevel;

    public void LoadDetails(TechnologyCategories category, int indexId)
    {
        // Ana teknolojiyi basıyoruz.
        switch (category)
        {
            case TechnologyCategories.Araştırmalar:
                PutResearchToContent(indexId, 0, SelectedTechContent);
                break;
            case TechnologyCategories.Gemiler:
                PutShipToContent(indexId, SelectedTechContent);
                break;
            case TechnologyCategories.Savunmalar:
                PutDefenseToContent(indexId, SelectedTechContent);
                break;
            case TechnologyCategories.Binalar:
                PutBuildingToContent(indexId, 0, SelectedTechContent);
                break;
        }

        // Gereksinimleri basarken kullanacağız.
        IEnumerable<TechnologyDTO> reqTechQuery = TechnologyController.TC.Technologies.Where(x => x.TechnologyCategoryId == (int)category && x.IndexId == indexId);

        // Gereksinimlerini basıyoruz.
        foreach (TechnologyDTO techReq in reqTechQuery)
        {
            switch ((TechnologyCategories)techReq.RequiredTechnologyCategoryId)
            {
                case TechnologyCategories.Araştırmalar:
                    PutResearchToContent(techReq.RequiredIndexId, techReq.RequiredLevel, SelectedTechReqsContent);
                    break;
                case TechnologyCategories.Binalar:
                    PutBuildingToContent(techReq.RequiredIndexId, techReq.RequiredLevel, SelectedTechReqsContent);
                    break;
            }
        }

        // Eğer gereksinim yok var ise uyarıyı kapatıyoruz.
        if (reqTechQuery.Count() > 0)
            TXT_TechNoReq.SetActive(false);
        else // Aksi durumda açıyoruz.
            TXT_TechNoReq.SetActive(true);
    }

    public void PutShipToContent(int indexId, Transform content)
    {
        // Teknolojiyi ekrana basıyoruz.
        GameObject techItem = Instantiate(TechItem, content);

        // Yalnızca gereksinimler basılacak. Content değeri basılan alan referansı ile aynı değil ise tıklama eventi olmayacak.
        if (ReferenceEquals(content, SelectedTechReqsContent))
            techItem.GetComponent<Button>().onClick.AddListener(() => TechnologyController.TC.ShowTechnologyPanelWithItem(TechnologyCategories.Gemiler, indexId));
        else // Eğer tıklanabilir değil ise teknoloji ağacı metnini basıcaz.
            TXT_TechName.text = base.GetLanguageText("XTeknolojiAğacı", base.GetLanguageText($"S{indexId}"), Environment.NewLine);

        // Teknolojinin ismini basacağız.
        techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"S{indexId}");

        // Resim bilgisini buluyoruz.
        ShipImageDTO shipImage = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == (Ships)indexId);

        // Resmi var ise resmi yüklüyoruz.
        if (shipImage != null)
            techItem.transform.Find("ItemImage").GetComponent<Image>().sprite = shipImage.ShipImage;

        // Keşfedildi mi?
        techItem.transform.Find("IsInvented").gameObject.SetActive(TechnologyController.TC.IsInvented(TechnologyCategories.Gemiler, indexId));
    }

    public void PutDefenseToContent(int indexId, Transform content)
    {
        // Teknolojiyi ekrana basıyoruz.
        GameObject techItem = Instantiate(TechItem, content);

        // Yalnızca gereksinimler basılacak. Content değeri basılan alan referansı ile aynı değil ise tıklama eventi olmayacak.
        if (ReferenceEquals(content, SelectedTechReqsContent))
            techItem.GetComponent<Button>().onClick.AddListener(() => TechnologyController.TC.ShowTechnologyPanelWithItem(TechnologyCategories.Savunmalar, indexId));
        else // Eğer tıklanabilir değil ise teknoloji ağacı metnini basıcaz.
            TXT_TechName.text = base.GetLanguageText("XTeknolojiAğacı", base.GetLanguageText($"D{indexId}"), Environment.NewLine);

        // Teknolojinin ismini basacağız.
        techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"D{indexId}");

        // Resim bilgisini buluyoruz.
        DefenseImageDTO defenseImage = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == (Defenses)indexId);

        // Resmi var ise resmi yüklüyoruz.
        if (defenseImage != null)
            techItem.transform.Find("ItemImage").GetComponent<Image>().sprite = defenseImage.DefenseImage;

        // Keşfedildi mi?
        techItem.transform.Find("IsInvented").gameObject.SetActive(TechnologyController.TC.IsInvented(TechnologyCategories.Savunmalar, indexId));
    }

    public void PutBuildingToContent(int indexId, int reqLevel, Transform content)
    {
        // Teknolojiyi ekrana basıyoruz.
        GameObject techItem = Instantiate(TechItem, content);

        // Yalnızca gereksinimler basılacak. Content değeri basılan alan referansı ile aynı değil ise tıklama eventi olmayacak.
        if (ReferenceEquals(content, SelectedTechReqsContent))
            techItem.GetComponent<Button>().onClick.AddListener(() => TechnologyController.TC.ShowTechnologyPanelWithItem(TechnologyCategories.Binalar, indexId));
        else // Eğer tıklanabilir değil ise teknoloji ağacı metnini basıcaz.
            TXT_TechName.text = base.GetLanguageText("XTeknolojiAğacı", base.GetLanguageText($"B{indexId}"), Environment.NewLine);

        // Teknolojinin ismini basacağız.
        techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"B{indexId}");

        // Seviye yeterli mi?
        bool isLevelEnough = true;

        // Eğer gereken seviye 0dan büyük ise yazacağızç
        if (reqLevel > 0)
        {
            // Kullanıcının bina seviyesi.
            int userOwnedLevel = LoginController.LC.CurrentUser.UserPlanetsBuildings.Where(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == (Buildings)indexId).Select(x => x.BuildingLevel).DefaultIfEmpty(0).FirstOrDefault();

            // Seviyeyi basacağımız alan.
            TMP_Text techReqLevel = techItem.transform.Find("TechReqLevel").GetComponent<TMP_Text>();

            // Kullanıcının görünen seviyesi gereksinimden fazla olamaz.
            userOwnedLevel = userOwnedLevel > reqLevel ? reqLevel : userOwnedLevel;

            // Seviyeyi basıyoruz.
            techReqLevel.text = $"{userOwnedLevel}/{reqLevel}";

            // Eğer seviye yeterli değil ise o renge boyuyoruz.
            if (userOwnedLevel < reqLevel)
            {
                // Metni kırmızıya boyuyoruz.
                techReqLevel.faceColor = CLR_NotEnoughLevel;

                // Öğrenilmedi!
                isLevelEnough = false;
            }
        }

        // Resim bilgisini buluyoruz.
        BuildingsWithImage buildingImage = GlobalBuildingController.GBC.BuildingWithImages.Find(x => x.Building == (Buildings)indexId);

        // Resmi var ise resmi yüklüyoruz.
        if (buildingImage != null)
            techItem.transform.Find("ItemImage").GetComponent<Image>().sprite = buildingImage.BuildingImage;

        // Keşfedildi mi?
        if (isLevelEnough)
            techItem.transform.Find("IsInvented").gameObject.SetActive(TechnologyController.TC.IsInvented(TechnologyCategories.Binalar, indexId));
    }

    public void PutResearchToContent(int indexId, int reqLevel, Transform content)
    {
        // Teknolojiyi ekrana basıyoruz.
        GameObject techItem = Instantiate(TechItem, content);

        // Yalnızca gereksinimler basılacak. Content değeri basılan alan referansı ile aynı değil ise tıklama eventi olmayacak.
        if (ReferenceEquals(content, SelectedTechReqsContent))
            techItem.GetComponent<Button>().onClick.AddListener(() => TechnologyController.TC.ShowTechnologyPanelWithItem(TechnologyCategories.Araştırmalar, indexId));
        else
            TXT_TechName.text = base.GetLanguageText("XTeknolojiAğacı", base.GetLanguageText($"R{indexId}"), Environment.NewLine);

        // Teknolojinin ismini basacağız.
        techItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"R{indexId}");

        // Seviye yeterli mi?
        bool isLevelEnough = true;

        // Eğer gereken seviye 0dan büyük ise yazacağızç
        if (reqLevel > 0)
        {
            // Kullanıcının araştırma seviyesi.
            int userOwnedLevel = LoginController.LC.CurrentUser.UserResearches.Where(x => x.ResearchID == (Researches)indexId).Select(x => x.ResearchLevel).DefaultIfEmpty(0).FirstOrDefault();

            // Seviyeyi basacağımız alan.
            TMP_Text techReqLevel = techItem.transform.Find("TechReqLevel").GetComponent<TMP_Text>();

            // Kullanıcının görünen seviyesi gereksinimden fazla olamaz.
            userOwnedLevel = userOwnedLevel > reqLevel ? reqLevel : userOwnedLevel;

            // Seviyeyi basıyoruz.
            techReqLevel.text = $"{userOwnedLevel}/{reqLevel}";

            // Eğer seviye yeterli değil ise o renge boyuyoruz.
            if (userOwnedLevel < reqLevel)
            {
                // Metni kırmızıya boyuyoruz.
                techReqLevel.faceColor = CLR_NotEnoughLevel;

                // Öğrenilmedi!
                isLevelEnough = false;
            }
        }

        // Resim bilgisini buluyoruz.
        ResearchImageDTO researchImage = ResearchController.RC.ResearchWithImages.Find(x => x.Research == (Researches)indexId);

        // Resmi var ise resmi yüklüyoruz.
        if (researchImage != null)
            techItem.transform.Find("ItemImage").GetComponent<Image>().sprite = researchImage.ResearchImage;

        // Keşfedildi mi?
        if (isLevelEnough)
            techItem.transform.Find("IsInvented").gameObject.SetActive(TechnologyController.TC.IsInvented(TechnologyCategories.Araştırmalar, indexId));
    }

}
