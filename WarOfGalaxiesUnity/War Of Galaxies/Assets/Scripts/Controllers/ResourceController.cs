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

    [Header("Kaynağa tıklandığında açılacak detay paneli.")]
    public ResourceDetailController MetalDetailPanel;

    [Header("Kaynaklar yükseltilmeye hazır mı?")]
    public bool IsResourceReadyToExecute;

    IEnumerator Start()
    {

        // Kullanıcının gezegenleri yüklenene kadar bekliyoruz.
        yield return new WaitUntil(() => GlobalPlanetController.GPC.CurrentPlanet != null);

        // Hesaplamalara başlıyoruz.
        StartCoroutine(UpdateResources());

    }

    private void Update()
    {
        // Eğer gezegen seçili değil ise geri dön.
        if (GlobalPlanetController.GPC.CurrentPlanet == null)
            return;

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

        // Kaynakları tekrar hesaplıyoruz.
        GlobalPlanetController.GPC.CurrentPlanet.VerifyResources();

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
        if (metalAnimQuantity == GlobalPlanetController.GPC.CurrentPlanet.Metal)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        long multiply = metalAnimQuantity > GlobalPlanetController.GPC.CurrentPlanet.Metal ? -1 : 1;

        // Artış oranını hespalıyoruz.
        long rate;

        // Eğer azalıyor ise yüksek olan değerle - yönünde çarpıyoruz.
        if (multiply == -1)
            rate = (((metalAnimQuantity * 2) / 1000) + 1) * -QuantityAnimationIncreaseSpeed;
        else
            rate = ((GlobalPlanetController.GPC.CurrentPlanet.Metal / 1000) + 1) * QuantityAnimationIncreaseSpeed;

        // Ve arttırma yada azaltmayı yapıyoruz.
        metalAnimQuantity += rate;

        // Eğer azalıyor ise.
        if (multiply == -1)
        {
            // Aradaki farkı alıyoruz.
            long quantity = metalAnimQuantity - GlobalPlanetController.GPC.CurrentPlanet.Metal;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                metalAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Metal;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            long quantity = GlobalPlanetController.GPC.CurrentPlanet.Metal - metalAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                metalAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Metal;
        }

        // Metal miktarını güncelliyoruz.
        MetalQuantityText.text = metalAnimQuantity.ToString();
    }

    #endregion

    #region Crystal ve Animasyon

    private long crystalAnimQuantity;

    private void DoCrystalAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if (crystalAnimQuantity == GlobalPlanetController.GPC.CurrentPlanet.Crystal)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        int multiply = crystalAnimQuantity > GlobalPlanetController.GPC.CurrentPlanet.Crystal ? -1 : 1;

        // Artış oranını hespalıyoruz.
        long rate;

        // Eğer azalıyor ise yüksek olan değerle - yönünde çarpıyoruz.
        if (multiply == -1)
            rate = (((crystalAnimQuantity * 2) / 1000) + 1) * -QuantityAnimationIncreaseSpeed;
        else
            rate = ((GlobalPlanetController.GPC.CurrentPlanet.Crystal / 1000) + 1) * QuantityAnimationIncreaseSpeed;

        // Ve arttırma yada azaltmayı yapıyoruz.
        crystalAnimQuantity += rate;

        // Eğer azalıyor ise.
        if (multiply == -1)
        {
            // Aradaki farkı alıyoruz.
            long quantity = crystalAnimQuantity - GlobalPlanetController.GPC.CurrentPlanet.Crystal;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                crystalAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Crystal;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            long quantity = GlobalPlanetController.GPC.CurrentPlanet.Crystal - crystalAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                crystalAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Crystal;
        }

        // Metal miktarını güncelliyoruz.
        CrystalQuantityText.text = crystalAnimQuantity.ToString();
    }

    #endregion

    #region Bor ve Animasyon

    private long boronAnimQuantity;

    private void DoBoronAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if (boronAnimQuantity == GlobalPlanetController.GPC.CurrentPlanet.Boron)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        long multiply = boronAnimQuantity > GlobalPlanetController.GPC.CurrentPlanet.Boron ? -1 : 1;

        // Artış oranını hespalıyoruz.
        long rate;

        // Eğer azalıyor ise yüksek olan değerle - yönünde çarpıyoruz.
        if (multiply == -1)
            rate = (((boronAnimQuantity * 2) / 1000) + 1) * -QuantityAnimationIncreaseSpeed;
        else
            rate = ((GlobalPlanetController.GPC.CurrentPlanet.Boron / 1000) + 1) * QuantityAnimationIncreaseSpeed;

        // Ve arttırma yada azaltmayı yapıyoruz.
        boronAnimQuantity += rate;

        // Eğer azalıyor ise.
        if (multiply == -1)
        {
            // Aradaki farkı alıyoruz.
            long quantity = boronAnimQuantity - GlobalPlanetController.GPC.CurrentPlanet.Boron;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                boronAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Boron;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            long quantity = GlobalPlanetController.GPC.CurrentPlanet.Boron - boronAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                boronAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Boron;
        }

        // Metal miktarını güncelliyoruz.
        BoronQuantityText.text = boronAnimQuantity.ToString();
    }

    #endregion

}
