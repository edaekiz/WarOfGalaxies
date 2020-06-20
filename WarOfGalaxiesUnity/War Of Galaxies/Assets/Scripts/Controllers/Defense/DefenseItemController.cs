using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseItemController : BaseLanguageBehaviour
{
    [Header("Aktif tuttuğu savunma.")]
    public Defenses CurrentDefense;

    [Header("Savunma ismini basıyoruz.")]
    public TMP_Text DefenseName;

    [Header("Kullanıcının sahip olduğu miktar.")]
    public TMP_Text DefenseCount;

    [Header("Savunma resmi.")]
    public Image DefenseImage;

    [Header("Geri sayım resmi.")]
    public Image CountdownImage;

    [Header("Geri sayım süresi.")]
    public TMP_Text CountdownText;

    private void Start()
    {
        InvokeRepeating("RefreshState", 0, 1);
    }

    public void LoadDefenseDetails(Defenses defense)
    {
        // Savunma bilgisi.
        CurrentDefense = defense;

        // İsmini basıyoruz.
        DefenseName.text = base.GetLanguageText($"D{(int)defense}");

        // Resmi yüklüyoruz.
        DefenseImage.sprite = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == defense).DefenseImage;

    }

    public void RefreshState()
    {
        // Aktif savunma miktarı.
        UserPlanetDefenseDTO currentDefenseCount = LoginController.LC.CurrentUser.UserPlanetDefenses.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.DefenseId == CurrentDefense);

        // Üretim var mı?
        UserPlanetDefenseProgDTO prog = LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.DefenseId == CurrentDefense);

        // Eğer üretim var ise süreyi basıyoruz. Eğer gezegende şuan üretilen üretim bu ise ozaman göstereceğiz ilerlemeyi.
        if (prog != null && ReferenceEquals(prog, LoginController.LC.CurrentUser.UserPlanetDefenseProgs.FirstOrDefault(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId)))
        {
            // İkonu ve kalan süreyi basıyoruz.
            if (!CountdownImage.gameObject.activeSelf)
                CountdownImage.gameObject.SetActive(true);

            // Robot fabrikasını buluyoruz.
            UserPlanetBuildingDTO robotFac = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

            // Bir savunmanın üretimi için gereken süre.
            double countdownOneItem = DataController.DC.CalculateDefenseCountdown(CurrentDefense, robotFac == null ? 0 : robotFac.BuildingLevel);

            // Birim başına baktıktan sonra tamamlanmasına kalan süreye bakıyoruz.
            DateTime completeTime = prog.LastVerifyDate.Value.AddSeconds(-prog.OffsetTime).AddSeconds(countdownOneItem);

            // Tamamlanmasına kalan süre.
            TimeSpan leftTime = completeTime - DateTime.UtcNow;

            // Üretim geri sayımını aktif ediyoruz.
            CountdownText.text = $"({prog.DefenseCount}){Environment.NewLine}{TimeExtends.GetCountdownText(leftTime)}";

            // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
            DefenseCount.text = currentDefenseCount == null ? $"{0}" : $"{currentDefenseCount.DefenseCount}".ToString();
        }
        else
        {
            // İkonu kapatıyoruz.
            if (CountdownImage.gameObject.activeSelf)
                CountdownImage.gameObject.SetActive(false);

            // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
            DefenseCount.text = currentDefenseCount == null ? $"{0}" : $"{currentDefenseCount.DefenseCount}";
        }
    }

    public void ShowDefenseDetail()
    {
        GameObject defenseDetailPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.DefenseDetailPanel);
        DefenseDetailItemPanel ddip = defenseDetailPanel.GetComponent<DefenseDetailItemPanel>();
        ddip.LoadDefenseDetails(CurrentDefense);
    }

}
