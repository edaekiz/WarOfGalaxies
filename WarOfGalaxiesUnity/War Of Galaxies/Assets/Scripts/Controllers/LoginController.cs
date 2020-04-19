using Assets.Scripts.ApiModels;
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

    // Update is called once per frame
    void Update()
    {

    }
}
