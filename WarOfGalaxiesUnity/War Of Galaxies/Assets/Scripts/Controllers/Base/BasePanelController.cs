using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Extends;
using System;
using UnityEngine;
using static GlobalPanelController;

public class BasePanelController : BaseLanguageBehaviour
{
    [Serializable]
    public enum Transitions { Bottom, Left, Right, Top, Shrink }

    [HideInInspector]
    [Header("Panel türü.")]
    public PanelTypes PanelType;

    [Header("Panelin açılış hızı.")]
    public float TransitionSpeed;

    [Header("Açılırken kullanılacak transition.")]
    public Transitions OpenTransition;

    [Header("Kapanırken kullanılacak transition.")]
    public Transitions CloseTransition;

    [Header("Bütün nesnelerin içinde bulunduğu panel.")]
    public RectTransform MainPanel;

    [Header("Gereken metal detayı.")]
    public ResourceDetailController MetalDetail;

    [Header("Gereken kristal detayı.")]
    public ResourceDetailController CrystalDetail;

    [Header("Gereken boron detayı.")]
    public ResourceDetailController BoronDetail;

    // Panel kapanıyorsa bu değer true olacak..
    private bool isClosing;

    // Panel açılıyor ise bu değer true olacak.
    private bool isOpening;

    // Açılırken ulacağı nokta.
    private Vector2 openDestination;

    // Açılırken ulaşacağı scale.
    private Vector3 openScale;

    // Panel Çıkış limitleri.
    private float minOutY = -1000;
    private float minOutX = -1920;
    private float maxOutY = 1000;
    private float maxOutX = 1920;

    protected virtual void Start()
    {
        // Panelin açılıyor olduğunu söylüyoruz.
        isOpening = true;

        // Açılış da ulaşacağı nokta.
        openDestination = MainPanel.anchoredPosition;

        // Açılışta ulaşacağı scale.
        openScale = MainPanel.localScale;

        // Açılış da ekranın altına kaydırıyoruz.
        switch (OpenTransition)
        {
            case Transitions.Bottom:
                MainPanel.anchoredPosition = new Vector2(MainPanel.anchoredPosition.x, minOutY);
                break;
            case Transitions.Left:
                MainPanel.anchoredPosition = new Vector2(minOutX, MainPanel.anchoredPosition.y);
                break;
            case Transitions.Right:
                MainPanel.anchoredPosition = new Vector2(maxOutX, MainPanel.anchoredPosition.y);
                break;
            case Transitions.Top:
                MainPanel.anchoredPosition = new Vector2(MainPanel.anchoredPosition.x, maxOutY);
                break;
            case Transitions.Shrink:
                MainPanel.localScale = Vector3.zero;
                break;
            default:
                break;
        }
    }

    protected virtual void Update()
    {
        // Kapanış animasyonları.
        CheckOpening();

        // Açılış animasyonları.
        CheckClosing();
    }

    #region Açılış Animasyonları

    private void CheckOpening()
    {
        // Eğer panel açılmıyor ise geri dön.
        if (!isOpening)
            return;

        // Seçili olan açılış animasyonuna göre efekt yapıyoruz.
        switch (OpenTransition)
        {
            case Transitions.Bottom:
                CheckOpeningBottomTransition();
                break;
            case Transitions.Left:
                CheckOpeningLeftTransition();
                break;
            case Transitions.Right:
                CheckOpeningRightTransition();
                break;
            case Transitions.Top:
                CheckOpeningTopTransition();
                break;
            case Transitions.Shrink:
                CheckOpeningShringTransition();
                break;
            default:
                break;
        }

    }
    private void CheckOpeningShringTransition()
    {
        // Bir miktar büyütüyoruz.
        MainPanel.localScale += Vector3.one * Time.deltaTime * TransitionSpeed;

        // Eğer yeterince büyüdüyse sonlandırıyoruz.
        if (MainPanel.localScale.x > openScale.x && MainPanel.localScale.y > openScale.y)
            OnOpenTransitionCompleted();

    }
    private void OnOpenTransitionCompleted()
    {
        // Açıl tamamlandığında.
        OnTransionCompleted(false);

        // Panelin olması gerektiği konumda olduğundan emin oluyoruz.
        MainPanel.anchoredPosition = openDestination;

        //Panelin scalesinden emin oluyoruz.
        MainPanel.localScale = openScale;

        // Açılış animasyonunu durduruyoruz..
        isOpening = false;
    }
    private void CheckOpeningRightTransition()
    {
        // Soldan sağa doğru oyun ekranına getiriyoruz.
        MainPanel.anchoredPosition += Vector2.left * Time.deltaTime * TransitionSpeed;

        // Eğer yeterince yaklaştıysa animasyonu durduruyoruz.
        if (MainPanel.anchoredPosition.x < openDestination.x)
            OnOpenTransitionCompleted();
    }
    private void CheckOpeningLeftTransition()
    {
        // Sağdan sola doğru oyun ekranına getiriyoruz.
        MainPanel.anchoredPosition += Vector2.right * Time.deltaTime * TransitionSpeed;

        // Eğer yeterince yaklaştıysa animasyonu durduruyoruz.
        if (MainPanel.anchoredPosition.x > openDestination.x)
            OnOpenTransitionCompleted();
    }
    private void CheckOpeningBottomTransition()
    {
        // Ağaşıdan yukarı doğru oyun ekranına getiriyoruz..
        MainPanel.anchoredPosition += Vector2.up * Time.deltaTime * TransitionSpeed;

        // Eğer yeterince yaklaştıysa animasyonu durduruyoruz.
        if (MainPanel.anchoredPosition.y > openDestination.y)
            OnOpenTransitionCompleted();
    }
    private void CheckOpeningTopTransition()
    {
        // Yukarıdan aşağı doğru oyun ekranına getiriyoruz..
        MainPanel.anchoredPosition += Vector2.down * Time.deltaTime * TransitionSpeed;

        // Eğer yeterince yaklaştıysa animasyonu durduruyoruz.
        if (MainPanel.anchoredPosition.y < openDestination.y)
            OnOpenTransitionCompleted();
    }

    #endregion

    #region Kapanış Animasyonları

    private void CheckClosing()
    {
        // Eğer panel kapanmıyor ise geri dön.
        if (!isClosing)
            return;

        // Seçili olan kapanış animasyonuna göre işlem yapacağız.
        switch (CloseTransition)
        {
            case Transitions.Bottom:
                CheckClosingBottomTransition();
                break;
            case Transitions.Left:
                CheckClosingLeftTransition();
                break;
            case Transitions.Right:
                CheckClosingRightTransition();
                break;
            case Transitions.Top:
                CheckClosingTopTransition();
                break;
            case Transitions.Shrink:
                CheckClosingShringTransition();
                break;
            default:
                break;
        }
    }
    private void OnCloseTransitionCompleted()
    {
        // Kapandığını söylüyoruz.
        OnTransionCompleted(true);

        // Paneli sistemden kaldırıyoruz.
        GlobalPanelController.GPC.ClosePanel(PanelType);

        // Tamamlandığında objeyi yok edeceğiz.
        Destroy(gameObject);
    }
    private void CheckClosingShringTransition()
    {
        // Bir miktar büyütüyoruz.
        MainPanel.localScale -= Vector3.one * Time.deltaTime * TransitionSpeed;

        // Eğer yeterince büyüdüyse sonlandırıyoruz.
        if (MainPanel.localScale.x <= 0 && MainPanel.localScale.y <= 0)
            OnCloseTransitionCompleted();

    }
    private void CheckClosingRightTransition()
    {
        // Paneli sağa doğru kaydırıyoruz.
        MainPanel.anchoredPosition += Vector2.right * Time.deltaTime * TransitionSpeed;

        // Ekranın dışına çıktığında kaybolacak.
        if (MainPanel.anchoredPosition.x > maxOutX)
            OnCloseTransitionCompleted();
    }
    private void CheckClosingLeftTransition()
    {
        // Paneli sola doğru kaydırıyoruz.
        MainPanel.anchoredPosition += Vector2.left * Time.deltaTime * TransitionSpeed;

        // Ekranın dışına çıktığında kaybolacak.
        if (MainPanel.anchoredPosition.x < minOutX)
            OnCloseTransitionCompleted();
    }
    private void CheckClosingBottomTransition()
    {
        // Paneli aşağı doğru kaydırıyoruz.
        MainPanel.anchoredPosition += Vector2.down * Time.deltaTime * TransitionSpeed;

        // Ekranın dışına çıktığında kaybolacak.
        if (MainPanel.anchoredPosition.y < minOutY)
            OnCloseTransitionCompleted();
    }
    private void CheckClosingTopTransition()
    {
        // Paneli yukarı doğru kaydırıyoruz.
        MainPanel.anchoredPosition += Vector2.up * Time.deltaTime * TransitionSpeed;

        // Ekranın dışına çıktığında kaybolacak.
        if (MainPanel.anchoredPosition.y > maxOutY)
            OnCloseTransitionCompleted();
    }

    #endregion

    public void ClosePanel()
    {
        // Eğer açılıyor ise geri dön kapatılamaz.
        if (isOpening)
            return;

        // Eğer açılmıyor ise kapatılıyor olarak ayarlıyoruz.
        isClosing = true;
    }

    protected virtual void OnTransionCompleted(bool isClosed)
    {
    }

    /// <summary>
    /// Eğer yeterli material var ise true döner.
    /// </summary>
    /// <param name="resources"></param>
    /// <returns></returns>
    public bool SetResources(ResourcesDTO resources)
    {
        // Yeterli material var mı?
        bool isEnoughMat = true;

        #region Kaynak ikonları üstüne tıklandığında yazacak olan detaylar.

        // Eğer metal kaynağı 0 dan fazla ise yazabiliriz. Ancak değil ise kapatacağız.
        if (resources.Metal > 0)
        {
            // Metal miktarını basıyoruz.
            MetalDetail.ContentField.text = ResourceExtends.ConvertToDottedResource(resources.Metal);

            // Açık ise açmaya gerek yok paneli
            if (!MetalDetail.gameObject.activeSelf)
                MetalDetail.gameObject.SetActive(true);
        }
        else // Aksi durumda kapatıyoruz.
            MetalDetail.gameObject.SetActive(false);

        // Eğer kristal kaynağı 0 dan fazla ise yazabiliriz. Ancak değil ise kapatacağız.
        if (resources.Crystal > 0)
        {
            CrystalDetail.ContentField.text = ResourceExtends.ConvertToDottedResource(resources.Crystal);

            // Açık değil ise açacağız.
            if (!CrystalDetail.gameObject.activeSelf)
                CrystalDetail.gameObject.SetActive(true);
        }
        else
            CrystalDetail.gameObject.SetActive(false);

        if (resources.Boron > 0)
        {
            // Boron miktarını basıyoruz.
            BoronDetail.ContentField.text = ResourceExtends.ConvertToDottedResource(resources.Boron);

            // Açık ise açmaya gerek yok paneli
            if (!BoronDetail.gameObject.activeSelf)
                BoronDetail.gameObject.SetActive(true);
        }
        else
            BoronDetail.gameObject.SetActive(false);

        #endregion

        #region Kaynak gereksinimlerini basıyoruz.

        // Gereken metal kaynağı.
        MetalDetail.QuantityText.text = ResourceExtends.ConvertResource(resources.Metal);

        // Eğer gereken kaynak kadar kaynağı yok ise gezegenin kırmızı yanacak.
        if (resources.Metal > GlobalPlanetController.GPC.CurrentPlanet.Metal)
        {
            // Metal madenini kırmzııya boyuyoruz.
            MetalDetail.QuantityText.color = Color.red;

            // Yeterli kaynak yok.
            isEnoughMat = false;
        }
        else
            MetalDetail.QuantityText.color = Color.white;

        // Gereken kristal kaynağı.
        CrystalDetail.QuantityText.text = ResourceExtends.ConvertResource(resources.Crystal);

        // Eğer gereken kaynak kadar kaynağı yok ise gezegenin kırmızı yanacak.
        if (resources.Crystal > GlobalPlanetController.GPC.CurrentPlanet.Crystal)
        {
            // Kristal madenini kırmzııya boyuyoruz.
            CrystalDetail.QuantityText.color = Color.red;

            // Yeterli kaynak yok.
            isEnoughMat = false;
        }
        else
            CrystalDetail.QuantityText.color = Color.white;

        // Gereken boron kaynağı.
        BoronDetail.QuantityText.text = ResourceExtends.ConvertResource(resources.Boron);

        // Eğer gereken kaynak kadar kaynağı yok ise gezegenin kırmızı yanacak.
        if (resources.Boron > GlobalPlanetController.GPC.CurrentPlanet.Boron)
        {
            // Bor madenini kırmzııya boyuyoruz.
            BoronDetail.QuantityText.color = Color.red;

            // Yeterli kaynak yok.
            isEnoughMat = false;
        }
        else
            BoronDetail.QuantityText.color = Color.white;

        #endregion

        // Eğer materyal yeterli ise true döner.
        return isEnoughMat;
    }

}
