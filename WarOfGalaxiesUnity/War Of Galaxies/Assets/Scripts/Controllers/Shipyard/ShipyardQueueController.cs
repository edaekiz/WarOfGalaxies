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

public class ShipyardQueueController : BaseLanguageBehaviour
{
    public static ShipyardQueueController SQC { get; set; }

    [Header("Kuyruk oluştuğunda eklenecek olan eşya.")]
    public GameObject ShipyardQueueItem;

    [Header("Tersane kuyruğunu buraya yükleyeceğiz.")]
    public Transform ContentField;

    [Header("Üretim olduğunda geri sayımı buraya basacağız.")]
    public TMP_Text TXT_Countdown;

    private void Awake()
    {
        if (SQC == null)
            SQC = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InvokeRepeating("RefreshCountdown", 0, 1);
    }

    public void RefreshShipyardQueue()
    {
        // Eskilerini siliyoruz.
        foreach (Transform child in ContentField)
            Destroy(child.gameObject);

        // Şimdi yenilerini basıyoruz.
        foreach (UserPlanetShipProgDTO queue in LoginController.LC.CurrentUser.UserPlanetShipProgs.Where(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId))
        {
            // Kuyruk eşyası.
            GameObject go = Instantiate(ShipyardQueueItem, ContentField);

            // Resmi buluyoruz.
            ShipImageDTO image = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == queue.ShipId);

            // Resmi yüklüyoruz.
            if (image != null)
                go.transform.Find("ItemImage").GetComponent<Image>().sprite = image.ShipImage;

            // Miktarı basıyoruz.
            go.transform.Find("ItemCount").GetComponent<TMP_Text>().text = queue.ShipCount.ToString();
        }
    }

    public void RefreshCountdown()
    {
        // Üretim var mı?
        UserPlanetShipProgDTO prog = LoginController.LC.CurrentUser.UserPlanetShipProgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId);

        // Üretimi yok ise metni boş basıyoruz.
        if (prog == null)
        {
            // Metni boş basıyoruz üretim yok ise.
            TXT_Countdown.text = $"-";
        }
        else
        {
            // Tersanesini buluyoruz.
            UserPlanetBuildingDTO shipyard = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.Tersane);

            // Bir geminin üretimi için gereken süre.
            double countdownOneItem = DataController.DC.CalculateShipCountdown(prog.ShipId, shipyard == null ? 0 : shipyard.BuildingLevel);

            // Birim başına baktıktan sonra tamamlanmasına kalan süreye bakıyoruz.
            DateTime completeTime = prog.LastVerifyDate.Value.AddSeconds(-prog.OffsetTime).AddSeconds(countdownOneItem);

            // Tamamlanmasına kalan süre.
            TimeSpan leftTime = completeTime - DateTime.UtcNow;

            // Üretim geri sayımını aktif ediyoruz.
            TXT_Countdown.text = $"{TimeExtends.GetCountdownText(leftTime)}";
        }
    }
}
