using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalBuildingController : MonoBehaviour
{
    [Serializable]
    public struct BuildingsWithImage
    {
        public Buildings Building;
        public Sprite BuildingImage;
    }

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
            UserPlanetBuildingUpgDTO UserPlanetBuildingUpg = LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.FirstOrDefault(x => x.UserPlanetId == userPlanet.UserPlanetId);

            if (UserPlanetBuildingUpg == null)
                continue;

            // Eğer tarih tamamlanma tarihinnden az ise tamamlanmıştır.
            if (DateTime.UtcNow >= UserPlanetBuildingUpg.EndDate)
            {
                // Kullanıcının gezegendeki kaynaklarını verify ediyoruz.
                LoginController.LC.VerifyUserResources(GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId, (UserPlanetDTO onSuccess) =>
                {
                    // Var olan binayı buluyoruz.
                    UserPlanetBuildingDTO userBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == UserPlanetBuildingUpg.UserPlanetId && x.BuildingId == UserPlanetBuildingUpg.BuildingId);

                    // Bina ilk kez kuruluyor.
                    if (userBuilding == null)
                    {
                        // Bina listesine ekliyoruz.
                        LoginController.LC.CurrentUser.UserPlanetsBuildings.Add(new UserPlanetBuildingDTO
                        {
                            BuildingId = UserPlanetBuildingUpg.BuildingId,
                            BuildingLevel = UserPlanetBuildingUpg.BuildingLevel,
                            UserPlanetId = UserPlanetBuildingUpg.UserPlanetId
                        });
                    }
                    else // Eğer bina zaten var ise sadece kaynaklarını güncelliyoruz.
                    {
                        userBuilding.BuildingLevel = UserPlanetBuildingUpg.BuildingLevel;
                    }

                    // Yükseltmeyi sistemden siliyoruz.
                    LoginController.LC.CurrentUser.UserPlanetsBuildingsUpgs.Remove(UserPlanetBuildingUpg);

                });
            }
        }

        yield return new WaitForSeconds(1);
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
        foreach (BuildingController bc in FindObjectsOfType<BuildingController>())
            bc.SelectionMesh.SetActive(false);
    }
}
