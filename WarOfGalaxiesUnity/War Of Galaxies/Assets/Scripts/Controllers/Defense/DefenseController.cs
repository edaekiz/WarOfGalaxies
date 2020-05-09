using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public class DefenseController : MonoBehaviour
{
    public static DefenseController DC { get; set; }
    public List<DefenseImageDTO> DefenseWithImages;
    private void Awake()
    {
        if (DC == null)
            DC = this;
        else
            Destroy(gameObject);
    }

    
    public void ShowDefensePanel()
    {
        GameObject panel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.DefensePanel);
        panel.GetComponent<DefensePanelController>().LoadAllDefenses();
    }

}
