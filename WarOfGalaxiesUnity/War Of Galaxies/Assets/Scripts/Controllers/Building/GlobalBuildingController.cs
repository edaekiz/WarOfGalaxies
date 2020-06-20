using Assets.Scripts.ApiModels;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalBuildingController : MonoBehaviour
{
    public static GlobalBuildingController GBC { get; set; }
    private void Awake()
    {
        if (GBC == null)
            GBC = this;
        else
            Destroy(GBC);
    }

    [Header("Binalar ve ikonları.")]
    public List<BuildingsWithImage> BuildingWithImages;

    [Header("Seçili olan bina.")]
    public BuildingController CurrentSelectedBuilding;

    [Header("Oyundaki binalar.")]
    public List<BuildingController> BuildingsInGame;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => LoadingController.LC.IsGameLoaded);
        StartCoroutine(ReCalculateBuildings());
    }

    public IEnumerator ReCalculateBuildings()
    {
        DateTime currentDate = DateTime.UtcNow;

        foreach (UserPlanetDTO userPlanet in LoginController.LC.CurrentUser.UserPlanets)
        {
            // Yükseltme bilgisi bulunamadı.
            UserPlanetBuildingUpgDTO userPlanetBuildingUpg = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.FirstOrDefault(x => x.UserPlanetId == userPlanet.UserPlanetId);

            // Yükseltme yok ise geri dön.
            if (userPlanetBuildingUpg == null)
                continue;

            // Eğer tarih tamamlanma tarihinnden az ise tamamlanmıştır.
            if (DateTime.UtcNow >= userPlanetBuildingUpg.EndDate)
            {
                // Kullanıcının gezegendeki kaynaklarını verify ediyoruz.
                LoginController.LC.VerifyUserResources(userPlanet.UserPlanetId, (UserPlanetDTO onSuccess) =>
                {
                    // Var olan binayı buluyoruz.
                    UserPlanetBuildingDTO userBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == userPlanetBuildingUpg.UserPlanetId && x.BuildingId == userPlanetBuildingUpg.BuildingId);

                    // Bina ilk kez kuruluyor.
                    if (userBuilding == null)
                    {
                        // Bina listesine ekliyoruz.
                        LoginController.LC.CurrentUser.UserPlanetsBuildings.Add(new UserPlanetBuildingDTO
                        {
                            BuildingId = userPlanetBuildingUpg.BuildingId,
                            BuildingLevel = userPlanetBuildingUpg.BuildingLevel,
                            UserPlanetId = userPlanetBuildingUpg.UserPlanetId
                        });
                    }
                    else // Eğer bina zaten var ise sadece kaynaklarını güncelliyoruz.
                    {
                        userBuilding.BuildingLevel = userPlanetBuildingUpg.BuildingLevel;
                    }

                    // Binayı buluyoruz.
                    BuildingController building = FindObjectsOfType<BuildingController>().FirstOrDefault(x => x.BuildingType == userPlanetBuildingUpg.BuildingId);

                    // Binayı bulduktan sonra yeniliyoruz detaylarını.
                    if (building != null)
                        building.LoadBuildingDetails();

                });

                // Yükseltmeyi sistemden siliyoruz.
                LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Remove(userPlanetBuildingUpg);
            }
        }

        // 1 saniye bekliyoruz.
        yield return new WaitForSeconds(1);

        // Sonra tekrar hesaplıyoruz.
        StartCoroutine(ReCalculateBuildings());
    }

    public void SelectBuilding(BuildingController _selectedBuilding)
    {
        // Tüm seçimleri kaldırıyoruz.
        DeSelectBuilding();

        // Binayı seçiyoruz.
        CurrentSelectedBuilding = _selectedBuilding;

        // Seçimini açıyoruz.
        CurrentSelectedBuilding.SelectionMesh.gameObject.SetActive(true);
    }

    public void DeSelectBuilding()
    {
        // Bütün seçimleri kaldırıyoruz.
        foreach (BuildingController bc in BuildingsInGame)
            bc.SelectionMesh.SetActive(false);
    }
}
