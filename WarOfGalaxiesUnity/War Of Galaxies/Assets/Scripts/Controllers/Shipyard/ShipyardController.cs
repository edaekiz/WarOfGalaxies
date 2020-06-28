using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipyardController : MonoBehaviour
{
    public static ShipyardController SC { get; set; }

    [Header("Üretimi yapılan gemi olduğunda bu obje aktif edilecek.")]
    public GameObject ShipyardProgressIcon;

    [Header("Gemiler ve ikonları.")]
    public List<ShipImageDTO> ShipWithImages;

    private void Awake()
    {
        if (SC == null)
            SC = this;
        else
            Destroy(gameObject);
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => LoadingController.LC.IsGameLoaded);
        StartCoroutine(ReCalculateShips());
    }

    public void ShowShipyardPanel()
    {
        // Paneli açıyoruz.
        GameObject panel = GlobalPanelController.GPC.ShowPanel(PanelTypes.ShipyardPanel);

        // Paneldeki gemileri yüklüyoruz.
        panel.GetComponent<ShipyardPanelController>().LoadAllShips();
    }

    public IEnumerator ReCalculateShips()
    {
        DateTime currentDate = DateTime.UtcNow;

        // Her bir gezegenin üretimi kontrol ediyoruz.
        foreach (UserPlanetDTO userPlanet in LoginController.LC.CurrentUser.UserPlanets)
        {
            // Eğer üretim var ise ilk üretim devam eden üretimdir.
            UserPlanetShipProgDTO firstShipProg = LoginController.LC.CurrentUser.UserPlanetShipProgs.FirstOrDefault(x => x.UserPlanetId == userPlanet.UserPlanetId);

            // Eğer yok ise geri dön.
            if (firstShipProg == null)
                continue;

            // Tersanesini buluyoruz.
            UserPlanetBuildingDTO shipyard = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == userPlanet.UserPlanetId && x.BuildingId == Buildings.Tersane);

            // Bir geminin üretimi için gereken süre.
            double countdownOneItem = DataController.DC.CalculateShipCountdown(firstShipProg.ShipId, shipyard == null ? 0 : shipyard.BuildingLevel);

            // Birim başına baktıktan sonra tamamlanmasına kalan süreye bakıyoruz.
            DateTime completeTime = firstShipProg.LastVerifyDate.Value.AddSeconds(-firstShipProg.OffsetTime).AddSeconds(countdownOneItem);

            // Tamamlanmasına kalan süre.
            TimeSpan leftTime = completeTime - currentDate;

            // Eğer üretim süresi bittiyse.
            if (leftTime.TotalSeconds <= 0)
            {
                // Ve yeni üretimlere başlıyoruz.
                firstShipProg.LastVerifyDate = currentDate;

                // Yarım üretimi 0lıyoruz.
                firstShipProg.OffsetTime = 0;

                // Üretilecek gemi miktarını 1 azaltıyoruz
                firstShipProg.ShipCount--;

                // Aktif gemi miktarı.
                UserPlanetShipDTO currentShipCount = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == userPlanet.UserPlanetId && x.ShipId == firstShipProg.ShipId);

                // Daha önce bu gemiye sahip miydi?
                if (currentShipCount == null)
                {
                    // Eğer ilk defa ekleniyor ise yeni oluşturuyoruz.
                    currentShipCount = new UserPlanetShipDTO() { ShipCount = 1, ShipId = firstShipProg.ShipId, UserPlanetId = firstShipProg.UserPlanetId };

                    // Listeye ekliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetShips.Add(currentShipCount);
                }
                else // AKsi durumda sadece miktarı arttırıyoruz.
                    currentShipCount.ShipCount++;

                // Eğer gemi kalmamış ise siliyoruz.
                if (firstShipProg.ShipCount <= 0)
                {
                    // Eğer daha yok ise listeden siliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetShipProgs.Remove(firstShipProg);

                    // Sonrakinin başlangıç tarihini güncelliyoruz.
                    UserPlanetShipProgDTO nextProg = LoginController.LC.CurrentUser.UserPlanetShipProgs.FirstOrDefault(x => x.UserPlanetId == userPlanet.UserPlanetId);

                    // Sonraki üretim.
                    if (nextProg != null)
                        nextProg.LastVerifyDate = currentDate;
                }

                // Paneli yeniliyoruz.
                if (ShipyardPanelController.SPC != null)
                    ShipyardPanelController.SPC.LoadAllShips();

                // Kuyruğu yeniliyoruz.
                if (ShipyardQueueController.SQC != null)
                    ShipyardQueueController.SQC.RefreshShipyardQueue();

            }
        }

        // Her saniye sonunda gerekiyorsa progress ikonunu açacağız ya da kapatacağız.
        RefreshProgressIcon();

        yield return new WaitForSecondsRealtime(1);

        StartCoroutine(ReCalculateShips());
    }

    public void RefreshProgressIcon()
    {
        if (LoginController.LC.CurrentUser.UserPlanetShipProgs.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId))
        {
            if (!ShipyardProgressIcon.activeSelf)
                ShipyardProgressIcon.SetActive(true);
        }
        else
        {
            if (ShipyardProgressIcon.activeSelf)
                ShipyardProgressIcon.SetActive(false);
        }
    }

    public void DestroyShip(Ships shipId, int quantity)
    {
        // Gezegendeki gemiyi buluyoruz.
        UserPlanetShipDTO ship = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == shipId);

        // Eğer yok ise geri dön.
        if (ship == null)
            return;

        // Var ise miktarı düş.
        ship.ShipCount -= quantity;

        // Eğer miktar 0 dan küçük olduysa listeden siliyoruz.
        if (ship.ShipCount <= 0)
            LoginController.LC.CurrentUser.UserPlanetShips.Remove(ship);

        // Eğer panel açık ise refresh ediyoruz.
        if (ShipyardPanelController.SPC != null)
            ShipyardPanelController.SPC.LoadAllShips();

    }

    public void AddShip(int userPlanetId, Ships shipId, int quantity)
    {
        // Gezegendeki gemiyi buluyoruz.
        UserPlanetShipDTO ship = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == userPlanetId && x.ShipId == shipId);

        // Eğer yok ise geri dön.
        if (ship == null)
        {
            LoginController.LC.CurrentUser.UserPlanetShips.Add(new UserPlanetShipDTO
            {
                UserPlanetId = userPlanetId,
                ShipId = shipId,
                ShipCount = quantity
            });
        }
        else
            ship.ShipCount += quantity;

    }
}
