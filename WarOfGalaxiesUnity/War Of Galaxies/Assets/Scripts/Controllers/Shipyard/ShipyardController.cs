using Assets.Scripts.ApiModels;
using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipyardController : MonoBehaviour
{
    public static ShipyardController SC { get; set; }
    public List<ShipImageDTO> ShipWithImages;
    private void Awake()
    {
        if (SC == null)
            SC = this;
        else
            Destroy(gameObject);
    }


    public void ShowShipyardPanel()
    {
        // Kapalı olduğu sürede üretilen toplam miktarı hesaplıyoruz.
        ReCalculateShips();

        // Paneli açıyoruz.
        GameObject panel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.ShipyardPanel);

        // Paneldeki gemileri yüklüyoruz.
        panel.GetComponent<ShipyardPanelController>().LoadAllShips();
    }

    /// <summary>
    /// Kapalıyken sayma işlemi gerçekleşmez. Bu yüzden kapalı olduğu süredeki üretimi bitirmek gerekiyor.
    /// </summary>
    public void ReCalculateShips()
    {
        // Şu an.
        DateTime currentDate = DateTime.UtcNow;

        // Tersaneyi buluyoruz.
        UserPlanetBuildingDTO shipyard = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.Tersane);

        // Tersane seviyesi.
        int shipyardLevel = shipyard == null ? 0 : shipyard.BuildingLevel;

        // Son doğrulama tarihi.
        DateTime? lastVerifyDateInShipyard = null;

        // Bütün gemi üretimlerini dönüyoruz.
        foreach (UserPlanetShipProgDTO userPlanetShipProg in LoginController.LC.CurrentUser.UserPlanetShipProgs)
        {
            // Bir geminin üretim süresi.
            double shipBuildTime = StaticData.CalculateShipCountdown(userPlanetShipProg.ShipId, shipyardLevel);

            // Son onaylanma tarihi bir öncekinin bitiş tarihi.
            if (!userPlanetShipProg.LastVerifyDate.HasValue)
                userPlanetShipProg.LastVerifyDate = lastVerifyDateInShipyard;

            // Son doğrulamadan bu yana geçen süre.
            double passedSeconds = (currentDate - userPlanetShipProg.LastVerifyDate.Value).TotalSeconds;

            // Toplam üretilen gemi sayısı.
            int producedCount = (int)(passedSeconds / shipBuildTime);

            // Eğer üretim yok ise güncel üretimdeyiz.
            if (producedCount == 0)
                break;

            // Eğer olandan fazla ürettiysek üretebileceğimiz sınıra getiriyoruz.
            if (producedCount > userPlanetShipProg.ShipCount)
                producedCount = userPlanetShipProg.ShipCount;

            // Üretilmesi için geçen süreyi buluyoruz.
            passedSeconds = shipBuildTime * producedCount;

            // Son doğrulama tarihini güncelliyoruz.
            userPlanetShipProg.LastVerifyDate = userPlanetShipProg.LastVerifyDate.Value.AddSeconds(passedSeconds);

            // Son doğrulama süresini veriyoruz bunun üzerinden hesaplayacağız.
            lastVerifyDateInShipyard = userPlanetShipProg.LastVerifyDate;

            // Gezegende bulunan benzer gemi.
            UserPlanetShipDTO userPlanetShip = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == userPlanetShipProg.ShipId);

            // Eğer gezegende bu gemiden yok ise ekliyoruz.
            if (userPlanetShip == null)
            {
                // Veritabanına gemiyi ekliyoruz.
                LoginController.LC.CurrentUser.UserPlanetShips.Add(new UserPlanetShipDTO
                {
                    ShipId = userPlanetShipProg.ShipId,
                    ShipCount = producedCount,
                    UserPlanetId = GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId,
                });
            }
            else
            {
                // Sadece miktarı güncelliyoruz.
                userPlanetShip.ShipCount += producedCount;
            }

            // Üretim miktarını azaltıyoruz.
            userPlanetShipProg.ShipCount -= producedCount;

            // Eğer üretilebilecek gemi kalmamış ise veritabanından siliyoruz.
            if (userPlanetShipProg.ShipCount > 0)
                break;
        }

        // Bitenleri siliyoruz.
        LoginController.LC.CurrentUser.UserPlanetShipProgs.RemoveAll(x => x.ShipCount <= 0);
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

    public void AddShip(int userPlanetId,Ships shipId,int quantity)
    {
        // Gezegendeki gemiyi buluyoruz.
        UserPlanetShipDTO ship = LoginController.LC.CurrentUser.UserPlanetShips.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.ShipId == shipId);

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
