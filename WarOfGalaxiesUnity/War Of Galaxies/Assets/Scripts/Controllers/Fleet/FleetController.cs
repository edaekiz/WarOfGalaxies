using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FleetController : MonoBehaviour
{
    public static FleetController FC { get; set; }

    [Header("Eğer düşmanca bir saldırı var ise bu uyarı açılacak.")]
    public GameObject EnemyFleetAlert;

    [Header("Eğer düşmanca bir saldırı yok ise ancak filo hareketi var ise bu uyarı açılacak.")]
    public GameObject AnyFleetAlert;


    [Header("Filoları tutar.")]
    public List<FleetDTO> Fleets = new List<FleetDTO>();

    private void Awake()
    {
        if (FC == null)
            FC = this;
        else
            Destroy(gameObject);
    }

    IEnumerator Start()
    {
        // Giriş yaptığımızdan emin oluyoruz.
        yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

        // 15 saniyede bir filoları kontrol ediyoruz.
        InvokeRepeating("GetLatestFleets", 0, 15);

        // Biten filoları onaylıyoruz.
        StartCoroutine(CheckDoneFleets());

    }

    public void GetLatestFleets()
    {
        StartCoroutine(ApiService.API.Post("GetLastFleets", new GetLastFleetsDTO { LastFleetId = Fleets.Select(x => x.FleetId).DefaultIfEmpty(0).Max() }, (ApiResult response) =>
         {

             // Yanıt başarılı ise.
             if (response.IsSuccess)
             {
                 // Yeni gelen filo bilgilerini alıyoruz.
                 List<FleetDTO> newFleets = response.GetDataList<FleetDTO>();

                 // Eğer yeni filo var ise listeye ekliyoruz.
                 if (newFleets.Count > 0)
                 {
                     // Listeye dahil ediyoruz.
                     Fleets.AddRange(newFleets.Where(x => !Fleets.Any(y => y.FleetId == x.FleetId)));

                     // Eğer panel açık ise paneli de yeniliyoruz.
                     if (FleetPanelController.FPC != null)
                         FleetPanelController.FPC.RefreshActiveFleets();

                     // Bildirimleri yeniliyoruz.
                     RefreshProgressIcon();
                 }
             }
         }));
    }

    public void ShowFleetPanel()
    {
        GlobalPanelController.GPC.ShowPanel(PanelTypes.FleetPanel);
    }

    /// <summary>
    /// Her bir filoyu kontrol ediyoruz biten var mı?
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckDoneFleets()
    {
        DateTime currentDate = DateTime.Now;

        // Filoları kontrol ediyoruz.
        for (int ii = 0; ii < Fleets.Count; ii++)
        {
            FleetDTO fleet = Fleets[ii];

            // Filo hareketinin tamamlanmasına kalan süre.
            DateTime fleetEndFlyDate = fleet.FleetLoadDate.AddSeconds(fleet.EndLeftTime);

            // Eğer kaynak gezegene dönüş yapıysak.
            if (currentDate >= fleetEndFlyDate)
            {
                // Listeden siliyoruz.
                Fleets.Remove(fleet);

                // Bildirim ışığını yeniliyoruz.
                RefreshProgressIcon();

                // Filoyu verify ediyoruz sunucuda.
                VerifyFleet(fleet);
            }
        }

        yield return new WaitForSecondsRealtime(1);

        StartCoroutine(CheckDoneFleets());
    }

    public void RefreshProgressIcon()
    {
        // Uyarıları başlangıç da kapatıyoruz.
        EnemyFleetAlert.SetActive(false);
        AnyFleetAlert.SetActive(false);

        // Kullanıcının sahip olduğu gezegenlerin idlerini alıyoruz.
        int[] userPlanetIds = LoginController.LC.CurrentUser.UserPlanets.Select(x => x.UserPlanetId).ToArray();

        // Ve Kontrol ediyoruz kullanıcıya bir saldırı var mı?
        bool isEnemyActionExists = Fleets.Exists(x => userPlanetIds.Contains(x.DestinationUserPlanetId) && x.FleetActionTypeId == FleetTypes.Saldır && !x.IsReturnFleet);

        // Eğer düşmanca bir saldırı var ise uyarıyı açıyoruz.
        if (isEnemyActionExists)
            EnemyFleetAlert.SetActive(true);
        else if (Fleets.Count > 0)
            AnyFleetAlert.SetActive(true);
    }

    public void VerifyFleet(FleetDTO fleet)
    {
        // Dönüş gezegeninin kaynaklarını doğruluyoruz.
        if (fleet.DestinationUserId == LoginController.LC.CurrentUser.UserData.UserId)
            LoginController.LC.VerifyUserResources(fleet.DestinationUserPlanetId, (UserPlanetDTO userPlanet) =>
             {
                 // Ve eğer galaksiye bakıyorsak açık olan galaksiyi yenilememiz lazım.
                 if (GlobalGalaxyController.GGC.IsInGalaxyView && fleet.FleetActionTypeId == FleetTypes.Sök)
                 {
                     // Sök panelini yenilememiz lazım. Kapatıyoruz. İsterse yeniden açabilir.
                     GlobalPanelController.GPC.ClosePanel(PanelTypes.PlanetActionFooterPanel);

                     // Paneli yeniliyoruz.
                     GalaxyChangePanelController.GCPC.GoToCordinate();
                 }

                 // Hedefe gidiyor ise.
                 if (!fleet.IsReturnFleet)
                 {
                     // Dönüş filosundaki dataları güncelliyoruz.
                     RefreshReturnFleetData(fleet.ReturnFleetId);
                 }
                 else // Dönüş filosunun işlemi.
                 {
                     // Filodaki gemiler.
                     List<Tuple<Ships, int>> ships = FleetExtends.FleetDataToShipData(fleet.FleetData);

                     // Her bir gemiyi dönüyoruz ve envantere ekliyoruz..
                     ships.ForEach(e => ShipyardController.SC.AddShip(fleet.DestinationUserPlanetId, e.Item1, e.Item2));

                     // Paneli yeniliyoruz.
                     if (FleetPanelController.FPC != null)
                         FleetPanelController.FPC.RefreshActiveFleets();
                 }
             });
    }

    public void RefreshReturnFleetData(int returnFleetId)
    {
        StartCoroutine(ApiService.API.Post("GetFleetById", new GetLastFleetsDTO { LastFleetId = returnFleetId }, (ApiResult response) =>
        {
            // Yanıt başarılı ise.
            if (response.IsSuccess)
            {
                // Öncekini siliyoruz.
                Fleets.RemoveAll(x => x.FleetId == returnFleetId);

                // Yeni gelen filo bilgilerini alıyoruz.
                FleetDTO newFleet = response.GetData<FleetDTO>();

                // Listeye ekliyoruz.
                if (newFleet != null) // Eğer filo yok olmuş ise (Savaş esnasında) ozaman eklemeyeceğiz. Çünkü filo yok. 
                    Fleets.Add(newFleet);

                // Eğer panel açık ise paneli de yeniliyoruz.
                if (FleetPanelController.FPC != null)
                    FleetPanelController.FPC.RefreshActiveFleets();
            }
        }));
    }

}
