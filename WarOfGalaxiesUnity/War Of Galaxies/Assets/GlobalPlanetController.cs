using Assets.Scripts.ApiModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalPlanetController : MonoBehaviour
{
    public static GlobalPlanetController GPC { get; set; }
    private void Awake()
    {
        if (GPC == null)
            GPC = this;
        else
            Destroy(gameObject);
    }

    [Header("Kullanıcının sahip olduğu gezegenler.")]
    public List<UserPlanetDTO> UserPlanets;

    [Header("Seçili olan gezegen.")]
    public UserPlanetDTO CurrentPlanet;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ApiService.API.Post("GetUserPlanets", null, (ApiResult response) =>
          {

              if (response.IsSuccess)
              {
                  // Kullanıcının gezegenlerini alıyoruz.
                  UserPlanets = response.GetDataList<UserPlanetDTO>();

                  // Default gezegeni seçiyoruz.
                  CurrentPlanet = UserPlanets.FirstOrDefault();

                  // Yüklemeyi ilerlet.
                  LoadingController.LC.IncreaseLoadCount();
              }

          }));
    }
}
