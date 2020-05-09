using Assets.Scripts.ApiModels;
using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseItemController : MonoBehaviour
{
    [Header("Aktif tuttuğu savunma.")]
    public Defenses CurrentDefense;

    [Header("Savunma ismini basıyoruz.")]
    public TextMeshProUGUI DefenseName;

    [Header("Kullanıcının sahip olduğu miktar.")]
    public TextMeshProUGUI DefenseCount;

    [Header("Savunma resmi.")]
    public Image DefenseImage;

    [Header("Geri sayım resmi.")]
    public Image CountdownImage;

    [Header("Geri sayım süresi.")]
    public TextMeshProUGUI CountdownText;

    public IEnumerator LoadDefenseDetails(Defenses defense)
    {
        // Savunma bilgisi.
        CurrentDefense = defense;

        // Aktif savunma miktarı.
        UserPlanetDefenseDTO currentDefenseCount = LoginController.LC.CurrentUser.UserPlanetDefenses.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.DefenseId == defense);

        // Resmi yüklüyoruz.
        DefenseImage.sprite = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == defense).DefenseImage;

        // Üretim var mı?
        UserPlanetDefenseProgDTO prog = LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.DefenseId == defense);

        // Eğer üretim var ise süreyi basıyoruz.
        if (prog != null)
        {
            // İkonu ve kalan süreyi basıyoruz.
            CountdownImage.gameObject.SetActive(true);

            // Robot fabrikasını buluyoruz.
            UserPlanetBuildingDTO robotFac = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

            // Bir savunmanın üretimi için gereken süre.
            double countdownOneItem = StaticData.CalculateDefenseCountdown(defense, robotFac == null ? 0 : robotFac.BuildingLevel);

            // Birim başına baktıktan sonra tamamlanmasına kalan süreye bakıyoruz.
            DateTime completeTime = prog.LastVerifyDate.AddSeconds(-prog.OffsetTime).AddSeconds(countdownOneItem);

            // Tamamlanmasına kalan süre.
            TimeSpan leftTime = completeTime - DateTime.UtcNow;

            // Üretim geri sayımını aktif ediyoruz.
            CountdownText.text = $"({prog.DefenseCount}){Environment.NewLine}{TimeExtends.GetCountdownText(leftTime)}";

            // Eğer üretim süresi bittiyse.
            if (leftTime.TotalSeconds <= 0)
            {
                // Yarım üretimi 0lıyoruz.
                prog.OffsetTime = 0;

                // Ve yeni üretimlere başlıyoruz.
                prog.LastVerifyDate = DateTime.UtcNow;

                // Üretilecek gemi miktarını 1 azaltıyoruz
                prog.DefenseCount--;

                // Daha önce bu gemiye sahip miydi?
                if (currentDefenseCount == null)
                {
                    // Eğer ilk defa ekleniyor ise yeni oluşturuyoruz.
                    currentDefenseCount = new UserPlanetDefenseDTO() { DefenseCount= 1, DefenseId = prog.DefenseId, UserPlanetId = prog.UserPlanetId };

                    // Listeye ekliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetDefenses.Add(currentDefenseCount);
                }
                else // AKsi durumda sadece miktarı arttırıyoruz.
                    currentDefenseCount.DefenseCount++;

                // Eğer gemi kalmamış ise siliyoruz.
                if (prog.DefenseCount <= 0)
                {
                    // Eğer daha yok ise listeden siliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetDefenseProgs.Remove(prog);

                    // Sonrakinin başlangıç tarihini güncelliyoruz.
                    UserPlanetDefenseProgDTO nextProg = LoginController.LC.CurrentUser.UserPlanetDefenseProgs.FirstOrDefault();

                    // Sonraki üretim.
                    if (nextProg != null)
                        nextProg.LastVerifyDate = DateTime.UtcNow;
                }

                // Kuyruğu yeniliyoruz.
                DefenseQueueController.DQC.RefreshDefenseQueue();
            }

            // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
            DefenseCount.text = currentDefenseCount== null ? $"{0}" : $"{currentDefenseCount.DefenseCount}".ToString();
        }
        else
        {
            // İkonu kapatıyoruz.
            CountdownImage.gameObject.SetActive(false);

            // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
            DefenseCount.text = currentDefenseCount == null ? $"{0}" : $"{currentDefenseCount.DefenseCount}";
        }

        // 1 saniye bekleyip tekrar çağırıyoruz.
        yield return new WaitForSecondsRealtime(1);

        // Tekrar çağırıyoruz.
        StartCoroutine(LoadDefenseDetails(defense));
    }

    public void ShowDefenseDetail()
    {
        GameObject defenseDetailPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.DefenseDetailPanel);
        DefenseDetailItemPanel ddip = defenseDetailPanel.GetComponent<DefenseDetailItemPanel>();
        ddip.StartCoroutine(ddip.LoadDefenseDetails(CurrentDefense));
    }

}
