using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Enums;
using Assets.Scripts.ApiModels;
using Assets.Scripts.Data;
using Assets.Scripts.Extends;

public class ResourceDetailController : MonoBehaviour, IPointerUpHandler
{
    public enum ResourceDetailTypes { ResourceMetal, ResourceCrystal, ResourceBoron, UpgradeResourceMetal, UpgradeResourceCrystal, UpgradeResourceBoron };

    [Header("Transparan olma hızı.")]
    [Range(0.1f, 5)]
    public float AnimationSpeed;

    [Header("Detayların bulunduğu panel.")]
    public GameObject ResourceDetailPanel;

    [Header("Detayların basılacağı alan.")]
    public TextMeshProUGUI ContentField;

    [Header("Datalarının basılacağı kaynak.")]
    public ResourceDetailTypes Resource;

    [Header("Değiştirilecek data.")]
    [TextArea]
    public string Template;

    private bool isOpening;
    private Image detailPanelImage;
    private Color detailPanelImageDefaultColor;
    private Color contentFieldDefaultColor;

    private void Awake()
    {
        // Panelin kapalı olduğundan emin oluyoruz.
        ResourceDetailPanel.SetActive(false);

        // Detay panelinin resmini alıyoruz. saydamlaştırmak için kullanacağız.
        detailPanelImage = ResourceDetailPanel.GetComponent<Image>();

        // Saydamlık veriyoruz.
        Color detailImageColor = detailPanelImage.color;

        // Varsayılan hedef değerlerini tutuyoruz.
        detailPanelImageDefaultColor = detailImageColor;

        // Saydam hale getiriyoruz.
        detailImageColor.a = 0;

        // Ve transparan bir şekilde renklendiriyoruz.
        detailPanelImage.color = detailImageColor;

        // Varsayılan renk bilgisini alıyoruz.
        contentFieldDefaultColor = ContentField.color;

        // Renk bilgisini alıyoruz.
        Color contentFieldColor = ContentField.color;

        // Saydamlığını kaldırıyoruz.
        contentFieldColor.a = 0;

        // Ve tamamen transparan hale getiriyoruz.
        ContentField.color = contentFieldColor;
    }

    private void Update()
    {
        // Panel açılıyor ise açılış animasyonunu yapıyoruz.
        if (isOpening)
        {
            // Eğer açık ise boş bir alana ya da kendi alanı dışında bir yere tıklanırsa kapatacağız.
            if (Input.GetMouseButtonDown(0))
            {
                // Eğer seçili olan obje kendisi değil ise geri dön.
                if (!ReferenceEquals(EventSystem.current.currentSelectedGameObject, ResourceDetailPanel.gameObject) &&
                    !ReferenceEquals(EventSystem.current.currentSelectedGameObject, gameObject))
                {
                    // Kapatıyoruz..
                    isOpening = false;

                    // Geri dön.
                    return;
                }
            }

            // Eğer zaten açık ise geri dön.
            if (ContentField.color.a < contentFieldDefaultColor.a)
            {
                // Renk bilgisini alıyoruz.
                Color contentFieldColor = ContentField.color;

                // saydamlığını azaltıyoruz.
                contentFieldColor.a += Time.deltaTime * AnimationSpeed;

                // Eğer default değerinden fazla transparan olduysa defaulta çekiyoruz.
                if (contentFieldColor.a > contentFieldDefaultColor.a)
                    contentFieldColor.a = contentFieldDefaultColor.a;

                // Ve rengini değiştiriyoruz.
                ContentField.color = contentFieldColor;
            }

            // Eğer zaten açık ise geri dön.
            if (detailPanelImage.color.a < detailPanelImageDefaultColor.a)
            {
                // Renk bilgisini alıyoruz.
                Color panelColor = detailPanelImage.color;

                // saydamlığını azaltıyoruz.
                panelColor.a += Time.deltaTime * AnimationSpeed;

                // Eğer default değerinden fazla transparan olduysa defaulta çekiyoruz.
                if (panelColor.a > detailPanelImageDefaultColor.a)
                    panelColor.a = detailPanelImageDefaultColor.a;

                // Ve rengini değiştiriyoruz.
                detailPanelImage.color = panelColor;
            }

        }
        else if (ResourceDetailPanel.activeSelf) // Eğer kapanıyorsa detay paneli açık ise.
        {
            // Renk bilgisini alıyoruz.
            Color contentFieldColor = ContentField.color;

            // saydamlığını azaltıyoruz.
            contentFieldColor.a -= Time.deltaTime * AnimationSpeed;

            // Ve rengini değiştiriyoruz.
            ContentField.color = contentFieldColor;

            // Renk bilgisini alıyoruz.
            Color panelColor = detailPanelImage.color;

            // saydamlığını azaltıyoruz.
            panelColor.a -= Time.deltaTime * AnimationSpeed;

            // Ve rengini değiştiriyoruz.
            detailPanelImage.color = panelColor;

            // Eğer yeterince saydam olduysa kapatıyoruz nesneyi.
            if (ContentField.color.a <= 0.0f && detailPanelImage.color.a <= 0.0f)
                ResourceDetailPanel.SetActive(false);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Eğer açık değil ise açıyoruz.
        if (!isOpening)
        {
            // Panelin açık olduğundan emin oluyoruz.
            ResourceDetailPanel.SetActive(true);

            // Ve artık panelin açıldığını söylüyoruz.
            isOpening = true;

            // Load resource Details.
            LoadResourceDetails();

        }
        else // Geri dön.
        {
            isOpening = false;
            return;
        }
    }

    public void LoadResourceDetails()
    {
        switch (Resource)
        {
            case ResourceDetailTypes.ResourceMetal:
                {

                    #region Metal Üretim hesaplaması.

                    // Kullanıcının metal binası var mı?
                    UserPlanetBuildingDTO metalBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.MetalMadeni);

                    // Saatlik hesaplanmış üretim.
                    double metalProducePerHour = StaticData.GetBuildingProdPerHour(Buildings.MetalMadeni, metalBuilding == null ? 0 : metalBuilding.BuildingLevel);

                    #endregion

                    #region Depo Kapasitesini hesaplıyoruz.

                    // Kullanıcının metal binası.
                    UserPlanetBuildingDTO metalStorageBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.MetalDeposu);

                    // Kullanıcının metal deposu.
                    double metalBuildingCapacity = StaticData.GetBuildingStorage(Buildings.MetalDeposu, metalStorageBuilding == null ? 0 : metalStorageBuilding.BuildingLevel);

                    #endregion

                    #region Ekrana basıyoruz.

                    string temp = Template.Replace("{0}", ResourceExtends.ConvertToDottedResource(GlobalPlanetController.GPC.CurrentPlanet.Metal));
                    temp = temp.Replace("{1}", ResourceExtends.ConvertToDottedResource(metalBuildingCapacity));
                    temp = temp.Replace("{2}", ResourceExtends.ConvertToDottedResource(metalProducePerHour));
                    ContentField.text = temp;

                    #endregion

                }
                break;
            case ResourceDetailTypes.ResourceCrystal:
                {

                    #region Kristal Üretim hesaplaması.

                    // Kullanıcının Kristal binası var mı?
                    UserPlanetBuildingDTO crystalBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.KristalMadeni);

                    // Saatlik hesaplanmış üretim.
                    double crystalProducePerHour = StaticData.GetBuildingProdPerHour(Buildings.KristalMadeni, crystalBuilding == null ? 0 : crystalBuilding.BuildingLevel);

                    #endregion

                    #region Depo Kapasitesini hesaplıyoruz.

                    // Kullanıcının metal binası.
                    UserPlanetBuildingDTO crystalStorageBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.KristalDeposu);

                    // Kullanıcının kristal deposu.
                    double crystalBuildingCapacity = StaticData.GetBuildingStorage(Buildings.MetalDeposu, crystalStorageBuilding == null ? 0 : crystalStorageBuilding.BuildingLevel);

                    #endregion

                    #region Ekrana basıyoruz.

                    string temp = Template.Replace("{0}", ResourceExtends.ConvertToDottedResource(GlobalPlanetController.GPC.CurrentPlanet.Crystal));
                    temp = temp.Replace("{1}", ResourceExtends.ConvertToDottedResource(crystalBuildingCapacity));
                    temp = temp.Replace("{2}", ResourceExtends.ConvertToDottedResource(crystalProducePerHour));
                    ContentField.text = temp;

                    #endregion

                }
                break;
            case ResourceDetailTypes.ResourceBoron:
                {

                    #region Boron Üretim hesaplaması.

                    // Kullanıcının Boron binası var mı?
                    UserPlanetBuildingDTO boronBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.BoronMadeni);

                    // Saatlik hesaplanmış üretim.
                    double boronProducePerHour = StaticData.GetBuildingProdPerHour(Buildings.BoronMadeni, boronBuilding == null ? 0 : boronBuilding.BuildingLevel);

                    #endregion

                    #region Depo Kapasitesini hesaplıyoruz.

                    // Kullanıcının metal binası.
                    UserPlanetBuildingDTO boronStorageBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.BoronDeposu);

                    // Kullanıcının kristal deposu.
                    double boronBuildingCapacity = StaticData.GetBuildingStorage(Buildings.BoronDeposu, boronStorageBuilding == null ? 0 : boronStorageBuilding.BuildingLevel);

                    #endregion

                    #region Ekrana basıyoruz.

                    string temp = Template.Replace("{0}", ResourceExtends.ConvertToDottedResource(GlobalPlanetController.GPC.CurrentPlanet.Boron));
                    temp = temp.Replace("{1}", ResourceExtends.ConvertToDottedResource(boronBuildingCapacity));
                    temp = temp.Replace("{2}", ResourceExtends.ConvertToDottedResource(boronProducePerHour));
                    ContentField.text = temp;

                    #endregion

                }
                break;
            case ResourceDetailTypes.UpgradeResourceMetal:
                {
                    int nextLevel = 1;
                    if (GlobalBuildingController.GBC.CurrentSelectedBuilding.UserPlanetBuilding != null)
                        nextLevel = GlobalBuildingController.GBC.CurrentSelectedBuilding.UserPlanetBuilding.BuildingLevel + 1;
                    ResourcesDTO cost = StaticData.CalculateCostBuilding(GlobalBuildingController.GBC.CurrentSelectedBuilding.BuildingType, nextLevel);
                    ContentField.text = ResourceExtends.ConvertToDottedResource(cost.Metal);
                }
                break;
            case ResourceDetailTypes.UpgradeResourceCrystal:
                {
                    int nextLevel = 1;
                    if (GlobalBuildingController.GBC.CurrentSelectedBuilding.UserPlanetBuilding != null)
                        nextLevel = GlobalBuildingController.GBC.CurrentSelectedBuilding.UserPlanetBuilding.BuildingLevel + 1;
                    ResourcesDTO cost = StaticData.CalculateCostBuilding(GlobalBuildingController.GBC.CurrentSelectedBuilding.BuildingType, nextLevel);
                    ContentField.text = ResourceExtends.ConvertToDottedResource(cost.Crystal);
                }
                break;
            case ResourceDetailTypes.UpgradeResourceBoron:
                {
                    int nextLevel = 1;
                    if (GlobalBuildingController.GBC.CurrentSelectedBuilding.UserPlanetBuilding != null)
                        nextLevel = GlobalBuildingController.GBC.CurrentSelectedBuilding.UserPlanetBuilding.BuildingLevel + 1;
                    ResourcesDTO cost = StaticData.CalculateCostBuilding(GlobalBuildingController.GBC.CurrentSelectedBuilding.BuildingType, nextLevel);
                    ContentField.text = ResourceExtends.ConvertToDottedResource(cost.Boron);
                }
                break;
            default:
                break;
        }
    }

}
