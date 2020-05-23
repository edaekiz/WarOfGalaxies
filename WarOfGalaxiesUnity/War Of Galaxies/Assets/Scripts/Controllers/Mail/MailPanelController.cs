using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailPanelController : BasePanelController
{
    public static MailPanelController MPC { get; set; }

    [Serializable]
    public struct MailButtonWithCatgories
    {
        public MailCategories Category;
        public Button CategoryButton;
    }

    [Header("Butonlar ve kategorileri eşlenmiş şekilde tutuyoruz.")]
    public List<MailButtonWithCatgories> ButtonWithCategories = new List<MailButtonWithCatgories>();

    [Header("Aktif gösterilen kategori detayı.")]
    public MailCategories CurrentShownCategory;

    [Header("Ekrana basılacak olan item.")]
    public GameObject MailItem;

    [Header("Maillerin basılacağı alan.")]
    public ScrollRect MailItemContent;

    [Header("Hiç maili yok ise ozaman ufak texti açıyoruz.")]
    public GameObject NoMailAlert;

    private void Awake()
    {
        if (MPC == null)
            MPC = this;
        else
            Destroy(gameObject);
    }

    protected override void Start()
    {
        base.Start();

        // Butonlara görevlerini yüklüyoruz.
        ButtonWithCategories.ForEach(e =>
        {
            e.CategoryButton.onClick.AddListener(() => ShowCategoryDetails(e.Category));
            e.CategoryButton.GetComponentInChildren<TMP_Text>().text = base.GetLanguageText($"MC{(int)e.Category}");
        });
    }

    public void ShowCategoryDetails(MailCategories lastSelectedCategory)
    {
        // Önceki mailleri temizliyoruz.
        ClearCurrentMails();

        // Gösterilen kategori.
        CurrentShownCategory = lastSelectedCategory;

        // Bütün okunmamış mailleri ekrana basıyoruz.
        foreach (UserMailDTO mail in MailController.MC.UserMails.Where(x => x.MailCategoryId == (int)CurrentShownCategory).Reverse())
            Instantiate(MailItem, MailItemContent.content).GetComponent<MailPanelItemController>().LoadMailData(mail);

        // Eğer bu kategoride mail yok ise ekrana uyarıyı gösteriyoruz.
        CheckForNoMessageAlert();

        // Seçili olan kategorinin butonlarını kapatıp açıyoruz.
        VerifyCategoryButtons();
    }

    public void ClearCurrentMails()
    {
        // Eskileri siliyoruz.
        foreach (Transform mailItem in MailItemContent.content)
            Destroy(mailItem.gameObject);
    }

    public void VerifyCategoryButtons()
    {
        // Seçili olan kategoriyi bulup kapatıyoruz. Diğerlerini açıyoruz.
        ButtonWithCategories.ForEach(e =>
        {
            if (e.Category == CurrentShownCategory)
                e.CategoryButton.interactable = false;
            else
                e.CategoryButton.interactable = true;
        });
    }

    public void CheckForNoMessageAlert()
    {
        // Eğer hiç mail yok ise uyarıyı açıyoruz.
        if (!MailController.MC.UserMails.Any(x => x.MailCategoryId == (int)CurrentShownCategory))
            NoMailAlert.SetActive(true);
        else
            NoMailAlert.SetActive(false);
    }

}
