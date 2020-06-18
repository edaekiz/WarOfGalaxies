using Assets.Scripts.ApiModels;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetPickerController : BasePanelController
{
    public static PlanetPickerController PPC { get; set; }

    private void Awake()
    {
        if (PPC == null)
            PPC = this;
        else
            Destroy(gameObject);
    }

    [Header("Kullanıcının her bir gezegeni için üretilecek.")]
    public GameObject PlanetPickerItem;

    [Header("Gezegenler buraya koyulacak.")]
    public RectTransform PlanetPickerContent;

    protected override void Start()
    {
        base.Start();
    }

    public void ReLoadPlanets()
    {
        // Eskilerini siliyoruz.
        foreach (Transform oldPlanet in PlanetPickerContent)
            Destroy(oldPlanet.gameObject);

        // Yenilerini basıyoruz.
        LoginController.LC.CurrentUser.UserPlanets.ForEach(e =>
        {
            GameObject planet = Instantiate(PlanetPickerItem, PlanetPickerContent);

            // Kordinat bilgisini alıyoruz.
            UserPlanetCordinatesDTO cordinateInfo = LoginController.LC.CurrentUser.UserPlanetCordinates.Find(x => x.UserPlanetId == e.UserPlanetId);

            // Kordinata çeviriyoruz.
            CordinateDTO cordinate = new CordinateDTO(cordinateInfo.GalaxyIndex, cordinateInfo.SolarIndex, cordinateInfo.OrderIndex);

            // Gezegen ismini hazırlıyoruz.
            string planetName = $"{e.PlanetName}{Environment.NewLine}<size=24><color=orange>({CordinateExtends.ToCordinateString(cordinate)}) </color></size>";

            // Gezegen ismini basıyoruz.
            planet.transform.Find("PlanetName").GetComponent<TMP_Text>().text = planetName;

            // Eğer seçili olan gezegen ise outline açılıyor
            if (e.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId)
                planet.transform.Find("SelectedIcon").gameObject.SetActive(true);
            else // Gezegen ikonuna tıkladığımız da gezegeni yükleyeceğiz.
                planet.GetComponent<Button>().onClick.AddListener(() => LoadSelectedPlanet(e.UserPlanetId));
        });
    }


    public void LoadSelectedPlanet(int userPlanetId)
    {
        // Tıklanılan gezegeni seçiyoruz.
        GlobalPlanetController.GPC.SelectPlanet(LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == userPlanetId));

        // Gezegendeki binaları tekrar yüklüyoruz.
        foreach (BuildingController bc in GlobalBuildingController.GBC.BuildingsInGame)
            bc.LoadBuildingDetails();

        // Seçilen gezegeni ekrana basıyoruz.
        ToastController.TC.ShowToast(LanguageController.LC.GetText("GezegeniSeçildi", GlobalPlanetController.GPC.CurrentPlanet.PlanetName));

        base.ClosePanel();
    }

}
