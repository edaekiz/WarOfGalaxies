using Assets.Scripts.ApiModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FleetController : MonoBehaviour
{
    public static FleetController FC { get; set; }

    [Header("Filoları tutar.")]
    public List<FleetDTO> Fleets = new List<FleetDTO>();

    private void Awake()
    {
        if (FC == null)
            FC = this;
        else
            Destroy(gameObject);
    }

    IEnumerator Start()
    {
        // Giriş yaptığımızdan emin oluyoruz.
        yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

        // 15 saniyede bir filoları kontrol ediyoruz.
        InvokeRepeating("GetLatestFleets", 0, 15);

    }

    public void GetLatestFleets()
    {
        ApiService.API.Post("GetLastFleets", new GetLastFleetsDTO { LastFleetId = Fleets.Select(x => x.FleetId).DefaultIfEmpty(0).Max() }, (ApiResult response) =>
         {

             if (response.IsSuccess)
             {
                 var newFleets = response.GetDataList<FleetDTO>();

                 if (newFleets.Count > 0)
                 {
                     Fleets.AddRange(newFleets);
                 }
             }
         });
    }

    public void RefreshFleetList()
    {

    }

    public void ShowFleetPanel()
    {
        GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.FleetPanel);
    }

}
