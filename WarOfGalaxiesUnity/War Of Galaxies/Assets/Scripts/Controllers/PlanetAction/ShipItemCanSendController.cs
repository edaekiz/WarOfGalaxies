using System;
using System.Linq;
using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuantityItemPanel;

public class ShipItemCanSendController : MonoBehaviour
{
    public UserPlanetShipDTO UserPlanetShip { get; set; }

    [Header("GEmiye ait resim buraya basılacak.")]
    public Image ShipImage;

    [Header("Geminin ismi.")]
    public TMP_Text ShipName;

    [Header("Geminin sayısı.")]
    public TMP_Text ShipCount;

    public void LoadData(UserPlanetShipDTO userPlanetShip)
    {
        // Gezegendeki gemi miktarıç
        UserPlanetShip = userPlanetShip;

        // Gemiye ait resim.
        Sprite shipImage = ShipyardController.SC.ShipWithImages.Find(x => x.Ship == userPlanetShip.ShipId).ShipImage;

        // Geminin ismi
        string shipName = LanguageController.LC.GetText($"S{(int)userPlanetShip.ShipId}");

        // Sahip olunan gemi miktarı.
        int shipCount = userPlanetShip.ShipCount - GetUsedQuantity;

        // Geminin resmi.
        ShipImage.sprite = shipImage;

        // Gemi ismini basıyoruz.
        ShipName.text = shipName;

        // Gemi sayısını basıyoruz.
        ShipCount.text = shipCount.ToString();

        // Eğer yeterli miktar kalmadıysa sil.
        if (shipCount <= 0)
        {
            // Eğer kayıt kalmadıysa listeden siliyoruz.
            PlanetActionController.PAC.ShipsCanSend.Remove(this);

            // Eğer daha fazla gemi kalmamış ise uyarıyı açıyoruz
            if (PlanetActionController.PAC.ShipsCanSend.Count == 0)
                PlanetActionController.PAC.ShipNotFoundInPlanet.SetActive(true);

            // Kendisini yok ediyoruz.
            Destroy(gameObject);
        }
    }

    public void OnClickItem()
    {
        GameObject quantityPanel = GlobalPanelController.GPC.ShowPanel(PanelTypes.QuantityPanel);

        // Panele dataları yüklüyoruz.
        quantityPanel.GetComponent<QuantityItemPanel>().LoadData(ShipImage.sprite, ShipName.text, GetUseableQuantity);

        // Panel tamama basılarak kapatıldığında burası tetiklenecek.
        quantityPanel.GetComponent<QuantityItemPanel>().OnPanelClose = new Action<QuantityEventArs>((e) => AddToUsedQueue(e.Quantity));
    }

    public void AddToUsedQueue(int quantity)
    {
        // Eğer 0 girilmiş ise geri dön.
        if (quantity <= 0)
            return;

        // Listede var mı diye kontrol ediyoruz.
        ShipItemToSendController alreadyInQueue = PlanetActionController.PAC.ShipsToSend.Find(x => x.UserPlanetShip.ShipId == this.UserPlanetShip.ShipId);

        // Eğer daha önce bu gemiden eklenmemiş ise gemiyi ekliyoruz.
        if (alreadyInQueue == null)
        {
            // Gemiyi oluşturuyoruz.
            GameObject newShipItemToSend = Instantiate(PlanetActionController.PAC.UsedShipPref, PlanetActionController.PAC.UsedShipContent.content);

            // Gemi bilgisini yüklüyecepiz.
            alreadyInQueue = newShipItemToSend.GetComponent<ShipItemToSendController>();

            // Oluşturulanı listeye ekliyoruz ki daha sonra güncellemek için kullanalım.
            PlanetActionController.PAC.ShipsToSend.Add(alreadyInQueue);

            // Miktarı güncelliyoruz.
            alreadyInQueue.Quantity = quantity;
        }
        else
            alreadyInQueue.Quantity += quantity;

        // Resim isim gibi bilgileri yüklüyoruz.
        alreadyInQueue.LoadData(this.UserPlanetShip);

        // Verileri yeniliyoruz.
        LoadData(UserPlanetShip);

        // Butonu açıyoruz. En az bir gemi seçili olduğu için.
        if (PlanetActionController.PAC.CurrentFleetType != Assets.Scripts.Enums.FleetTypes.None)
            PlanetActionController.PAC.ContinueButton.interactable = true;

        // Eğer panel açık değil ise açıyoruz.
        if (PlanetActionController.PAC.NoShipSelected.activeSelf)
            PlanetActionController.PAC.NoShipSelected.SetActive(false);

    }

    /// <summary>
    /// Gönderilecek olan gemi miktarı.
    /// </summary>
    public int GetUsedQuantity
    {
        get
        {
            return PlanetActionController.PAC.ShipsToSend.Where(x=> x.UserPlanetShip.ShipId == this.UserPlanetShip.ShipId).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();
        }
    }

    /// <summary>
    /// Gönderilebilir olan gemi miktarı.
    /// </summary>
    public int GetUseableQuantity
    {
        get
        {
            return UserPlanetShip.ShipCount - GetUsedQuantity;
        }
    }

}
