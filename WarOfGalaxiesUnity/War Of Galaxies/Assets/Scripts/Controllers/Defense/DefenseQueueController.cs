using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseQueueController : BaseLanguageBehaviour
{
    public static DefenseQueueController DQC { get; set; }

    private void Awake()
    {
        if (DQC == null)
            DQC = this;
        else
            Destroy(gameObject);
    }

    [Header("Kuyruk oluştuğunda eklenecek olan eşya.")]
    public GameObject DefenseQueueItem;

    [Header("Robot fabrikası kuyruğunu buraya yükleyeceğiz.")]
    public Transform ContentField;

    [Header("Üretim olduğunda geri sayımı buraya basacağız.")]
    public TMP_Text TXT_Countdown;
    private void Start()
    {
        InvokeRepeating("RefreshCountdown", 0, 1);
    }

    public void RefreshDefenseQueue()
    {
        // Eskilerini siliyoruz.
        foreach (Transform child in ContentField)
            Destroy(child.gameObject);

        // Şimdi yenilerini basıyoruz.
        foreach (UserPlanetDefenseProgDTO queue in LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Where(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId))
        {
            // Kuyruk eşyası.
            GameObject go = Instantiate(DefenseQueueItem, ContentField);

            // Savunma resmini alıyoruz.
            DefenseImageDTO defenseImage = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == queue.DefenseId);

            // Resmi yüklüyoruz.
            if (defenseImage != null)
                go.transform.Find("ItemImage").GetComponent<Image>().sprite = defenseImage.DefenseImage;

            // Miktarı basıyoruz.
            go.transform.Find("ItemCount").GetComponent<TMP_Text>().text = queue.DefenseCount.ToString();
        }
    }

    public void RefreshCountdown()
    {
        // Üretim var mı?
        UserPlanetDefenseProgDTO prog = LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Üretimi yok ise metni boş basıyoruz.
        if (prog == null)
        {
            // Metni boş basıyoruz üretim yok ise.
            TXT_Countdown.text = $"-";
        }
        else
        {
            // Tersanesini buluyoruz.
            UserPlanetBuildingDTO robotFac = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

            // Bir savunmanın üretimi için gereken süre.
            double countdownOneItem = DataController.DC.CalculateDefenseCountdown(prog.DefenseId, robotFac == null ? 0 : robotFac.BuildingLevel);

            // Birim başına baktıktan sonra tamamlanmasına kalan süreye bakıyoruz.
            DateTime completeTime = prog.LastVerifyDate.Value.AddSeconds(-prog.OffsetTime).AddSeconds(countdownOneItem);

            // Tamamlanmasına kalan süre.
            TimeSpan leftTime = completeTime - DateTime.UtcNow;

            // Üretim geri sayımını aktif ediyoruz.
            TXT_Countdown.text = $"{TimeExtends.GetCountdownText(leftTime)}";
        }
    }
}
