using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuantityItemPanel : BasePanelController
{
    public class QuantityEventArs : EventArgs
    {
        public int Quantity { get; set; }
    }

    [Header("İtemin resmi")]
    public Image ItemImage;

    [Header("İtemin ismi.")]
    public TMP_Text ItemName;

    [Header("Kullanıcının sahip olduğu miktar.")]
    public TMP_Text ItemMaxQuantity;

    [Header("İtemin miktarı.")]
    public TMP_InputField ItemQuantity;

    // Maksimum miktarı tutuyoruz.
    private int maxQuantity;

    /// <summary>
    /// Panel kapandığında çağrılacak olanlar.
    /// </summary>
    public Action<QuantityEventArs> OnPanelClose;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Fielddaki datanın geçerliliğini kontrol ediyoruz.
        ItemQuantity.onEndEdit.AddListener((string val) =>
        {
            int quantity = 0;
            if (int.TryParse(ItemQuantity.text, out quantity))
            {
                if (quantity > maxQuantity)
                    quantity = maxQuantity;
                else if (quantity < 0)
                    quantity = 0;
            }

            ItemQuantity.text = quantity.ToString();

        });
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    public void OnClickOk()
    {
        // Miktar default değeri 0.
        int quantity = 0;

        // Eğer parse edemezsek sıfır kalacak. Edersek de ettiğimiz değer.
        int.TryParse(ItemQuantity.text, out quantity);

        // EĞer çağrılmak istenen var ise buraya ekliyoruz.
        if (OnPanelClose != null)
            OnPanelClose.Invoke(new QuantityEventArs { Quantity = quantity });

        base.ClosePanel();
    }

    public void LoadData(Sprite _itemImage, string _itemName, int _itemMaxQuantity)
    {
        ItemImage.sprite = _itemImage;
        ItemName.text = _itemName;
        maxQuantity = _itemMaxQuantity;
        ItemMaxQuantity.text = $"({_itemMaxQuantity})";
        ItemQuantity.text = _itemMaxQuantity.ToString();
    }

}
