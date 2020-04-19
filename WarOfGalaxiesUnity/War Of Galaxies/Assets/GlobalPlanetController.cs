using Assets.Scripts.ApiModels;
using System.Collections;
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

    [Header("Seçili olan gezegen.")]
    public UserPlanetDTO CurrentPlanet;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

        // Default gezegeni seçiyoruz.
        CurrentPlanet = LoginController.LC.CurrentUser.UserPlanets.FirstOrDefault();
    }
}
