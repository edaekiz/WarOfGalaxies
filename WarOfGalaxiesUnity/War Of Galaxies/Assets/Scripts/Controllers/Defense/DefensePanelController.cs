using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefensePanelController : BasePanelController
{
    /// <summary>
    /// Açık olan defans paneli.
    /// </summary>
    public static DefensePanelController DPC { get; set; }

    [Header("Savunmaları burada tutacağız.")]
    public List<DefenseItemController> _defenseItems = new List<DefenseItemController>();

    [Header("Basılacak olan savunmalar.")]
    public GameObject DefenseItem;

    [Header("Savunmaları buraya basacağız.")]
    public Transform DefenseItemContent;

    [Header("Savunma üretemiyor ise burada hata vereceğiz.")]
    public TMP_Text TXT_Alert;

    private void Awake()
    {
        if (DPC == null)
            DPC = this;
        else
            Destroy(gameObject);
    }
    protected override void Start()
    {
        base.Start();

        // Kontrol ediyoruz üretim yapabilmek için şartlar uygun mu?
        StartCoroutine(CheckIsDefenseExists());
    }

    public void LoadAllDefenses()
    {
        // Bütün eski savunmaları sil.
        foreach (Transform child in DefenseItemContent)
            Destroy(child.gameObject);

        // Öncekileri temizliyoruz.
        _defenseItems.Clear();

        // Bütün savunmaları teker teker basıyoruz.
        for (int ii = 0; ii < DataController.DC.SystemData.Defenses.Count; ii++)
        {
            // Savunma bilgisi.
            DefenseDataDTO defense = DataController.DC.SystemData.Defenses[ii];

            // Savunmayı oluşturuyoruz.
            GameObject defenseItem = Instantiate(DefenseItem, DefenseItemContent);

            // Savunma controlleri.
            DefenseItemController dic = defenseItem.GetComponent<DefenseItemController>();

            // Detayları yükle.
            dic.StartCoroutine(dic.LoadDefenseDetails((Defenses)defense.DefenseId));

            // Listeye ekle
            _defenseItems.Add(dic);
        }

        // Hepsini kurduktan sonra kuyruğu yeniliyoruz.
        DefenseQueueController.DQC.RefreshDefenseQueue();

    }

    /// <summary>
    /// Kontrol ediyoruz tersanesi var mı?
    /// </summary>
    public IEnumerator CheckIsDefenseExists()
    {
        // Giriş yapana kadar bekliyoruz.
        yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

        // Kontrol ediyoruz robot fabrikası var mı bu gezegende?
        bool isResearchLabExists = LoginController.LC.CurrentUser.UserPlanetsBuildings.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

        // Eğer savunma üretemiyor isek bir hatadan dolayı, 1 saniye sonra tekrar kontrol ediyoruz.
        if (!CanProduceDefense())
        {
            // 1 saniye beklemeliyiz.
            yield return new WaitForSecondsRealtime(1);

            // Tekrar kendisini çağırıyoruz.
            StartCoroutine(CheckIsDefenseExists());
        }
    }

    public bool CanProduceDefense()
    {
        // Eğer tersane yükseltiliyor ise bu buton açılacak.
        bool isRobFacUpgrading = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

        // Robot fabrikası yükseltiliyor ise uyarı panelini açacağız.
        if (isRobFacUpgrading)
        {
            // Uyarıyı basıyoruz.
            TXT_Alert.text = base.GetLanguageText("RobotFabYükseltmeVar");

            // Üretim yapılamaz.
            return false;
        }
        else
        {
            // Robot fabrikası var mı diye kontrol ediyoruz.
            bool isRoboFacExists = LoginController.LC.CurrentUser.UserPlanetsBuildings.Exists(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.RobotFabrikası);

            if (!isRoboFacExists)
            {
                // Uyarıyı basıyoruz.
                TXT_Alert.text = base.GetLanguageText("RobotFabYok");

                // Üretim yapılamaz.
                return false;
            }
            else
            {
                // Uyarıyı siliyoruz.
                TXT_Alert.text = string.Empty;

                // Başarılı sonucunu dönüyoruz. Yani yükseltilebilir.
                return true;
            }
        }
    }


}
