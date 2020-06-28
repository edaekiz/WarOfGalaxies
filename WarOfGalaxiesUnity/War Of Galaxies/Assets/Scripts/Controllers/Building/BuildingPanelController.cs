using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelController : BasePanelController
{
    [Header("Binayı yükseltme butonu.")]
    public Button UpgradeButton;

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

    [Header("Bir uyarı vereceğimiz de burayı kullanacağız.")]
    public TMP_Text TXT_Alert;

    [Header("Gösterilen bina bilgisi.")]
    public Buildings CurrentBuilding;

    [Header("Bina yükseltme sesi.")]
    public string SND_BuildingUpgrade;

    protected override void Start()
    {
        base.Start();
        InvokeRepeating("RefreshState", 0, 1);
    }

    public void LoadBuildingDetails(Buildings building)
    {
        // Bina bilgisini basıyoruz.
        this.CurrentBuilding = building;

        // Binanın ismini basıyoruz.
        BuildingName.text = base.GetLanguageText($"B{(int)building}");

        // Bina açıklamasını basıyoruz.
        BuildingDescription.text = base.GetLanguageText($"BD{(int)building}");

        // Bina resim bilgisi.
        BuildingsWithImage buildingImage = GlobalBuildingController.GBC.BuildingWithImages.Find(x => x.Building == building);

        // Eğer resmi var ise resmi basıyoruz.
        if (buildingImage != null)
            BuildingImage.sprite = buildingImage.BuildingImage;
    }

    public void RefreshState()
    {
        #region Bina detaylarını basıyoruz.

        // Yükseltilen bina aktif bina ise değer dolu olacak.
        UserPlanetBuildingUpgDTO upgrade = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == CurrentBuilding);

        // Kullanıcının binassı.
        UserPlanetBuildingDTO userBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == CurrentBuilding);

        // Kullanıcının binası yok ise 0. kademe yazacak var ise tesisin seviyesini yazacak.
        if (userBuilding == null)
            BuildingLevel.text = base.GetLanguageText("Kademe", "0");
        else
            BuildingLevel.text = base.GetLanguageText("Kademe", userBuilding.BuildingLevel.ToString());

        // Bir sonraki seviye.
        int nextLevel = userBuilding == null ? 1 : userBuilding.BuildingLevel + 1;

        // Robot fabrikasını buluyoruz.
        UserPlanetBuildingDTO robotBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

        // Yükseltme süresi.
        double upgradeTime = DataController.DC.CalculateBuildingUpgradeTime(CurrentBuilding, nextLevel, robotBuilding == null ? 0 : robotBuilding.BuildingLevel);

        // Eğer seçili olan bina yükseltiliyor ise ekrana geri sayımı basacağız.
        if (upgrade == null)
            BuildingUpgradeTime.text = $"<color=#FF8B00>{base.GetLanguageText("YapımSüresi")}:</color>{Environment.NewLine}{TimeExtends.GetCountdownText(TimeSpan.FromSeconds(upgradeTime))}";
        else
            BuildingUpgradeTime.text = $"<color=#FF8B00>{base.GetLanguageText("YapımSüresi")}:</color>{Environment.NewLine}{TimeExtends.GetCountdownText((upgrade.EndDate - DateTime.UtcNow))}";

        // Kaynak kontrolü ve koşulları sağlıyor mu kontorlü
        ResourcesDTO resources = DataController.DC.CalculateCostBuilding(CurrentBuilding, nextLevel);

        // Kaynakları atıyoruz.
        base.SetResources(resources);

        #endregion

        #region Yükseltme yapılabilir mi? Kaynak kontrolü olmadan.

        // Yükseltebilir mi?
        bool canUpgrade = true;

        // Eğer bir bina yükseltiliyor ise true olacak.
        bool isAlreadyUpgrading = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Any(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Eğer zaten upgrade ediliyor ise upgrade edilemez.
        if (isAlreadyUpgrading)
            canUpgrade = false;

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

        #region Koşullar sağlanıyor mu?

        // Koşullar sağlanıyor mu?
        bool isCondionsTrue = TechnologyController.TC.IsInvented(TechnologyCategories.Binalar, (int)CurrentBuilding);

        // Eğer şartlar uygun değil ise.
        if (!isCondionsTrue)
        {
            // Butonu kapatıyoruz.
            canUpgrade = false;

            // Texti de ekrana basıyoruz.
            TXT_Alert.text = base.GetLanguageText("BinaKoşulOlumsuz");
        }

        #endregion

        #region Tersane | Araştırma Lab | Robot Fabrikası.

        // Şartlar uygun ise bu kontrolü yapıyoruz.
        if (isCondionsTrue)
        {
            // Tersane araştırma ve robot fabrikasını yükseltirken üretimleri mevcut mu diye kontrol etmeliyiz.
            if (CurrentBuilding == Buildings.Tersane || CurrentBuilding == Buildings.ArastirmaLab || CurrentBuilding == Buildings.RobotFabrikası)
            {
                bool isProgExits = false;

                switch (CurrentBuilding)
                {
                    case Buildings.RobotFabrikası:
                        isProgExits = LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);
                        TXT_Alert.text = base.GetLanguageText("SavunmaÜretimiMevcut");
                        break;
                    case Buildings.Tersane:
                        isProgExits = LoginController.LC.CurrentUser.UserPlanetShipProgs.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);
                        TXT_Alert.text = base.GetLanguageText("GemiÜretimiMevcut");
                        break;
                    case Buildings.ArastirmaLab:
                        isProgExits = LoginController.LC.CurrentUser.UserResearchProgs.Count > 0;
                        TXT_Alert.text = base.GetLanguageText("AraştırmaDevamEdiyor");
                        break;
                }

                // Eğer bir üretim mevcut ise bina yükseltilemz.
                if (isProgExits)
                    canUpgrade = false;
                else // Eğer mevcut değil ise ve uyarı paneli açık ise paneli kapatıyoruz.
                    TXT_Alert.text = string.Empty;
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
    }


    public void UpgradeBuilding()
    {
        ApiService.API.Post("UpgradeUserPlanetBuilding", new UserPlanetUpgradeBuildingDTO
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

                // Yükseltme sesini çalıyoruz.
                AudioController.AC.PlaySoundOnCamera(SND_BuildingUpgrade);

                // Paneli kapatıyoruz.
                base.ClosePanel();
            }
        });
    }

    private void OnDestroy()
    {
        // Binanın seçimini kaldırıyoruz.
        GlobalBuildingController.GBC.DeSelectBuilding();
    }

    public void ShowConditions() => TechnologyController.TC.ShowTechnologyPanelWithItem(TechnologyCategories.Binalar, (int)CurrentBuilding);

}
