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

    /// <summary>
    /// Aktif seçili olan gezegen.
    /// </summary>
    public UserPlanetDTO CurrentPlanet { get; set; }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

        // Default gezegeni seçiyoruz.
        CurrentPlanet = LoginController.LC.CurrentUser.UserPlanets.FirstOrDefault();
    }

    public void ShowPlanetPickerPanel()
    {
        GameObject panel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.PlanetPickerPanel);
        panel.GetComponent<PlanetPickerController>().ReLoadPlanets();
    }

}
