using Assets.Scripts.Models;
using System.Collections;
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
    public long MetalQuantity;

    [Header("Kullanıcının kristal miktarı.")]
    public long CrystalQuantity;

    [Header("Kullanıcının boron miktarı.")]
    public long BoronQuantity;

    [Header("Kaynağa tıklandığında açılacak detay paneli.")]
    public ResourceDetailController MetalDetailPanel;

    [Header("Kaynaklar yükseltilmeye hazır mı?")]
    public bool IsResourceReadyToExecute;

    IEnumerator Start()
    {

        // Kullanıcının gezegenleri yüklenene kadar bekliyoruz.
        yield return new WaitUntil(() => GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId > 0);

        // Gezegenin kaynakları yüklendiğinde kaynakları yeniliyoruz.
        SetMetalQuantity(GlobalPlanetController.GPC.CurrentPlanet.Metal);

        // Gezegenin kaynakları yüklendiğinde kaynakları yeniliyoruz.
        SetCrystalQuantity(GlobalPlanetController.GPC.CurrentPlanet.Crystal);

        // Gezegenin kaynakları yüklendiğinde kaynakları yeniliyoruz.
        SetBoronQuantity(GlobalPlanetController.GPC.CurrentPlanet.Boron);

        // Hesaplamalara başlıyoruz.
        StartCoroutine(UpdateResources());

    }

    private void Update()
    {
        // Metal animasyonu.
        DoMetalAnimation();

        // Kristal animasyonu.
        DoCrystalAnimation();

        // Boron animasyonu.
        DoBoronAnimation();
    }

    #region Üretim Hesaplamaları

    public IEnumerator UpdateResources()
    {

        // Her saniye tekrar çağıracağız bu methotu. Bu yüzden 1 saniye bekletiyoruz.
        yield return new WaitForSecondsRealtime(1);

        // Tekrar kendisin çağırıyoruz.
        StartCoroutine(UpdateResources());

    }

    #endregion

    #region Metal ve Animasyon

    private long metalAnimQuantity;

    private void DoMetalAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if (metalAnimQuantity == MetalQuantity)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        long multiply = metalAnimQuantity > MetalQuantity ? -1 : 1;

        // Artış oranını hespalıyoruz.
        long rate;

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
            long quantity = metalAnimQuantity - MetalQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                metalAnimQuantity = MetalQuantity;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            long quantity = MetalQuantity - metalAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                metalAnimQuantity = MetalQuantity;
        }

        // Metal miktarını güncelliyoruz.
        MetalQuantityText.text = metalAnimQuantity.ToString();
    }

    public void SetMetalQuantity(long quantity) => MetalQuantity += quantity;

    #endregion

    #region Crystal ve Animasyon

    private long crystalAnimQuantity;

    private void DoCrystalAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if (crystalAnimQuantity == CrystalQuantity)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        int multiply = crystalAnimQuantity > CrystalQuantity ? -1 : 1;

        // Artış oranını hespalıyoruz.
        long rate;

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
            long quantity = crystalAnimQuantity - CrystalQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                crystalAnimQuantity = CrystalQuantity;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            long quantity = CrystalQuantity - crystalAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                crystalAnimQuantity = CrystalQuantity;
        }

        // Metal miktarını güncelliyoruz.
        CrystalQuantityText.text = crystalAnimQuantity.ToString();
    }

    public void SetCrystalQuantity(long quantity) => CrystalQuantity += quantity;

    #endregion

    #region Bor ve Animasyon

    private long boronAnimQuantity;

    private void DoBoronAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if (boronAnimQuantity == BoronQuantity)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        long multiply = boronAnimQuantity > BoronQuantity ? -1 : 1;

        // Artış oranını hespalıyoruz.
        long rate;

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
            long quantity = boronAnimQuantity - BoronQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                boronAnimQuantity = BoronQuantity;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            long quantity = BoronQuantity - boronAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                boronAnimQuantity = BoronQuantity;
        }

        // Metal miktarını güncelliyoruz.
        BoronQuantityText.text = boronAnimQuantity.ToString();
    }

    public void SetBoronQuantity(long quantity) => BoronQuantity += quantity;

    #endregion

}
