using Assets.Scripts.ApiModels;
using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Oluşturulan filolar")]
    public List<FleetPanelItemController> Fleets = new List<FleetPanelItemController>();

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

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void RefreshActiveFleets()
    {
        // Filolaları
        foreach (FleetDTO fleet in FleetController.FC.Fleets)
        {
            // Listede var ise buluyoruz.
            bool isAlreadyInListFleet = Fleets.Any(x => x.FleetInfo.FleetId == fleet.FleetId);

            // Eğer yok ise oluşturuyoruz.
            if (!isAlreadyInListFleet)
            {
                // Olmayan filoyu ekliyoruz.
                GameObject newFleet = Instantiate(FleetItem, FleetItemContent.content);

                // Filo kontrollerini alıyoruz.
                FleetPanelItemController fpic = newFleet.GetComponent<FleetPanelItemController>();

                // Listeye filomuzu ekliyoruz.
                Fleets.Add(fpic);

                // Filo bilgisini de veriyoruz.
                fpic.FleetInfo = fleet;

                // Ve hesaplamalara başlıyoruz.
                fpic.StartCoroutine(fpic.LoadData(fleet));
            }
        }

        int index = 0;
        var currentDate = DateTime.Now;
        foreach (FleetPanelItemController fleet in Fleets.OrderBy(x => x.GetHalfOfFlyDate < currentDate ? x.GetEndFlyDate : x.GetHalfOfFlyDate))
        {
            fleet.transform.SetSiblingIndex(index);
            index++;
        }

    }
}
