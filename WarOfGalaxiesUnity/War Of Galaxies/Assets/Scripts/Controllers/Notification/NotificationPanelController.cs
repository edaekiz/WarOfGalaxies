using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanelController : BasePanelController
{
    [Header("Bildirim metninin basılacağı metin.")]
    public TMP_Text NotificationMessage;

    [Header("Tıklandığında çalışacak olan methot.")]
    public Button TriggerButton;

    /// <summary>
    /// Panel kapandığında tetiklenecek olan methot.
    /// </summary>
    public Action OnPanelClose;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(CloseAuto());
    }

    public void LoadData(string message, Action onClick)
    {
        // Eğer tıklama komutu geldiyse tıklandığında tetikliyoruz.
        if (onClick != null)
            TriggerButton.onClick.AddListener(() => { onClick.Invoke(); base.ClosePanel(); });

        // Bildirim mesajını basıyoruz.
        NotificationMessage.text = message;
    }

    private IEnumerator CloseAuto()
    {
        yield return new WaitForSecondsRealtime(NotificationController.NC.NotificationLifeTime);
        base.ClosePanel();
    }

    protected override void OnTransionCompleted(bool isClosed)
    {
        base.OnTransionCompleted(isClosed);
        if (isClosed)
            if (OnPanelClose != null)
                OnPanelClose.Invoke();
    }
}
