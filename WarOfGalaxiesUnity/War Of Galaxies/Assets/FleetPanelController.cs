﻿using Assets.Scripts.ApiModels;
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

        RefreshPanelItems();

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void RefreshPanelItems()
    {
        // Filolaları
        foreach (FleetDTO fleet in FleetController.FC.Fleets)
        {
            // Listede var ise buluyoruz.
            bool isAlreadyInListFleet = Fleets.Any(x => x.FleetInfo.FleetId == fleet.FleetId);
            
            // Eğer yok ise oluşturuyoruz.
            if(!isAlreadyInListFleet)
            {
                // Olmayan filoyu ekliyoruz.
                GameObject newFleet = Instantiate(FleetItem, FleetItemContent.content);

                // Filo kontrollerini alıyoruz.
                FleetPanelItemController fpic = newFleet.GetComponent<FleetPanelItemController>();

                // Listeye filomuzu ekliyoruz.
                Fleets.Add(fpic);

                // Ve hesaplamalara başlıyoruz.
                fpic.StartCoroutine(fpic.LoadData(fleet));
            }
        }
    }

}
