using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    public static NotificationController NC { get; set; }

    /// <summary>
    /// Bekleyen bildirimlerin listesi.
    /// </summary>
    public List<Tuple<string, Action>> WaitingActions = new List<Tuple<string, Action>>();

    [Header("Bildirimlerin yaşam süresi")]
    public float NotificationLifeTime;

    private void Awake()
    {
        if (NC == null)
            NC = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Bildirim gösteriyoruz.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="callBackOnClick"></param>
    public void AddNotificationQueue(string message, Action callBackOnClick)
    {
        // Bekleyen bildirimlere ekliyoruz.
        WaitingActions.Add(new Tuple<string, Action>(message, callBackOnClick));

        // Eğer sadece bir bildirim  var ise bildirimi gösteriyoruz. Birden fazla ise zaten gösterim mevcuttur.
        if (WaitingActions.Count == 1)
            ShowNextNotificationPanel();
    }

    public void ShowNextNotificationPanel()
    {
        // Bir sonraki gösterilecek olan bildirimi buluyoruz.
        Tuple<string, Action> nextAction = WaitingActions.FirstOrDefault();

        // Notificationı açıyoruz.
        GameObject notificationPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.NotificationPanel);

        // Notification panelini buluyoruz.
        NotificationPanelController npc = notificationPanel.GetComponent<NotificationPanelController>();

        // Notificationa metni yüklüyoruz.
        npc.LoadData(nextAction.Item1, nextAction.Item2);

        // Bildirim kapandığında bir sonrakine geçeceğiz.
        npc.OnPanelClose = new Action(() =>
        {
            // Gösterileni siliyoruz.
            WaitingActions.Remove(nextAction);

            // Bir sonraki var ise gösteriyoruz.
            if (WaitingActions.Count > 0)
                ShowNextNotificationPanel();
        });
    }
}
