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
                     Fleets.AddRange(newFleets);

                     // Eğer panel açık ise paneli de yeniliyoruz.
                     if (FleetPanelController.FPC != null)
                         FleetPanelController.FPC.RefreshActiveFleets();
                 }
             }
         }));
    }

    public void ShowFleetPanel()
    {
        GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.FleetPanel);
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

            // Uçuşa başlanılan tarih.
            DateTime fleetBeginFlyDate = fleet.FleetLoadDate.AddSeconds(-fleet.BeginPassedTime);

            // Filo hareketinin tamamlanmasına kalan süre.
            DateTime fleetEndFlyDate = fleet.FleetLoadDate.AddSeconds(fleet.EndLeftTime);

            // Toplam yolculuk süresi saniye cinsinden.
            double totalPathSeconds = (fleetEndFlyDate - fleetBeginFlyDate).TotalSeconds;

            // Toplam yolculuk süresi.
            double haflOfPath = totalPathSeconds / 2;

            // Filonun hedefe ulaşma süresi.
            DateTime halfDateOfFly = fleetBeginFlyDate.AddSeconds(haflOfPath);

            // Filonun gezegene dönüş süresi.
            DateTime endDateOfFly = fleet.FleetLoadDate.AddSeconds(fleet.EndLeftTime);

            // Eğer gidişi bitirdikysek ve hala daha geri dönmüyor görünüyor isek true yapıyoruz değeri.
            if (!fleet.IsReturning && currentDate >= halfDateOfFly)
            {
                // Filoyu doğruluyoruz.
                VerifyFleet(fleet);

                // Geri dönecek filo.
                fleet.IsReturning = true;

            }

            // Eğer kaynak gezegene dönüş yapıysak.
            if (fleet.IsReturning && currentDate >= endDateOfFly)
            {
                // Filoyu verify ediyoruz sunucuda.
                VerifyFleet(fleet);

                // Eğer panel açık ise paneli refresh edeceğiz.
                if (FleetPanelController.FPC != null)
                {
                    // Paneldeki filo bilgisini buluyoruz.
                    FleetPanelItemController fleetItemInPanel = FleetPanelController.FPC.Fleets.Find(x => x.FleetInfo.FleetId == fleet.FleetId);

                    // Eğer var ise sileceğiz.
                    if (fleetItemInPanel != null)
                    {
                        // Önce listeden siliyoruz.
                        FleetPanelController.FPC.Fleets.Remove(fleetItemInPanel);

                        // Sonra panelden siliyoruz.
                        Destroy(fleetItemInPanel.gameObject);
                    }
                }

                // Filosunu geri veriyoruz.
                List<Tuple<Ships, int>> fleetShips = FleetExtends.FleetDataToShipData(fleet.FleetData);

                // Filoyu geri veriyoruz.
                fleetShips.ForEach(e => ShipyardController.SC.AddShip(fleet.SenderUserPlanetId, e.Item1, e.Item2));

                // PAnel açık ise bütün gemileri yeniden yüklüyoruz.
                if (ShipyardPanelController.SPC != null)
                    ShipyardPanelController.SPC.LoadAllShips();

                // Listeden siliyoruz.
                Fleets.Remove(fleet);
            }
        }

        yield return new WaitForSecondsRealtime(1);

        StartCoroutine(CheckDoneFleets());
    }

    public void VerifyFleet(FleetDTO fleet)
    {
        if (fleet.SenderUserId == LoginController.LC.CurrentUser.UserData.UserId)
        {
            LoginController.LC.VerifyUserResources(fleet.SenderUserPlanetId, (UserPlanetDTO userPlanet) =>
             {

                 StartCoroutine(ApiService.API.Post("GetFleetDetails", new GetLastFleetsDTO { LastFleetId = fleet.FleetId }, (ApiResult response) =>
                 {
                     // Eğer response false ise filo yok demektir geri dön.
                     if (!response.IsSuccess)
                         return;

                     // Son güncel filo bilgisi.
                     FleetDTO newFleetInfo = response.GetData<FleetDTO>();

                     // Eski olanı aktif filodan siliyoruz.
                     Fleets.Remove(fleet);

                     // Yeni olanı aktif filoya ekliyoruz.
                     Fleets.Add(newFleetInfo);

                     // Panel açık ise bilgiyi güncelliyoruz.
                     if (FleetPanelController.FPC != null)
                     {
                         // Paneldeki filoyu buluyoruz.
                         FleetPanelItemController panelItem = FleetPanelController.FPC.Fleets.Find(x => x.FleetInfo.FleetId == fleet.FleetId);

                         // Yok ise geri dön.
                         if (panelItem == null)
                             return;

                         // Filo bilgisini güncelliyoruz.
                         panelItem.FleetInfo = newFleetInfo;

                         // Paneli refresh ediyoruz.
                         FleetPanelController.FPC.RefreshActiveFleets();
                     }
                 }));
             });
        }

        if (fleet.DestinationUserId == LoginController.LC.CurrentUser.UserData.UserId)
            LoginController.LC.VerifyUserResources(fleet.DestinationUserPlanetId);
    }

}
