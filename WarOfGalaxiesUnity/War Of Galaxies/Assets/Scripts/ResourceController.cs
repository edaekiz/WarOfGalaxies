using TMPro;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public static ResourceController RC { get; set; }

    private void Awake()
    {
        if (RC == null)
            RC = this;
        else
            Destroy(gameObject);
    }

    [Header("Kaynak kullanıldığında yada alındığında ne kadar hızlı animasyon yapılacak.")]
    [Range(1, 20)]
    public int QuantityAnimationIncreaseSpeed;

    [Header("Kullanıcının metal miktarının basılacağı alan.")]
    public TextMeshProUGUI MetalQuantityText;

    [Header("Kullanıcının kristal miktarının basılacağı alan.")]
    public TextMeshProUGUI CrystalQuantityText;

    [Header("Kullanıcının boron miktarının basılacağı alan.")]
    public TextMeshProUGUI BoronQuantityText;

    [Header("Kullanıcının metal miktarı.")]
    public int MetalQuantity;

    [Header("Kullanıcının kristal miktarı.")]
    public int CrystalQuantity;

    [Header("Kullanıcının boron miktarı.")]
    public int BoronQuantity;

    [Header("Kaynağa tıklandığında açılacak detay paneli.")]
    public ResourceDetailController MetalDetailPanel;


    private void Update()
    {
        // Metal animasyonu.
        DoMetalAnimation();

        // Kristal animasyonu.
        DoCrystalAnimation();

        // Boron animasyonu.
        DoBoronAnimation();
    }

    #region Metal ve Animasyon

    private int metalAnimQuantity;

    private void DoMetalAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if (metalAnimQuantity == MetalQuantity)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        int multiply = metalAnimQuantity > MetalQuantity ? -1 : 1;

        // Artış oranını hespalıyoruz.
        int rate;

        // Eğer azalıyor ise yüksek olan değerle - yönünde çarpıyoruz.
        if (multiply == -1)
            rate = (((metalAnimQuantity * 2) / 1000) + 1) * -QuantityAnimationIncreaseSpeed;
        else
            rate = ((MetalQuantity / 1000) + 1) * QuantityAnimationIncreaseSpeed;

        // Ve arttırma yada azaltmayı yapıyoruz.
        metalAnimQuantity += rate;

        // Eğer azalıyor ise.
        if (multiply == -1)
        {
            // Aradaki farkı alıyoruz.
            int quantity = metalAnimQuantity - MetalQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                metalAnimQuantity = MetalQuantity;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            int quantity = MetalQuantity - metalAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                metalAnimQuantity = MetalQuantity;
        }

        // Metal miktarını güncelliyoruz.
        MetalQuantityText.text = metalAnimQuantity.ToString();
    }

    public void SetMetalQuantity(int quantity) => MetalQuantity += quantity;

    #endregion

    #region Crystal ve Animasyon

    private int crystalAnimQuantity;

    private void DoCrystalAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if (crystalAnimQuantity == CrystalQuantity)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        int multiply = crystalAnimQuantity > CrystalQuantity ? -1 : 1;

        // Artış oranını hespalıyoruz.
        int rate;

        // Eğer azalıyor ise yüksek olan değerle - yönünde çarpıyoruz.
        if (multiply == -1)
            rate = (((crystalAnimQuantity * 2) / 1000) + 1) * -QuantityAnimationIncreaseSpeed;
        else
            rate = ((CrystalQuantity / 1000) + 1) * QuantityAnimationIncreaseSpeed;

        // Ve arttırma yada azaltmayı yapıyoruz.
        crystalAnimQuantity += rate;

        // Eğer azalıyor ise.
        if (multiply == -1)
        {
            // Aradaki farkı alıyoruz.
            int quantity = crystalAnimQuantity - CrystalQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                crystalAnimQuantity = CrystalQuantity;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            int quantity = CrystalQuantity - crystalAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                crystalAnimQuantity = CrystalQuantity;
        }

        // Metal miktarını güncelliyoruz.
        CrystalQuantityText.text = crystalAnimQuantity.ToString();
    }

    public void SetCrystalQuantity(int quantity) => CrystalQuantity += quantity;

    #endregion

    #region Bor ve Animasyon

    private int boronAnimQuantity;

    private void DoBoronAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if (boronAnimQuantity == BoronQuantity)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        int multiply = boronAnimQuantity > BoronQuantity ? -1 : 1;

        // Artış oranını hespalıyoruz.
        int rate;

        // Eğer azalıyor ise yüksek olan değerle - yönünde çarpıyoruz.
        if (multiply == -1)
            rate = (((boronAnimQuantity * 2) / 1000) + 1) * -QuantityAnimationIncreaseSpeed;
        else
            rate = ((BoronQuantity / 1000) + 1) * QuantityAnimationIncreaseSpeed;

        // Ve arttırma yada azaltmayı yapıyoruz.
        boronAnimQuantity += rate;

        // Eğer azalıyor ise.
        if (multiply == -1)
        {
            // Aradaki farkı alıyoruz.
            int quantity = boronAnimQuantity - BoronQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                boronAnimQuantity = BoronQuantity;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            int quantity = BoronQuantity - boronAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                boronAnimQuantity = BoronQuantity;
        }

        // Metal miktarını güncelliyoruz.
        BoronQuantityText.text = boronAnimQuantity.ToString();
    }

    public void SetBoronQuantity(int quantity) => BoronQuantity += quantity;

    #endregion



}
