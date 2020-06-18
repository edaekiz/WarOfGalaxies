using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Linq;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelController : BasePanelController
{
    [Header("Binayı yükseltme butonu.")]
    public Button UpgradeButton;

    [Header("Binayı yıkma butonu.")]
    public Button DowngradeButton;

    [Header("Binanın resmi buraya yüklenecek.")]
    public Image BuildingImage;

    [Header("Binanın ismini tutuyoruz.")]
    public TMP_Text BuildingName;

    [Header("Binanın açıklaması.")]
    public TMP_Text BuildingDescription;

    [Header("Binanın seviyesini basıyoruz.")]
    public TMP_Text BuildingLevel;

    [Header("Binanın yükseltme süresini buraya basacağız.")]
    public TMP_Text BuildingUpgradeTime;

    [Header("Yükseltme sırasındaki geri sayım.")]
    public TMP_Text BuildingCountdown;

    [Header("Bir uyarı vereceğimiz de burayı kullanacağız.")]
    public TMP_Text TXT_Alert;

    public IEnumerator LoadData(Buildings building)
    {
        #region Yükseltme yapılabilir mi? Kaynak kontrolü olmadan.

        // Yükseltebilir mi?
        bool canUpgrade = true;

        // Eğer bir bina yükseltiliyor ise true olacak.
        bool isAlreadyUpgrading = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Any(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Eğer zaten upgrade ediliyor ise upgrade edilemez.
        if (isAlreadyUpgrading)
            canUpgrade = false;

        #endregion

        #region Bina detaylarını basıyoruz.

        // Yükseltilen bina aktif bina ise değer dolu olacak.
        UserPlanetBuildingUpgDTO upgrade = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == building);

        // Eğer seçili olan bina yükseltiliyor ise ekrana geri sayımı basacağız.
        if (upgrade == null)
            BuildingCountdown.text = string.Empty;
        else
            BuildingCountdown.text = TimeExtends.GetCountdownText((upgrade.EndDate - DateTime.UtcNow));

        // Kullanıcının binassı.
        UserPlanetBuildingDTO userBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == building);

        // Kullanıcının binası yok ise 0. kademe yazacak var ise tesisin seviyesini yazacak.
        if (userBuilding == null)
            BuildingLevel.text = base.GetLanguageText("Kademe", "0");
        else
            BuildingLevel.text = base.GetLanguageText("Kademe", userBuilding.BuildingLevel.ToString());

        // Bir sonraki seviye.
        int nextLevel = userBuilding == null ? 1 : userBuilding.BuildingLevel + 1;

        // Eğer bir sonraki seviye 1 ise bu bina henüz kurulmamıştır. Kurulmadıysa yıkamayız o yüzden butonu kapatıyoruz.
        if (nextLevel == 1)
            DowngradeButton.interactable = false;
        else
        {
            // EĞer yükseltilmiyor ise butonu açabiliriz. Ancak aksi durumda kapatacağız.
            if (upgrade == null)
                DowngradeButton.interactable = true;
            else
                DowngradeButton.interactable = false;
        }

        // Binanın ismini basıyoruz.
        BuildingName.text = base.GetLanguageText($"B{(int)building}");

        // Bina açıklamasını basıyoruz.
        BuildingDescription.text = base.GetLanguageText($"BD{(int)building}");

        // Bina resim bilgisi.
        BuildingsWithImage buildingImage = GlobalBuildingController.GBC.BuildingWithImages.Find(x => x.Building == building);

        // Eğer resmi var ise resmi basıyoruz.
        if (buildingImage != null)
            BuildingImage.sprite = buildingImage.BuildingImage;

        // Robot fabrikasını buluyoruz.
        UserPlanetBuildingDTO robotBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

        // Yükseltme süresi.
        double upgradeTime = DataController.DC.CalculateBuildingUpgradeTime(building, nextLevel, robotBuilding == null ? 0 : robotBuilding.BuildingLevel);

        // Ekrana basıyoruz.
        BuildingUpgradeTime.text = $"<color=#FF8B00>{base.GetLanguageText("YapımSüresi")}:</color>{Environment.NewLine}{TimeExtends.GetCountdownText(TimeSpan.FromSeconds(upgradeTime))}";

        // Kaynak kontrolü ve koşulları sağlıyor mu kontorlü
        ResourcesDTO resources = DataController.DC.CalculateCostBuilding(building, nextLevel);

        #endregion

        #region Kaynak ikonları üstüne tıklandığında yazacak olan detaylar.

        // Eğer metal kaynağı 0 dan fazla ise yazabiliriz. Ancak değil ise kapatacağız.
        if (resources.Metal > 0)
        {
            // Metal miktarını basıyoruz.
            MetalDetail.ContentField.text = ResourceExtends.ConvertToDottedResource(resources.Metal);

            // Açık ise açmaya gerek yok paneli
            if (!MetalDetail.gameObject.activeSelf)
                MetalDetail.gameObject.SetActive(true);
        }
        else // Aksi durumda kapatıyoruz.
            MetalDetail.gameObject.SetActive(false);

        // Eğer kristal kaynağı 0 dan fazla ise yazabiliriz. Ancak değil ise kapatacağız.
        if (resources.Crystal > 0)
        {
            CrystalDetail.ContentField.text = ResourceExtends.ConvertToDottedResource(resources.Crystal);

            // Açık değil ise açacağız.
            if (!CrystalDetail.gameObject.activeSelf)
                CrystalDetail.gameObject.SetActive(true);
        }
        else
            CrystalDetail.gameObject.SetActive(false);

        if (resources.Boron > 0)
        {
            // Boron miktarını basıyoruz.
            BoronDetail.ContentField.text = ResourceExtends.ConvertToDottedResource(resources.Boron);

            // Açık ise açmaya gerek yok paneli
            if (!BoronDetail.gameObject.activeSelf)
                BoronDetail.gameObject.SetActive(true);
        }
        else
            BoronDetail.gameObject.SetActive(false);

        #endregion

        #region Bina kaynak gereksinimlerini kontrol ediyoruz.

        // Eğer yükseltilebilir ise kaynakları kontrol ediyoruz.
        if (canUpgrade)
        {
            // Gezegendeki kaynaklar yeterli değil ise butonu kapatacağız.
            if (GlobalPlanetController.GPC.CurrentPlanet.Metal < resources.Metal || GlobalPlanetController.GPC.CurrentPlanet.Crystal < resources.Crystal || GlobalPlanetController.GPC.CurrentPlanet.Boron < resources.Boron)
                canUpgrade = false;
        }

        #endregion

        // Kaynakları atıyoruz.
        base.SetResources(resources);

        #region Tersane | Araştırma Lab | Robot Fabrikası.

        // Tersane araştırma ve robot fabrikasını yükseltirken üretimleri mevcut mu diye kontrol etmeliyiz.
        if (building == Buildings.Tersane || building == Buildings.ArastirmaLab || building == Buildings.RobotFabrikası)
        {
            bool isProgExits = false;
            string textMessage = string.Empty;

            switch (building)
            {
                case Buildings.RobotFabrikası:
                    isProgExits = LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);
                    textMessage = base.GetLanguageText("SavunmaÜretimiMevcut");
                    break;
                case Buildings.Tersane:
                    isProgExits = LoginController.LC.CurrentUser.UserPlanetShipProgs.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId); ;
                    textMessage = base.GetLanguageText("GemiÜretimiMevcut");
                    break;
                case Buildings.ArastirmaLab:
                    isProgExits = LoginController.LC.CurrentUser.UserResearchProgs.Count > 0;
                    textMessage = base.GetLanguageText("AraştırmaDevamEdiyor");
                    break;
            }

            // Eğer bir üretim mevcut ise bina yükseltilemz.
            if (isProgExits)
            {
                // Artık bu yükseltme yapılamaz.
                canUpgrade = false;

                // Eğer uyarı açık değil ise açıyoruz uyarıyı açıyoruz.
                if (!TXT_Alert.gameObject.activeSelf)
                {
                    // Uyarıyı açıyoruz.
                    TXT_Alert.gameObject.SetActive(true);

                    // Uyarıyı basıyoruz.
                    TXT_Alert.text = textMessage;
                }
            }
            else // Eğer mevcut değil ise ve uyarı paneli açık ise paneli kapatıyoruz.
            {
                // Eğer uyarı açık ise kapatıyoruz.
                if (TXT_Alert.gameObject.activeSelf)
                {
                    // Uyarıyı açıyoruz.
                    TXT_Alert.gameObject.SetActive(false);
                }
            }
        }

        #endregion

        #region Yükseltme kontrolü yapılıyor ise zaten yapılıyor yazacak. Aksi durumda butonu açacağız ya da kapatacağız.

        // Eğer zaten yükseltiliyor ise butonu kapat ve texti güncelle.
        if (!canUpgrade)
        {
            // Butonu kapatıyoruz.
            UpgradeButton.interactable = false;

            // Texti değiştiriyoruz.
            if (isAlreadyUpgrading)
                UpgradeButton.GetComponentInChildren<TMP_Text>().text = base.GetLanguageText("Yükseltiliyor");
            else
                UpgradeButton.GetComponentInChildren<TMP_Text>().text = base.GetLanguageText("Yükselt");
        }
        else // Eğer aksi durumdaysa yükseltme butonunu açıyoruz.
        {
            // Butonu açıyoruz.
            UpgradeButton.interactable = true;

            // Texti değiştiriyoruz.
            UpgradeButton.GetComponentInChildren<TMP_Text>().text = base.GetLanguageText("Yükselt");
        }

        #endregion

        // 1 saniye bekliyoruz sonra kendisini yeniden çağıracağız.
        yield return new WaitForSecondsRealtime(1);

        // Tekrar çağırıyoruz.
        StartCoroutine(LoadData(building));
    }

    public void UpgradeBuilding()
    {
        StartCoroutine(ApiService.API.Post("UpgradeUserPlanetBuilding", new UserPlanetUpgradeBuildingDTO
        {
            BuildingID = (int)GlobalBuildingController.GBC.CurrentSelectedBuilding.BuildingType,
            UserPlanetID = GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId
        }, (ApiResult response) =>
        {
            // Eğer başarılı ise yükseltmeyi başlatacağız.
            if (response.IsSuccess)
            {
                // Gelen paketi açıyoruz.
                UserPlanetBuildingUpgDTO upgradeInfo = response.GetData<UserPlanetBuildingUpgDTO>();

                // Hesaplamasını yapıyoruz.
                upgradeInfo.CalculateDates(upgradeInfo.PassedTime, upgradeInfo.LeftTime);

                // Gezegeni buluyoruz.
                UserPlanetDTO userPlanet = LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == upgradeInfo.UserPlanetId);

                // Gezegenin kaynaklarını güncelliyoruz.
                userPlanet.SetPlanetResources(upgradeInfo.PlanetResources);

                // Gezegene yükseltmeyi ekliyoruz.
                LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Add(upgradeInfo);

                // Seçilen binanın datalarını yeniler.
                GlobalBuildingController.GBC.CurrentSelectedBuilding.LoadBuildingDetails();

                // Paneli kapatıyoruz.
                base.ClosePanel();
            }
        }));
    }

    private void OnDestroy()
    {
        // Binanın seçimini kaldırıyoruz.
        GlobalBuildingController.GBC.DeSelectBuilding();
    }

}
