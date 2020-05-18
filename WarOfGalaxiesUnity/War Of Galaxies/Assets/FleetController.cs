using Assets.Scripts.ApiModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GlobalPanelController;

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
        StartCoroutine(ApiService.API.Post("GetLastFleets", new GetLastFleetsDTO { LastFleetId = Fleets.Select(x => x.FleetId).DefaultIfEmpty(0).Max() }, (ApiResult response) =>
         {

             // Yanıt başarılı ise.
             if (response.IsSuccess)
             {
                 // Yeni gelen filo bilgilerini alıyoruz.
                 List<FleetDTO> newFleets = response.GetDataList<FleetDTO>();

                 // Eğer yeni filo var ise listeye ekliyoruz.
                 if (newFleets.Count > 0)
                 {
                     // Listeye dahil ediyoruz.
                     Fleets.AddRange(newFleets);

                     // Eğer panel açık ise paneli de yeniliyoruz.
                     if (FleetPanelController.FPC != null)
                         FleetPanelController.FPC.RefreshPanelItems();
                 }
             }
         }));
    }

    public void ShowFleetPanel()
    {
        GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.FleetPanel);
    }

}
