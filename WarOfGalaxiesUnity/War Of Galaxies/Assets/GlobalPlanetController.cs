using Assets.Scripts.ApiModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlanetController : MonoBehaviour
{
    public GlobalPlanetController GPC { get; set; }
    private void Awake()
    {
        if (GPC == null)
            GPC = this;
        else
            Destroy(gameObject);
    }

    public List<UserPlanetDTO> UserPlanets;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ApiService.API.Post("GetUserPlanets", null, (ApiResult response) =>
          {

              if (response.IsSuccess)
              {
                  // Kullanıcının gezegenlerini alıyoruz.
                  UserPlanets = response.GetDataList<UserPlanetDTO>();

                  // Yüklemeyi ilerlet.
                  LoadingController.LC.IncreaseLoadCount();
              }

          }));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
