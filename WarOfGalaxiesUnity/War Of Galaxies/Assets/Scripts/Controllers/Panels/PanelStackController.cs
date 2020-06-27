using Assets.Scripts.Controllers.Base;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelStackController : BaseLanguageBehaviour
{
    public static PanelStackController PSC { get; set; }

    /// <summary>
    /// Stacklenmiş olan panelleri tutacağız.
    /// </summary>
    public Stack<BasePanelController> OpenPanels = new Stack<BasePanelController>();

    [Header("Stackde bir geri gider.")]
    public Button BTN_BackButton;

    [Header("Stackden çıkar.")]
    public Button BTN_ExitButton;

    [Header("Kaynaklar ve Panel ismi burada yer alacak.")]
    public GameObject ResourcesCanvas;

    [Header("Panelin ismini buraya basacağız.")]
    public TMP_Text TXT_PanelName;

    [Header("İki view da kapatılacağında panelleri görüntülemek için kameraya ihtiyacımız olacak.")]
    public Camera PanelCamera;

    private void Awake()
    {
        if (PSC == null)
            PSC = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        BTN_BackButton.onClick.AddListener(() => Pop());
        BTN_ExitButton.onClick.AddListener(() => PopAll());
        ValidateButtons();
    }

    public void Push(BasePanelController basePanelController)
    {
        try
        {
            // Eğer hiç panel açık değil ise dön.
            if (OpenPanels.Count > 0)
            {
                // Son eklenen kayıt ekranda gözüküyor bu yüzden onu kapatıp yenisini eklemeliyiz.
                BasePanelController lastItem = OpenPanels.FirstOrDefault();

                // Eğer var ise kapatıyoruz.
                lastItem.gameObject.SetActive(false);
            }

            // Açılan panelin ismini basıyoruz.
            TXT_PanelName.text = base.GetLanguageText(basePanelController.PanelNameKeyword);

            // Ve üstüne açılan paneli ekliyoruz.
            OpenPanels.Push(basePanelController);
        }
        catch (System.Exception)
        {

            throw;
        }
        finally
        {
            ValidateButtons();
        }
    }

    public void Pop()
    {
        try
        {
            // Eğer hiç panel açık değil ise dön.
            if (OpenPanels.Count == 0)
                return;

            // Son paneli çıkartıyoruz.
            BasePanelController closedPanel = OpenPanels.Pop();

            // Paneli kapatıyoruz.
            closedPanel.ClosePanelForStack();

            // Eğer hiç panel açık değil ise dön.
            if (OpenPanels.Count == 0)
                return;

            // Var ise sonrakini aktif etmemiz gerekiyor.
            BasePanelController lastItem = OpenPanels.FirstOrDefault();

            // Tabiki kontrol ediyoruz var mı diye?
            if (lastItem != null)
            {
                // Paneli açıyoruz.
                lastItem.gameObject.SetActive(true);

                // Son açık olan panelin ismini basıyoruz.
                TXT_PanelName.text = base.GetLanguageText(lastItem.PanelNameKeyword);
            }
        }
        catch (System.Exception)
        {

            throw;
        }
        finally
        {
            ValidateButtons();
        }
    }

    public void PopAll()
    {
        try
        {
            // Bütün panelleri kapatıyoruz.
            foreach (BasePanelController openPanel in OpenPanels)
            {
                if (!openPanel.gameObject.activeSelf)
                    openPanel.gameObject.SetActive(true);
                openPanel.ClosePanelForStack();
            }

            // Panel stack listesini temizliyoruz.
            OpenPanels.Clear();
        }
        catch (System.Exception)
        {
            throw;
        }
        finally
        {
            ValidateButtons();
        }
    }

    public void ValidateButtons()
    {
        if (OpenPanels.Count == 0)
        {
            // Panel detaylarını kapatyoruz.
            CloseStackPanelItems();

            // Eğer galaksi görünümünde ve galaksi açık ise kapatıyoruz. Arkada çalışmasına gerek yok.
            if (GlobalGalaxyController.GGC.IsInGalaxyView && !GlobalGalaxyController.GGC.GalaxyView.activeSelf)
                GlobalGalaxyController.GGC.GalaxyView.SetActive(true);

            // Eğer gezegen görünümü aktifse ve planet görünümü hala açık ise kapatıyoruz.
            if (!GlobalGalaxyController.GGC.IsInGalaxyView && !GlobalGalaxyController.GGC.PlanetView.activeSelf)
                GlobalGalaxyController.GGC.PlanetView.SetActive(true);
        }
        else
        {
            // Panel detaylarını açıyoruz. Geri butonu felan.
            OpenStackPanelItems();

            // Aşağıdaki kodların amacı arkaplanda oyunun kasmasını önlemek.

            // Eğer galaksi görünümünde ve galaksi açık ise kapatıyoruz. Arkada çalışmasına gerek yok.
            if (GlobalGalaxyController.GGC.IsInGalaxyView && GlobalGalaxyController.GGC.GalaxyView.activeSelf)
                GlobalGalaxyController.GGC.GalaxyView.SetActive(false);

            // Eğer gezegen görünümü aktifse ve planet görünümü hala açık ise kapatıyoruz.
            if (!GlobalGalaxyController.GGC.IsInGalaxyView && GlobalGalaxyController.GGC.PlanetView.activeSelf)
                GlobalGalaxyController.GGC.PlanetView.SetActive(false);
        }
    }

    public void CloseStackPanelItems()
    {
        if (BTN_BackButton.gameObject.activeSelf)
            BTN_BackButton.gameObject.SetActive(false);
        if (BTN_ExitButton.gameObject.activeSelf)
            BTN_ExitButton.gameObject.SetActive(false);
        if (ResourcesCanvas.activeSelf)
            ResourcesCanvas.SetActive(false);
        if (PanelCamera.gameObject.activeSelf)
            PanelCamera.gameObject.SetActive(false);
    }

    public void OpenStackPanelItems()
    {
        if (!BTN_BackButton.gameObject.activeSelf)
            BTN_BackButton.gameObject.SetActive(true);
        if (!BTN_ExitButton.gameObject.activeSelf)
            BTN_ExitButton.gameObject.SetActive(true);
        if (!ResourcesCanvas.activeSelf)
            ResourcesCanvas.SetActive(true);
        if (!PanelCamera.gameObject.activeSelf)
            PanelCamera.gameObject.SetActive(true);
    }

    public BasePanelController GetCurrentPanel()
    {
        // Eğer hiç panel açık değil ise dön.
        if (OpenPanels.Count == 0)
            return null;

        // Son paneli döner.
        return OpenPanels.LastOrDefault();
    }
}
