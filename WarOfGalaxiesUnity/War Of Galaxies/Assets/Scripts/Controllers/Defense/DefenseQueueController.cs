using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseQueueController : BaseLanguageBehaviour
{
    public static DefenseQueueController DQC { get; set; }

    private void Awake()
    {
        if (DQC == null)
            DQC = this;
        else
            Destroy(gameObject);
    }

    [Header("Kuyruk oluştuğunda eklenecek olan eşya.")]
    public GameObject DefenseQueueItem;

    [Header("Robot fabrikası kuyruğunu buraya yükleyeceğiz.")]
    public Transform ContentField;

    public void RefreshDefenseQueue()
    {
        // Eskilerini siliyoruz.
        foreach (Transform child in ContentField)
            Destroy(child.gameObject);

        // Şimdi yenilerini basıyoruz.
        foreach (UserPlanetDefenseProgDTO queue in LoginController.LC.CurrentUser.UserPlanetDefenseProgs)
        {
            // Kuyruk eşyası.
            GameObject go = Instantiate(DefenseQueueItem,ContentField);

            // Resmi yüklüyoruz.
            go.transform.Find("ItemImage").GetComponent<Image>().sprite = DefenseController.DC.DefenseWithImages.Find(x => x.Defense == queue.DefenseId).DefenseImage;

            // İsmini basıyoruz.
            go.transform.Find("ItemName").GetComponent<TMP_Text>().text = base.GetLanguageText($"D{queue.DefenseId}");

            // Miktarı basıyoruz.
            go.transform.Find("ItemCount").GetComponent<TMP_Text>().text = queue.DefenseCount.ToString();
        }
    }

}
