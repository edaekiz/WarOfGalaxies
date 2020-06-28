using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseItemController : BaseLanguageBehaviour
{
    [Header("Aktif tuttuğu savunma.")]
    public Defenses CurrentDefense;

    [Header("Savunma ismini basıyoruz.")]
    public TMP_Text DefenseName;

    [Header("Kullanıcının sahip olduğu miktar.")]
    public TMP_Text DefenseCount;

    [Header("Savunma resmi.")]
    public Image DefenseImage;

    [Header("Keşfedilmemiş savunmaların rengi")]
    public Color32 NotInventedItemColor;

    [Header("Keşfedilmemiş savunmaların üstünde olacak ikon.")]
    public GameObject LockedIcon;

    public void LoadDefenseDetails(Defenses defense)
    {
        // Savunma bilgisi.
        CurrentDefense = defense;

        // İsmini basıyoruz.
        DefenseName.text = base.GetLanguageText($"D{(int)defense}");

        // Resmi yüklüyoruz.
        DefenseImage.sprite = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == defense).DefenseImage;

        // Aktif savunma miktarı.
        UserPlanetDefenseDTO currentDefenseCount = LoginController.LC.CurrentUser.UserPlanetDefenses.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.DefenseId == CurrentDefense);

        // Eğer yok ise gemisi 0 var ise miktarı basıyoruz.
        DefenseCount.text = currentDefenseCount == null ? $"{0}" : $"{currentDefenseCount.DefenseCount}";

        // Keşfedilmedi ise keşfedilmedi uyarısını çıkaraağız.
        if (!TechnologyController.TC.IsInvented(TechnologyCategories.Savunmalar, (int)defense))
        {
            // Disabled rengine boyuyoruz.
            GetComponent<Image>().color = NotInventedItemColor;

            // Disabled rengine boyuyoruz gemiyi.
            DefenseImage.color = NotInventedItemColor;

            // Kilitli ikonunu açıyoruz.
            LockedIcon.SetActive(true);
        }
    }

    public void ShowDefenseDetail()
    {
        GameObject defenseDetailPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.DefenseDetailPanel);
        DefenseDetailItemPanel ddip = defenseDetailPanel.GetComponent<DefenseDetailItemPanel>();
        ddip.LoadDefenseDetails(CurrentDefense);
    }

}
