using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;

public class ResourceDetailController : MonoBehaviour, IPointerUpHandler
{
    [Header("Transparan olma hızı.")]
    [Range(0.1f, 5)]
    public float AnimationSpeed;

    [Header("Detayların bulunduğu panel.")]
    public GameObject ResourceDetailPanel;

    [Header("Detayların basılacağı alan.")]
    public TextMeshProUGUI ContentField;

    [Header("Datalarının basılacağı kaynak.")]
    public ResourceTypes Resource;

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
            if (ContentField.color.a <= 0 && detailPanelImage.color.a <= 0)
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
            LoadResouceData();

        }
        else // Geri dön.
        {
            isOpening = false;
            return;
        }
    }

    public void LoadResouceData()
    {
        switch (Resource)
        {
            case ResourceTypes.Metal:
                {

                    #region Metal Üretim hesaplaması.

                    int metalQuantity = 0;

                    UserPlanetBuildingDTO metalBuilding = Data.UserPlanetBuidings.Find(x => x.BuildingID == Buildings.MetalMadeni);

                    if (metalBuilding != null)
                    {
                        BuildingLevelDTO upgrade = Data.BuildingLevels.Find(x => x.BuildingID == Buildings.MetalMadeni && x.BuildingLevel == metalBuilding.BuildingLevel);

                        if (upgrade != null)
                            metalQuantity = upgrade.BuildingValue;
                    }

                    #endregion

                    #region Depo Kapasitesini hesaplıyoruz.

                    int metalCapacityQuantity = 0;

                    UserPlanetBuildingDTO metalCapacityBuilding = Data.UserPlanetBuidings.Find(x => x.BuildingID == Buildings.MetalDeposu);

                    if (metalBuilding != null)
                    {
                        BuildingLevelDTO upgrade = Data.BuildingLevels.Find(x => x.BuildingID == Buildings.MetalDeposu && x.BuildingLevel == metalBuilding.BuildingLevel);

                        if (upgrade != null)
                            metalCapacityQuantity = upgrade.BuildingValue;
                    }

                    #endregion

                    #region Ekrana basıyoruz.

                    ContentField.text = $"Anlık\n<color=white>{ResourceController.RC.MetalQuantity}</color>\nDepo Kapasitesi\n<color=white>{metalCapacityQuantity}</color>\nSaaatlik Üretim\n<color=white>{metalQuantity}</color>";

                    #endregion

                }
                break;
            case ResourceTypes.Crystal:
                break;
            case ResourceTypes.Boron:
                break;
            default:
                break;
        }
    }

}
