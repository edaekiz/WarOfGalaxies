using Assets.Scripts.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public static LoginController LC { get; set; }

    [Header("Kullanıcı giri yapı mı?")]
    public bool IsLoggedIn;

    /// <summary>
    /// Giriş yapmış olan kullanıcı.
    /// </summary>
    public LoginStuffDTO CurrentUser { get; set; }

    [Header("Kullanıcı adının basılacağı yer.")]
    public TMP_Text UsernameField;

    private void Awake()
    {
        if (LC == null)
            LC = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ApiService.API.Post("Login", null, (ApiResult response) =>
        {
            // Eğer başarılı ise.
            if (response.IsSuccess)
            {
                // Giriş yapan kullanıcı.
                CurrentUser = response.GetData<LoginStuffDTO>();

                // Şuanki üretilen savunmayı bulup tarihine şuanı ekliyoruz.
                UserPlanetDefenseProgDTO currentDefenseProg = CurrentUser.UserPlanetDefenseProgs.FirstOrDefault();

                // Eğer var ise üretim ilk üretime tarihini veriyoruz. Ki kapalı olduğunda süreyi hesaplıyabilelim.
                if (currentDefenseProg != null)
                    currentDefenseProg.LastVerifyDate = DateTime.UtcNow;

                // Şuanki üretilen gemiyi bulup tarihine şuanı ekliyoruz.
                UserPlanetShipProgDTO currentShipProg = CurrentUser.UserPlanetShipProgs.FirstOrDefault();

                // Eğer var ise üretim ilk üretime tarihini veriyoruz. Ki kapalı olduğunda süreyi hesaplıyabilelim.
                if (currentShipProg != null)
                    currentShipProg.LastVerifyDate = DateTime.UtcNow;

                // Başlangıç ve bitiş tarihini ayarlıyoruz.
                CurrentUser.UserPlanetsBuildingsUpgs.ForEach(e => e.CalculateDates(e.LeftTime));

                // Araştırmanın başlangıç ve bitiş tarihlerini de ayarlıyoruz.
                CurrentUser.UserResearchProgs.ForEach(e => e.CalculateDates(e.LeftTime));

                // Yükleme sayısını 1 arttırıyoruz.
                LoadingController.LC.IncreaseLoadCount();

                // Evet kullanıcı giriş yaptı.
                IsLoggedIn = true;
            }
            else
            {
                Debug.LogWarning(response.Message);
            }
        }));
    }

    public void VerifyUserResources(int userPlanetId, Action<UserPlanetDTO> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(ApiService.API.Post("VerifyUserData", new VerifyResourceDTO { UserPlanetID = userPlanetId }, (ApiResult result) =>
           {
               if (result.IsSuccess)
               {
                   // Gelen gezegen kaynaklarını alıyoruz.
                   UserPlanetDTO data = result.GetData<UserPlanetDTO>();

                   // Eski gezegen bilgisini buluyoruz.
                   UserPlanetDTO existsPlanet = LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == data.UserPlanetId);

                   // Eğer sistemde var ise güncelliyoruz yok ise ekliyoruz.
                   if (existsPlanet != null)
                       data.CopyTo(existsPlanet);
                   else
                       LoginController.LC.CurrentUser.UserPlanets.Add(data);
                   
                   // Başarılı methotunu çağırıyoruz.
                   if (onSuccess != null)
                       onSuccess.Invoke(data);
               }
               else
               {
                   if (onError != null)
                       onError.Invoke(result.Message);
               }
           }));
    }
}
