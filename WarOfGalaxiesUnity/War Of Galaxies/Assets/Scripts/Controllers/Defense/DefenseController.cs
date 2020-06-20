using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefenseController : MonoBehaviour
{
    public static DefenseController DC { get; set; }

    [Header("Üretimi yapılan savunma olduğunda bu obje aktif edilecek.")]
    public GameObject DefenseProgressIcon;

    [Header("Defans birimin ikonu ile türü.")]
    public List<DefenseImageDTO> DefenseWithImages;

    private void Awake()
    {
        if (DC == null)
            DC = this;
        else
            Destroy(gameObject);
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => LoadingController.LC.IsGameLoaded);

        // Panelin kapalı olduğu sürede gerçekleşen üretimi hesaplıyoruz.
        StartCoroutine(ReCalculateDefenses());

    }

    public void ShowDefensePanel()
    {
        // Paneli buluyoruz.
        GameObject panel = GlobalPanelController.GPC.ShowPanel(PanelTypes.DefensePanel);

        // Paneli açıyoruz.
        panel.GetComponent<DefensePanelController>().LoadAllDefenses();
    }

    /// <summary>
    /// Kapalıyken sayma işlemi gerçekleşmez. Bu yüzden kapalı olduğu süredeki üretimi bitirmek gerekiyor.
    /// </summary>
    public IEnumerator ReCalculateDefenses()
    {
        // Şu an.
        DateTime currentDate = DateTime.UtcNow;

        foreach (var userPlanet in LoginController.LC.CurrentUser.UserPlanets)
        {
            // Eğer üretim var ise ilk üretim devam eden üretimdir.
            UserPlanetDefenseProgDTO firstDefenseProg = LoginController.LC.CurrentUser.UserPlanetDefenseProgs.FirstOrDefault(x => x.UserPlanetId == userPlanet.UserPlanetId);

            // Eğer yok ise geri dön.
            if (firstDefenseProg == null)
                continue;

            // Robot fabrikasını buluyoruz.
            UserPlanetBuildingDTO robotFac = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == userPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

            // Bir geminin üretimi için gereken süre.
            double countdownOneItem = DataController.DC.CalculateDefenseCountdown(firstDefenseProg.DefenseId, robotFac == null ? 0 : robotFac.BuildingLevel);

            // Birim başına baktıktan sonra tamamlanmasına kalan süreye bakıyoruz.
            DateTime completeTime = firstDefenseProg.LastVerifyDate.Value.AddSeconds(-firstDefenseProg.OffsetTime).AddSeconds(countdownOneItem);

            // Tamamlanmasına kalan süre.
            TimeSpan leftTime = completeTime - currentDate;

            // Eğer üretim süresi bittiyse.
            if (leftTime.TotalSeconds <= 0)
            {
                // Ve yeni üretimlere başlıyoruz.
                firstDefenseProg.LastVerifyDate = currentDate;

                // Yarım üretimi 0lıyoruz.
                firstDefenseProg.OffsetTime = 0;

                // Üretilecek gemi miktarını 1 azaltıyoruz
                firstDefenseProg.DefenseCount--;

                // Aktif gemi miktarı.
                UserPlanetDefenseDTO currentDefenseCount = LoginController.LC.CurrentUser.UserPlanetDefenses.Find(x => x.UserPlanetId == userPlanet.UserPlanetId && x.DefenseId == firstDefenseProg.DefenseId);

                // Daha önce bu gemiye sahip miydi?
                if (currentDefenseCount == null)
                {
                    // Eğer ilk defa ekleniyor ise yeni oluşturuyoruz.
                    currentDefenseCount = new UserPlanetDefenseDTO() { DefenseCount = 1, DefenseId = firstDefenseProg.DefenseId, UserPlanetId = firstDefenseProg.UserPlanetId };

                    // Listeye ekliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetDefenses.Add(currentDefenseCount);
                }
                else // AKsi durumda sadece miktarı arttırıyoruz.
                    currentDefenseCount.DefenseCount++;

                // Eğer gemi kalmamış ise siliyoruz.
                if (firstDefenseProg.DefenseCount <= 0)
                {
                    // Eğer daha yok ise listeden siliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Remove(firstDefenseProg);

                    // Sonrakinin başlangıç tarihini güncelliyoruz.
                    UserPlanetDefenseProgDTO nextProg = LoginController.LC.CurrentUser.UserPlanetDefenseProgs.FirstOrDefault(x => x.UserPlanetId == userPlanet.UserPlanetId);

                    // Sonraki üretim.
                    if (nextProg != null)
                        nextProg.LastVerifyDate = currentDate;
                }

                // Kuyruğu yeniliyoruz.
                if (DefenseQueueController.DQC != null)
                    DefenseQueueController.DQC.RefreshDefenseQueue();
            }
        }

        // Her saniye sonunda gerekiyorsa progress ikonunu açacağız ya da kapatacağız.
        RefreshProgressIcon();

        yield return new WaitForSecondsRealtime(1);

        StartCoroutine(ReCalculateDefenses());
    }


    public void RefreshProgressIcon()
    {
        if (LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId))
        {
            if (!DefenseProgressIcon.activeSelf)
                DefenseProgressIcon.SetActive(true);
        }
        else
        {
            if (DefenseProgressIcon.activeSelf)
                DefenseProgressIcon.SetActive(false);
        }
    }

}
