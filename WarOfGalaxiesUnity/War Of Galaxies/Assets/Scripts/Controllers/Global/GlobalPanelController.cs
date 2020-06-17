using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalPanelController : MonoBehaviour
{
    public static GlobalPanelController GPC { get; set; }

    [Serializable]
    public enum PanelTypes
    {
        BuildingPanel,
        ResearchPanel,
        ResearchDetailPanel,
        ShipyardPanel,
        ShipyardDetailPanel,
        DefensePanel,
        DefenseDetailPanel,
        PlanetPickerPanel,
        GalaxyPlanetActionPanel,
        PlanetActionShipSelectionStep,
        QuantityPanel,
        FleetPanel,
        NotificationPanel,
        MailPanel,
        MailDetailPanel,
        YesNoPanel,
        PlanetActionFooterPanel,
        MailBattleReport,
        MailSpyReport,
        MailTextReport
    }

    [Serializable]
    public class PanelData
    {
        public PanelTypes PanelType;
        public GameObject Prefab;
    }

    private void Awake()
    {
        if (GPC == null)
            GPC = this;
        else
            Destroy(gameObject);
    }

    [Header("Oyundaki bütün panellerin listesi.")]
    public List<PanelData> Panels;

    // Açık olan panellerin listesi.
    public List<Tuple<PanelData, BasePanelController>> OpenPanels;

    private void Start()
    {
        // Açık panelleri burada tutacağız.
        OpenPanels = new List<Tuple<PanelData, BasePanelController>>();
    }

    /// <summary>
    /// Seçilen paneli açar.
    /// </summary>
    /// <param name="panelType"></param>
    /// <returns></returns>
    public GameObject ShowPanel(PanelTypes panelType)
    {
        // Panel datasını buluyoruz.
        PanelData panelData = Panels.Find(x => x.PanelType == panelType);

        // Eğer panel yok ise hata döneceğiz.
        if (panelData == null)
        {
            Debug.LogError("Panel bulunamadı!");

            // Eğer panel bulunamadıysa boş dön.
            return null;
        }

        // Paneli açıyoruz.
        GameObject panelObject = Instantiate(panelData.Prefab, Vector3.zero, Quaternion.identity);

        // Base paneli var mı kontrol ediyoruz.
        BasePanelController basePanel = panelObject.GetComponent<BasePanelController>();

        // Eğer panel değil ise geri dön.
        if (basePanel == null)
        {
            // Uyarı dönüyoruz.
            Debug.LogError("Bu nesne base panelden beslenmiyor.");

            // Paneli yok ediyoruz.
            Destroy(panelObject);

            // Boş dönüyoruz.
            return null;
        }

        // Panel bilgisini veriyoruz.
        basePanel.PanelType = panelType;

        // Açık panellerin listesine ekliyoruz.
        OpenPanels.Add(new Tuple<PanelData, BasePanelController>(panelData, basePanel));

        return panelObject;
    }

    /// <summary>
    /// Açık olan bir paneli kapatır.
    /// </summary>
    /// <param name="panelType">Kapatılacak olan panel.</param>
    public void ClosePanel(PanelTypes panelType)
    {
        // Açık olan paneli buluyoruz.
        Tuple<PanelData, BasePanelController> panel = OpenPanels.Find(x => x.Item1.PanelType == panelType);

        // Eğer panel yok ise hata dön.
        if (panel == null)
            return;

        // Ancak yinede listeden siliyoruz.
        OpenPanels.Remove(panel);
    }

    /// <summary>
    /// Eğer açık panel var ise değer true döner.
    /// </summary>
    /// <returns></returns>
    public bool IsAnyPanelOpen => OpenPanels.Count(x => x.Item2.IsPanel) > 0;

}
