using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseDetailItemPanel : BasePanelController
{
    [Header("Savunmanın ismi ile miktarı.")]
    public TMP_Text ItemNameWithQuantity;

    [Header("Savunmanın kısa açıklaması.")]
    public TMP_Text ItemDescription;

    [Header("Üretim süresi.")]
    public TMP_Text ItemCountdown;

    [Header("Üretim butonu")]
    public Button ProduceButton;

    [Header("Savunma ikonu.")]
    public Image ItemImage;

    [Header("Aktif savunma bilgisi")]
    public Defenses CurrentDefense;

    [Header("Üretilecek miktar")]
    public TMP_InputField QuantityField;

    [Header("Eğer yükseltme yapılamaz ise buradaki uyarı açılacak")]
    public TMP_Text TXT_Alert;

    [Header("Sunucuya istek gönderiliyor mu?")]
    public bool IsSending;

    /// <summary>
    /// Defans bilgisini tutuyoruz.
    /// </summary>
    DefenseDataDTO DefenseInfo { get; set; }

    protected override void Start()
    {
        base.Start();
        InvokeRepeating("RefreshState", 0, 1);
    }

    public void LoadDefenseDetails(Defenses defense)
    {
        // Gemi bilgisi.
        CurrentDefense = defense;

        #region Dataları yüklüyoruz.

        // Savunma açıklaması.
        ItemDescription.text = base.GetLanguageText($"DD{(int)defense}");

        // Aktif gemi miktarı.
        UserPlanetDefenseDTO currentDefenseCount = LoginController.LC.CurrentUser.UserPlanetDefenses.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.DefenseId == defense);

        // Savunmaya sahip ise miktarı değil ise 0.
        string quantity = currentDefenseCount == null ? "0" : currentDefenseCount.DefenseCount.ToString();

        // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
        ItemNameWithQuantity.text = $"{base.GetLanguageText($"D{(int)defense}")} <color=orange>({quantity})</color>";

        // Savunma resmini basıyoruz.
        ItemImage.sprite = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == defense).DefenseImage;

        // Maliyeti.
        this.DefenseInfo = DataController.DC.GetDefense(defense);

        #endregion
    }

    public void RefreshState()
    {
        // Robot fab buluyoruz.
        UserPlanetBuildingDTO robotFac = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.BuildingId == Buildings.RobotFabrikası && x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Üretim süresini basıyoruz.
        double countdown = DataController.DC.CalculateDefenseCountdown(CurrentDefense, robotFac == null ? 0 : robotFac.BuildingLevel);

        // Üretim süresini basıyoruz.
        ItemCountdown.text = TimeExtends.GetCountdownText(TimeSpan.FromSeconds(countdown));

        #region Üretim koşulları sağlanıyor mu?

        // Eğer üretim yapılabiliyor ise true olacak.
        bool canProduce = true;

        // Eğer gönderim var ise butonu kapatıyoruz.
        if (IsSending)
            canProduce = false;

        // Maliyeti set ediyoruz.
        bool isMathEnough = base.SetResources(new ResourcesDTO(DefenseInfo.CostMetal, DefenseInfo.CostCrystal, DefenseInfo.CostBoron));

        // Eğer materyal yeterli ise butonu açıyoruz değil ise kapatıyoruz.
        if (!isMathEnough)
            canProduce = false;

        #region Koşullar sağlanıyor mu?

        // Koşullar sağlanıyor mu?
        bool isCondionsTrue = TechnologyController.TC.IsInvented(TechnologyCategories.Savunmalar, (int)CurrentDefense);

        // Eğer şartlar uygun değil ise.
        if (!isCondionsTrue)
        {
            // Butonu kapatıyoruz.
            canProduce = false;

            // Texti de ekrana basıyoruz.
            TXT_Alert.text = base.GetLanguageText("SavunmaKoşulOlumsuz");
        }

        #endregion

        #region Robot Fabrikası var mı? Varsa seviyesine bakıyoruz.

        // Şartlar uygun olduğunda bu kontrolü yapacağız.
        if (isCondionsTrue)
        {
            // Yükseltebiliyor muyuz diye kontrol ediyoruz.
            if (!DefensePanelController.DPC.CheckRobotFactoryBuilding())
            {
                // Yükseltemiyoruz demekki.
                canProduce = false;

                // Uyarıyı da buraya basacağız.
                TXT_Alert.text = DefensePanelController.DPC.TXT_Alert.text;
            }
            else
                TXT_Alert.text = string.Empty;
        }

        #endregion

        // Üretim yapılabiliyor ise butonu açıyoruz.
        if (canProduce)
            ProduceButton.interactable = true;
        else
            ProduceButton.interactable = false;

        #endregion
    }

    public void AddToQueue()
    {
        string quantityStr = QuantityField.text;

        if (int.TryParse(quantityStr, out int quantity))
        {
            // Gönderiliyor mu?
            IsSending = true;

            // Butonu kapatıyoruz.
            ProduceButton.interactable = false;

            // Yükleniyor ekranını açıyoruz.
            LoadingController.LC.ShowLoading();

            ApiService.API.Post("AddDefenseToDefenseQueue", new DefenseAddQueueRequestDTO
            {
                Quantity = quantity,
                DefenseID = CurrentDefense,
                UserPlanetID = GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId
            }, (ApiResult response) =>
            {
                // Yükleniyor ekranını kapatıyoruz.
                LoadingController.LC.CloseLoading();

                // Eğer başarılı ise.
                if (response.IsSuccess)
                {
                    // Gelen datayı alıyoruz.
                    DefenseAddQueueResponseDTO responseData = response.GetData<DefenseAddQueueResponseDTO>();

                    UserPlanetDefenseProgDTO progress = new UserPlanetDefenseProgDTO
                    {
                        DefenseCount = responseData.Quantity,
                        DefenseId = responseData.DefenseID,
                        UserPlanetId = responseData.UserPlanetID
                    };

                    // Eğer üretim yok ise tarih veriyoruz.
                    if (LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Where(x => x.UserPlanetId == responseData.UserPlanetID).Count() == 0)
                        progress.LastVerifyDate = DateTime.UtcNow;

                    // Üretime ekliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Add(progress);

                    // Kaynakları güncelliyoruz.
                    LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == responseData.UserPlanetID).SetPlanetResources(responseData.PlanetResources);

                    // Paneli yeniliyoruz.
                    if (DefenseQueueController.DQC != null)
                        DefenseQueueController.DQC.RefreshDefenseQueue();

                    // Detay panelini kapatıyoruz.
                    base.ClosePanel();
                }

            });
        }
    }

    public void ShowConditions() => TechnologyController.TC.ShowTechnologyPanelWithItem(TechnologyCategories.Savunmalar, (int)CurrentDefense);

}
