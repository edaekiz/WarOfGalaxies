using Assets.Scripts.ApiModels;
using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
using System.Collections;
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

    [Header("Sunucuya istek gönderiliyor mu?")]
    public bool IsSending;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public IEnumerator LoadDefenseDetails(Defenses defense)
    {
        // Gemi bilgisi.
        CurrentDefense = defense;

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
        DefenseDTO defenseInfo = StaticData.DefenseData.Find(x => x.DefenseID == defense);

        // Maliyeti set ediyoruz.
        bool isMathEnough = base.SetResources(defenseInfo.Cost);

        // Eğer materyal yeterli ise butonu açıyoruz değil ise kapatıyoruz.
        if (isMathEnough && !IsSending)
            ProduceButton.interactable = true;
        else
            ProduceButton.interactable = false;

        // Robot fab buluyoruz.
        UserPlanetBuildingDTO robotFac = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.BuildingId == Buildings.RobotFabrikası && x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Üretim süresini basıyoruz.
        double countdown = StaticData.CalculateDefenseCountdown(defense, robotFac == null ? 0 : robotFac.BuildingLevel);

        // Üretim süresini basıyoruz.
        ItemCountdown.text = TimeExtends.GetCountdownText(TimeSpan.FromSeconds(countdown));

        // 1 saniye bekliyoruz tekrar yenileyeceğiz.
        yield return new WaitForSecondsRealtime(1);

        // Gemi bilgisini yüklüyoruz.
        StartCoroutine(LoadDefenseDetails(defense));
    }

    public void AddToQueue()
    {
        string quantityStr = QuantityField.text;
        int quantity = 0;

        if (int.TryParse(quantityStr, out quantity))
        {
            // Gönderiliyor mu?
            IsSending = true;

            // Butonu kapatıyoruz.
            ProduceButton.interactable = false;

            StartCoroutine(ApiService.API.Post("AddDefenseToDefenseQueue", new DefenseAddQueueRequestDTO
            {
                Quantity = quantity,
                DefenseID = CurrentDefense,
                UserPlanetID = GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId
            }, (ApiResult response) =>
            {
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
                    if (LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Count == 0)
                        progress.LastVerifyDate = DateTime.UtcNow;

                    // Üretime ekliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Add(progress);

                    // Kaynakları güncelliyoruz.
                    LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == responseData.UserPlanetID).SetPlanetResources(responseData.PlanetResources);

                    // Paneli yeniliyoruz.
                    DefenseQueueController.DQC.RefreshDefenseQueue();

                    // Detay panelini kapatıyoruz.
                    base.ClosePanel();
                }

            }));
        }
    }

}
