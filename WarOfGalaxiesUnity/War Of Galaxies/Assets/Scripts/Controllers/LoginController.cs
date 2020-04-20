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
    public TextMeshProUGUI UsernameField;

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

                // Kullanıcı adını basıyoruz.
                UsernameField.text = CurrentUser.UserData.Username;

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

    public void VerifyUserResources(int[] userPlanetIds, Action<List<UserPlanetDTO>> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(ApiService.API.Post("VerifyUserData", userPlanetIds, (ApiResult result) =>
          {
              if (result.IsSuccess)
              {
                  List<UserPlanetDTO> data = result.GetDataList<UserPlanetDTO>();

                  // Olan kaynakları da güncelliyoruz.
                  foreach (UserPlanetDTO exists in data.Where(x => LoginController.LC.CurrentUser.UserPlanets.Exists(y => y.UserPlanetId == x.UserPlanetId)))
                      LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == exists.UserPlanetId).CopyTo(exists);

                  // Olmayanları buluyoruz.
                  IEnumerable<UserPlanetDTO> notInList = data.Where(x => !LoginController.LC.CurrentUser.UserPlanets.Exists(y => y.UserPlanetId == x.UserPlanetId));

                  // Kullanıcının gezegenlerin ekliyoruz.
                  LoginController.LC.CurrentUser.UserPlanets.AddRange(notInList);

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
