using Assets.Scripts.ApiModels;
using Assets.Scripts.Extends;
using Assets.Scripts.Models;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class GlobalPlanetController : MonoBehaviour
{
    public static GlobalPlanetController GPC { get; set; }

    [Header("Gezegenin ismi.")]
    public TMP_Text UserPlanetName;

    [Header("Gezegenin kordinatı.")]
    public TMP_Text UserPlanetCordinate;

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

    public CordinateDTO CurrentPlanetCordinate { get; set; }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

        // Default gezegeni seçiyoruz.
        SelectPlanet(LoginController.LC.CurrentUser.UserPlanets.FirstOrDefault());
    }

    public void SelectPlanet(UserPlanetDTO selectedPlanet)
    {
        CurrentPlanet = selectedPlanet;
        UserPlanetName.text = CurrentPlanet.PlanetName;
        UserPlanetCordinatesDTO cordinateInfo = LoginController.LC.CurrentUser.UserPlanetCordinates.Find(x => x.UserPlanetId == selectedPlanet.UserPlanetId);
        CurrentPlanetCordinate = new CordinateDTO(cordinateInfo.GalaxyIndex, cordinateInfo.SolarIndex, cordinateInfo.OrderIndex);
        UserPlanetCordinate.text = CordinateExtends.ToCordinateString(CurrentPlanetCordinate);
    }

    public void ShowPlanetPickerPanel()
    {
        GameObject panel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.PlanetPickerPanel);
        panel.GetComponent<PlanetPickerController>().ReLoadPlanets();
    }

}
