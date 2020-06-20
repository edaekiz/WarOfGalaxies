using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System;
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
        StartCoroutine(GetLatestMailsJob());
    }

    public void ShowMailPanel()
    {
        // Mail panelini açıyoruz.
        GameObject mailPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.MailPanel);

        // Mail panelinin componentini buluyoruz.
        MailPanelController mpc = mailPanel.GetComponent<MailPanelController>();

        // Ve son yüklenen kategoriyi yüklüyoruz.
        mpc.ShowCategoryDetails(LastSelectedCategory);
    }

    public IEnumerator GetLatestMailsJob()
    {
        // Son mailleri alıyoruz.
        GetLatestMails();

        // Saniye kadar bekliyoruz.
        yield return new WaitForSecondsRealtime(5);

        // Saniye sonra tekrar çağırıyoruz.
        StartCoroutine(GetLatestMailsJob());
    }

    public void GetLatestMails()
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
                    List<UserMailDTO> mails = newUnreadedMails.Where(x => !UserMails.Any(y => x.UserMailId == y.UserMailId)).ToList();

                    // Okunmamışlara ekliyoruz.
                    UserMails.AddRange(mails);

                    // Okunmamış mail miktarı.
                    int notReadedQuantity = mails.Where(x => !x.IsReaded).Count();

                    if (notReadedQuantity > 0)
                        NotificationController.NC.AddNotificationQueue(LanguageController.LC.GetText("XMesajınızVar", notReadedQuantity), () => ShowMailPanel());

                    // Bildirimi gösteriyoruz.

                    // Eğer panel açık ise paneli de yeniliyoruz.
                    if (MailPanelController.MPC != null)
                        MailPanelController.MPC.ShowCategoryDetails(MailPanelController.MPC.CurrentShownCategory);
                }

                // Miktarı güncelliyoruz.
                RefreshMailIconQuantity();
            }

        }));
    }

    public void RefreshMailIconQuantity()
    {
        // Okunmamış mail miktarı.
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

    public void DeleteMail(UserMailDTO userMail, Action callBack = null)
    {
        // Onay panelini açıyoruz.
        GameObject yesNoPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.YesNoPanel);

        // Kontrolleri alıyoruz.
        YesNoPanelController ynpc = yesNoPanel.GetComponent<YesNoPanelController>();

        // Uyarı paneli açıyoruz. Eğer panelden evet denirse kayıdı siliyoruz sistemden.
        ynpc.LoadData(LanguageController.LC.GetText("Uyarı"), LanguageController.LC.GetText("Silmekİstemek"), () =>
        {
            // Yükleniyor panelini açıyoruz.
            LoadingController.LC.ShowLoading();

            StartCoroutine(ApiService.API.Post("DeleteMail", new MailDeleteRequestDTO { UserMailId = userMail.UserMailId }, (ApiResult response) =>
            {
                // Yükleniyor panelini kapatıyoruz.
                LoadingController.LC.CloseLoading();

                if (response.IsSuccess)
                {
                    // Öncekini siliyoruz.
                    this.UserMails.Remove(userMail);

                    // Eğer panel açık ise paneli de yeniliyoruz.
                    if (MailPanelController.MPC != null)
                        MailPanelController.MPC.ShowCategoryDetails(MailPanelController.MPC.CurrentShownCategory);

                    if (callBack != null)
                        callBack.Invoke();
                }
            }));
        });
    }

}
