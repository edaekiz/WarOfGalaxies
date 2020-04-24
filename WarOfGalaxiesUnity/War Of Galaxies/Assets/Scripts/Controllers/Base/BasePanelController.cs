using System;
using UnityEngine;
using static GlobalPanelController;

public class BasePanelController : MonoBehaviour
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

    private void Awake()
    {
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

    private void Update()
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

}
