using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipyardPanelController : BasePanelController
{
    /// <summary>
    /// Açık olan araştırma paneli
    /// </summary>
    public static ShipyardPanelController SPC { get; set; }

    [Header("Basılacak olan gemiler.")]
    public GameObject ShipyardItem;

    [Header("Gemileri buraya basacağız.")]
    public Transform ShipyardItemContent;

    [Header("Gemi üretemiyor ise burada hata vereceğiz.")]
    public TMP_Text TXT_Alert;

    private void Awake()
    {
        if (SPC == null)
            SPC = this;
        else
            Destroy(gameObject);
    }

    protected override void Start()
    {
        base.Start();

        // Kontrol ediyoruz üretim yapabilmek için şartlar uygun mu?
        StartCoroutine(CheckIsShipyardExists());
    }

    public void LoadAllShips()
    {
        // Bütün eski araştırmaları sil.
        foreach (Transform child in ShipyardItemContent)
            Destroy(child.gameObject);

        // Bütün gemileri teker teker basıyoruz.
        for (int ii = 0; ii < DataController.DC.SystemData.Ships.Count; ii++)
        {
            // Gemi bilgisi.
            ShipDataDTO ship = DataController.DC.SystemData.Ships[ii];

            // Gemiyi oluşturuyoruz.
            GameObject shipyardItem = Instantiate(ShipyardItem, ShipyardItemContent);

            // Geminin controlleri.
            ShipyardItemController sic = shipyardItem.GetComponent<ShipyardItemController>();

            // Detayları yükle.
            sic.LoadShipDetails((Ships)ship.ShipId);
        }

        // Hepsini kurduktan sonra kuyruğu yeniliyoruz.
        ShipyardQueueController.SQC.RefreshShipyardQueue();
    }

    /// <summary>
    /// Kontrol ediyoruz tersanesi var mı?
    /// </summary>
    public IEnumerator CheckIsShipyardExists()
    {
        // Giriş yapana kadar bekliyoruz.
        yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

        // Kontrol ediyoruz tersane var mı bu gezegende?
        bool isResearchLabExists = LoginController.LC.CurrentUser.UserPlanetsBuildings.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.Tersane);

        // Eğer gemi üretemiyor isek bir hatadan dolayı 1 saniye sonra tekrar kontrol ediyoruz.
        if (!CheckShipyardBuilding())
        {
            // 1 saniye beklemeliyiz.
            yield return new WaitForSecondsRealtime(1);

            // Tekrar kendisini çağırıyoruz.
            StartCoroutine(CheckIsShipyardExists());
        }
    }

    public bool CheckShipyardBuilding()
    {
        // Eğer tersane yükseltiliyor ise bu buton açılacak.
        bool isTersaneUpgrading = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.Tersane);

        // Tersane yükseltiliyor ise uyarı panelini açacağız.
        if (isTersaneUpgrading)
        {
            // Uyarıyı basıyoruz.
            TXT_Alert.text = base.GetLanguageText("TersaneYükseltmeVar");

            // Üretim yapılamaz.
            return false;
        }
        else
        {
            // Tersane var mı diye kontrol ediyoruz.
            bool isTersaneExists = LoginController.LC.CurrentUser.UserPlanetsBuildings.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.Tersane);

            if (!isTersaneExists)
            {
                // Uyarıyı basıyoruz.
                TXT_Alert.text = base.GetLanguageText("TersaneYok");

                // Üretim yapılamaz.
                return false;
            }
            else
            {
                // Uyarıyı siliyoruz.
                TXT_Alert.text = string.Empty;

                // Başarılı sonucunu dönüyoruz. Yani yükseltilebilir.
                return true;
            }
        }
    }

}
