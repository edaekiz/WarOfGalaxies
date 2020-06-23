using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InfoPanelController : BasePanelController
{
    [Header("Panelin yaşam süresi.")]
    public float LifeTime = 5;

    [Header("Panelin içeriği burada yer alacak.")]
    public TMP_Text TXT_InfoContent;

    protected override void Start()
    {
        base.Start();

        // Önceki açık olanları buluyoruz.
        IEnumerable<Tuple<PanelData, BasePanelController>> panels = GlobalPanelController.GPC.OpenPanels.Where(x => x.Item1.PanelType == base.PanelType && x.Item2.GetInstanceID() != GetInstanceID());

        // Açık olanları siliyoruz.
        foreach (Tuple<PanelData, BasePanelController> panel in panels)
            panel.Item2.ClosePanel(true);
    }

    public void LoadText(string info)
    {
        TXT_InfoContent.text = info;
        Invoke("CloseAfterSeconds", LifeTime);
    }

    public void CloseAfterSeconds() => base.ClosePanel();

}
