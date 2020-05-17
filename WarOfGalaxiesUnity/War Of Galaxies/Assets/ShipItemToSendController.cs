using System;
using System.Linq;
using Assets.Scripts.ApiModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipItemToSendController : MonoBehaviour
{
    public UserPlanetShipDTO UserPlanetShip { get; set; }

    [Header("Gemiye ait resim buraya basılacak.")]
    public Image ShipImage;

    [Header("Geminin ismi.")]
    public TMP_Text ShipName;

    [Header("Geminin sayısı.")]
    public TMP_Text ShipCount;

    [Header("Gönderilecek gemi sayısı.")]
    public int Quantity;

    public void LoadData(UserPlanetShipDTO userPlanetShip)
    {
        // Gezegendeki gemi miktarıç
        UserPlanetShip = userPlanetShip;

        // Gemiye ait resim.
        Sprite shipImage = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == userPlanetShip.ShipId).ShipImage;

        // Geminin ismi
        string shipName = LanguageController.LC.GetText($"S{(int)userPlanetShip.ShipId}");

        // Geminin resmi.
        ShipImage.sprite = shipImage;

        // Gemi ismini basıyoruz.
        ShipName.text = shipName;

        // Gemi sayısını basıyoruz.
        ShipCount.text = Quantity.ToString();

        // Eğer yeterli miktar yok ise yok et.
        if (Quantity <= 0)
        {
            // Kendisini listeden siliyoruz.
            PlanetActionController.PAC.ShipsToSend.Remove(this);

            // Eğer gemi kalmamış ise butonu kapatıyoruz.
            if (PlanetActionController.PAC.ShipsToSend.Count == 0)
            {
                // Devam et butonunu kapatıyoruz.
                PlanetActionController.PAC.ContinueButton.interactable = false;

                // Gönderilecek gemi kalmadıysa uyarıyı açıyoruz.
                PlanetActionController.PAC.NoShipSelected.SetActive(true);
            }

            // Kendisini yok ediyoruz.
            Destroy(gameObject);
        }
    }

    public void OnClickItem()
    {
        // İptal etmek için miktar panelini açıyoruz.
        GameObject quantityPanel = GlobalPanelController.GPC.ShowPanel(GlobalPanelController.PanelTypes.QuantityPanel);

        // Panele dataları yüklüyoruz.
        quantityPanel.GetComponent<QuantityItemPanel>().LoadData(ShipImage.sprite, ShipName.text, Quantity);

        // Panel tamama basılarak kapatıldığında burası tetiklenecek.
        quantityPanel.GetComponent<QuantityItemPanel>().OnPanelClose += new EventHandler<QuantityItemPanel.QuantityEventArs>((s, e) => AddToUseableQuantity(e.Quantity));
    }

    public void AddToUseableQuantity(int quantity)
    {
        // Eğer 0 girilmiş ise geri dön.
        if (quantity <= 0)
            return;

        // Miktarı azaltıyoruz.
        Quantity -= quantity;

        // Zaten kuyrukta bu gemiden var mı?
        ShipItemCanSendController alreadyInQueue = PlanetActionController.PAC.ShipsCanSend.Find(x => x.UserPlanetShip.ShipId == this.UserPlanetShip.ShipId);

        // Eğer yok ise oluşturacağız.
        if (alreadyInQueue == null)
        {
            // Gemiyi oluşturuyoruz.
            GameObject newShipItemCanSend = Instantiate(PlanetActionController.PAC.UseableShipPref, PlanetActionController.PAC.UseableShipsContent.content);

            // Gemi bilgisini yüklüyecepiz.
            alreadyInQueue = newShipItemCanSend.GetComponent<ShipItemCanSendController>();

            // Oluşturulanı listeye ekliyoruz ki daha sonra güncellemek için kullanalım.
            PlanetActionController.PAC.ShipsCanSend.Add(alreadyInQueue);

            // Eğer uyarı açık ise onu da kapatıyoruz.
            if (PlanetActionController.PAC.ShipNotFoundInPlanet.activeSelf)
                PlanetActionController.PAC.ShipNotFoundInPlanet.SetActive(false);
        }

        // Resim isim gibi bilgileri yüklüyoruz.
        alreadyInQueue.LoadData(this.UserPlanetShip);

        // Verileri yeniliyoruz.
        LoadData(UserPlanetShip);
    }

}
