using Assets.Scripts.ApiModels;
using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DefenseController : MonoBehaviour
{
    public static DefenseController DC { get; set; }

    [Header("Defans birimin ikonu ile türü.")]
    public List<DefenseImageDTO> DefenseWithImages;

    private void Awake()
    {
        if (DC == null)
            DC = this;
        else
            Destroy(gameObject);
    }

    
    public void ShowDefensePanel()
    {
        // Panelin kapalı olduğu sürede gerçekleşen üretimi hesaplıyoruz.
        ReCalculateDefenses();

        // Paneli buluyoruz.
        GameObject panel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.DefensePanel);

        // Paneli açıyoruz.
        panel.GetComponent<DefensePanelController>().LoadAllDefenses();
    }

    /// <summary>
    /// Kapalıyken sayma işlemi gerçekleşmez. Bu yüzden kapalı olduğu süredeki üretimi bitirmek gerekiyor.
    /// </summary>
    public void ReCalculateDefenses()
    {
        // Şu an.
        DateTime currentDate = DateTime.UtcNow;

        // Tersaneyi buluyoruz.
        UserPlanetBuildingDTO robotFac = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

        // Tersane seviyesi.
        int robotFacLevel = robotFac == null ? 0 : robotFac.BuildingLevel;

        // Son doğrulama tarihi.
        DateTime? lastVerifyDateInDefense = null;

        // Her bir üretimi dönüyoruz.
        foreach (UserPlanetDefenseProgDTO userPlanetDefenseProg in LoginController.LC.CurrentUser.UserPlanetDefenseProgs)
        {
            // Bir savunmanın üretim süresi.
            double defenseBuildTime = StaticData.CalculateDefenseCountdown(userPlanetDefenseProg.DefenseId, robotFacLevel);

            // Son onaylanma tarihi bir öncekinin bitiş tarihi.
            if (!userPlanetDefenseProg.LastVerifyDate.HasValue)
                userPlanetDefenseProg.LastVerifyDate = lastVerifyDateInDefense;

            // Son doğrulamadan bu yana geçen süre.
            double passedSeconds = (currentDate - userPlanetDefenseProg.LastVerifyDate.Value).TotalSeconds;

            // Toplam üretilen gemi sayısı.
            int producedCount = (int)(passedSeconds / defenseBuildTime);

            // Eğer üretim yok ise güncel üretimdeyiz.
            if (producedCount == 0)
                break;

            // Eğer olandan fazla ürettiysek üretebileceğimiz sınıra getiriyoruz.
            if (producedCount > userPlanetDefenseProg.DefenseCount)
                producedCount = userPlanetDefenseProg.DefenseCount;

            // Üretilmesi için geçen süreyi buluyoruz.
            passedSeconds = defenseBuildTime * producedCount;

            // Son doğrulama tarihini güncelliyoruz.
            userPlanetDefenseProg.LastVerifyDate = userPlanetDefenseProg.LastVerifyDate.Value.AddSeconds(passedSeconds);

            // Son doğrulama süresini veriyoruz bunun üzerinden hesaplayacağız.
            lastVerifyDateInDefense = userPlanetDefenseProg.LastVerifyDate;

            // Gezegende bulunan benzer savunmalar.
            UserPlanetDefenseDTO userPlanetDefense = LoginController.LC.CurrentUser.UserPlanetDefenses.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.DefenseId == userPlanetDefenseProg.DefenseId);

            // Eğer gezegende bu savunmadan yok ise ekliyoruz.
            if (userPlanetDefense == null)
            {
                // Veritabanına savunma ekliyoruz.
                LoginController.LC.CurrentUser.UserPlanetDefenses.Add(new UserPlanetDefenseDTO
                {
                    DefenseId = userPlanetDefenseProg.DefenseId,
                    DefenseCount = producedCount,
                    UserPlanetId = GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId,
                });
            }
            else
            {
                // Sadece miktarı güncelliyoruz.
                userPlanetDefense.DefenseCount += producedCount;
            }

            // Üretim miktarını azaltıyoruz.
            userPlanetDefenseProg.DefenseCount -= producedCount;

            // Eğer üretimin tamamı bitmeiş ise döngüyü bitir.
            if (userPlanetDefenseProg.DefenseCount > 0)
                break;
        }

        // Bitenleri siliyoruz.
        LoginController.LC.CurrentUser.UserPlanetDefenseProgs.RemoveAll(x => x.DefenseCount <= 0);
    }


}
