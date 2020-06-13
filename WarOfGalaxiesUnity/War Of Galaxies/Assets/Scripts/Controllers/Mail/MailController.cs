using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailController : MonoBehaviour
{
    public static MailController MC { get; set; }

    /// <summary>
    /// Okunmamış olan maillerin listesi.
    /// </summary>
    public List<UserMailDTO> UserMails = new List<UserMailDTO>();

    [Header("Okunmamış olan maillerin miktarını basacağız.")]
    public GameObject UnreadMailCount;

    [Header("Default seçili olan kategori.")]
    public MailCategories LastSelectedCategory = MailCategories.Casusluk;

    private void Awake()
    {
        if (MC == null)
            MC = this;
        else
            Destroy(gameObject);
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => LoadingController.LC.IsGameLoaded);
        StartCoroutine(GetLatestMails());
    }

    public void ShowMailPanel()
    {
        GameObject mailPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.MailPanel);
        MailPanelController mpc = mailPanel.GetComponent<MailPanelController>();
        mpc.ShowCategoryDetails(LastSelectedCategory);
    }

    public IEnumerator GetLatestMails()
    {
        // Son gelen mailleri alıyoruz.
        StartCoroutine(ApiService.API.Post("GetLatestUnReadedMails", new LatestUnReadedUserMailRequestDTO { LastUnReadedMailId = UserMails.Select(x => x.UserMailId).DefaultIfEmpty(0).Max() }, (ApiResult response) =>
        {
            if (response.IsSuccess)
            {
                // Gelen mailleri alıyoruz.
                List<UserMailDTO> newUnreadedMails = response.GetDataList<UserMailDTO>();

                // Eğer miktar sıfırdan büyük ise listeye okunmamışlara ekliyoruz.
                if (newUnreadedMails.Count > 0)
                {
                    IEnumerable<UserMailDTO> mails = newUnreadedMails.Where(x => !UserMails.Any(y => x.UserMailId == y.UserMailId));

                    // Okunmamışlara ekliyoruz.
                    UserMails.AddRange(mails);

                    // Bildirimi gösteriyoruz.
                    ShowNotification(mails.Where(x => !x.IsReaded).Count());

                    // Eğer panel açık ise paneli de yeniliyoruz.
                    if (MailPanelController.MPC != null)
                        MailPanelController.MPC.ShowCategoryDetails(MailPanelController.MPC.CurrentShownCategory);
                }

                // Miktarı güncelliyoruz.
                RefreshMailIconQuantity();
            }

        }));

        // Saniye kadar bekliyoruz.
        yield return new WaitForSecondsRealtime(5);

        // Saniye sonra tekrar çağırıyoruz.
        StartCoroutine(GetLatestMails());
    }

    public void RefreshMailIconQuantity()
    {
        int unreadMailCount = UserMails.Where(x => !x.IsReaded).Count();
        // Eğer okunmamış mesaj var ise ekrana miktarı basıyoruz.
        if (unreadMailCount > 0)
        {
            // Eğer miktar alanı açık değil ise açıyoruz.
            if (!UnreadMailCount.activeSelf)
                UnreadMailCount.SetActive(true);

            // Ve miktarı basıyoruz.
            UnreadMailCount.GetComponentInChildren<TMP_Text>().text = $"{unreadMailCount}";
        }
        else // Eğer okumamış mesaj kalmadı ise kapatıyoruz.
            UnreadMailCount.SetActive(false);
    }

    public void ShowNotification(int newQuantity)
    {
        if (newQuantity > 0)
            NotificationController.NC.AddNotificationQueue(LanguageController.LC.GetText("XMesajınızVar", newQuantity), () => ShowMailPanel());
    }

    public void DeleteMail(UserMailDTO userMail)
    {
        // Öncekini siliyoruz.
        this.UserMails.Remove(userMail);

        // Eğer panel açık ise paneli de yeniliyoruz.
        if (MailPanelController.MPC != null)
            MailPanelController.MPC.ShowCategoryDetails(MailPanelController.MPC.CurrentShownCategory);

    }

}
