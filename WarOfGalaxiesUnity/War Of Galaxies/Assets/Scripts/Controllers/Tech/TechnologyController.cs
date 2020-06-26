using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TechnologyController : MonoBehaviour
{
    public static TechnologyController TC { get; set; }
    private void Awake()
    {
        if (TC == null)
            TC = this;
        else
            Destroy(gameObject);
    }

    [Header("Sistemde bulunan teknolojiler.")]
    public List<TechnologyDTO> Technologies;

    [Header("Son seçilen kategoriyi tutar.")]
    public TechnologyCategories LastSelectedCategory = TechnologyCategories.Araştırmalar;

    // Start is called before the first frame update
    void Start()
    {
        LoadTechnologies();
    }

    public void LoadTechnologies()
    {
        ApiService.API.Post("GetTechnologyTree", null, (ApiResult response) =>
        {
            if (response.IsSuccess)
            {             // Teknoloji listesini alıyoruz.
                List<TechnologyDTO> technologies = response.GetDataList<TechnologyDTO>();

                // Listeyi güncelliyruz.
                Technologies = technologies;

                // Ve yükleme ekranını bir arttırıyoruz.
                LoadingController.LC.IncreaseLoadCount();
            }
        });
    }

    public void ShowTechnologyPanel()
    {
        GameObject techPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.TechnologyPanel);
        techPanel.GetComponent<TechnologyPanelController>().LoadTechnologies(LastSelectedCategory);
    }

    public void ShowTechnologyPanelWithItem(TechnologyCategories category,int indexId)
    {
        // Hemen ardından seçilen teknolojiyi açıyoruz.
        GameObject techPanelDetail = GlobalPanelController.GPC.ShowPanel(PanelTypes.TechnologyDetailPanel);

        // Ve detayını gösteriyoruz.
        techPanelDetail.GetComponent<TechnologyDetailPanelController>().LoadDetails(category, indexId);
    }

    public bool IsInvented(TechnologyCategories category, int indexId)
    {
        // Gereksinimleri buluyoruz.
        List<TechnologyDTO> techReqs = TechnologyController.TC.Technologies.Where(x => x.TechnologyCategoryId == (int)category && x.IndexId == indexId).ToList();

        // Eğer teknoloji yok ise true dönüyoruz.
        if (techReqs.Count == 0)
            return true;

        // Her bir gereksinimi dönüyoruz.
        foreach (TechnologyDTO techReq in techReqs)
        {
            // Teknoloji.
            TechnologyCategories techCat = (TechnologyCategories)techReq.RequiredTechnologyCategoryId;

            // Gereksinimi kontrol ediyoruz.
            switch (techCat)
            {
                case TechnologyCategories.Araştırmalar:
                    // Kullanıcının bu araştırması mevcut mu?
                    if (!LoginController.LC.CurrentUser.UserResearches.Exists(x => x.ResearchID == (Researches)techReq.RequiredIndexId && x.ResearchLevel >= techReq.RequiredLevel))
                        return false;
                    break;
                case TechnologyCategories.Binalar:
                    if (!LoginController.LC.CurrentUser.UserPlanetsBuildings.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == (Buildings)techReq.RequiredIndexId && x.BuildingLevel >= techReq.RequiredLevel))
                        return false;
                    break;
            }

            // Döngüdeki teknoloji keşfedilebilir mi?
            if (!IsInvented((TechnologyCategories)techReq.RequiredTechnologyCategoryId, techReq.RequiredIndexId))
                return false;

        }

        // Eğer buraya kadar gelirse keşfedilebilir demek oluyor.
        return true;
    }
}
