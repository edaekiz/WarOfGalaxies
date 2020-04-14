using Assets.Scripts.ApiModels;
using TMPro;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    public static LoginController LC { get; set; }

    [Header("Giriş yapan kullanıcılar.")]
    public UserDTO CurrentUser;

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
                CurrentUser = response.GetData<UserDTO>();

                // Kullanıcı adını basıyoruz.
                UsernameField.text = CurrentUser.Username;

                LoadingController.LC.IncreaseLoadCount();
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
