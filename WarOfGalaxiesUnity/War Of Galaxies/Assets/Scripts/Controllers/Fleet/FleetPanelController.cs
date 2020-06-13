using Assets.Scripts.ApiModels;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FleetPanelController : BasePanelController
{
    public static FleetPanelController FPC { get; set; }

    [Header("Filo bilgilerini yükleyeceğimiz pref.")]
    public GameObject FleetItem;

    [Header("Filoların yükleneceği alan.")]
    public ScrollRect FleetItemContent;

    private void Awake()
    {
        if (FPC == null)
            FPC = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Aktif filoları getiriyoruz.
        RefreshActiveFleets();
    }

    public void RefreshActiveFleets()
    {
        foreach (Transform showFleet in FleetItemContent.content)
            Destroy(showFleet.gameObject);

        // Filolaları
        foreach (FleetDTO fleet in FleetController.FC.Fleets.OrderBy(x => x.FleetLoadDate.AddSeconds(x.EndLeftTime)))
        {
            // Eğer gidiyor ise dönüşü ekrana basmayacağız.
            if (FleetController.FC.Fleets.Any(x => x.ReturnFleetId == fleet.FleetId))
                continue;

            // Olmayan filoyu ekliyoruz.
            GameObject newFleet = Instantiate(FleetItem, FleetItemContent.content);

            // Filo kontrollerini alıyoruz.
            FleetPanelItemController fpic = newFleet.GetComponent<FleetPanelItemController>();

            // Filo bilgisini de veriyoruz.
            fpic.FleetInfo = fleet;

            // Ve hesaplamalara başlıyoruz.
            fpic.StartCoroutine(fpic.LoadData(fleet));
        }
    }
}
