using Assets.Scripts.ApiModels;
using Assets.Scripts.Data;
using Assets.Scripts.Extends;
using System.Collections;
using System.Globalization;
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

    private double metalAnimQuantity;

    private void DoMetalAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if ((long)metalAnimQuantity == (long)GlobalPlanetController.GPC.CurrentPlanet.Metal)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        double multiply = metalAnimQuantity > GlobalPlanetController.GPC.CurrentPlanet.Metal ? -1 : 1;

        // Artış oranını hespalıyoruz.
        double rate;

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
            double quantity = metalAnimQuantity - GlobalPlanetController.GPC.CurrentPlanet.Metal;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                metalAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Metal;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            double quantity = GlobalPlanetController.GPC.CurrentPlanet.Metal - metalAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                metalAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Metal;
        }

        // Metal miktarını güncelliyoruz.
        MetalQuantityText.text = ResourceExtends.ConvertToDottedResource(metalAnimQuantity);

        // Metal deposu seviyesi.
        int buildingLevel = 0;

        // MEtal deposunu buluyoruz.
        UserPlanetBuildingDTO storageBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Assets.Scripts.Enums.Buildings.MetalDeposu);

        // Eğer deposu var ise kapasiteyi hesaplıyoruz.
        if (storageBuilding != null)
            buildingLevel = storageBuilding.BuildingLevel;

        // Kapasitesi.
        double buildingCapacity = StaticData.GetBuildingStorage(Assets.Scripts.Enums.Buildings.MetalDeposu, buildingLevel);

        // Eğer deposu yeterli ise beyaz değil ise kırmızı olacak.
        if (metalAnimQuantity < buildingCapacity)
            MetalQuantityText.color = Color.white;
        else
            MetalQuantityText.color = Color.red;
    }

    #endregion

    #region Crystal ve Animasyon

    private double crystalAnimQuantity;

    private void DoCrystalAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if ((long)crystalAnimQuantity == (long)GlobalPlanetController.GPC.CurrentPlanet.Crystal)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        int multiply = crystalAnimQuantity > GlobalPlanetController.GPC.CurrentPlanet.Crystal ? -1 : 1;

        // Artış oranını hespalıyoruz.
        double rate;

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
            double quantity = crystalAnimQuantity - GlobalPlanetController.GPC.CurrentPlanet.Crystal;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                crystalAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Crystal;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            double quantity = GlobalPlanetController.GPC.CurrentPlanet.Crystal - crystalAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                crystalAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Crystal;
        }

        // Kristal miktarını güncelliyoruz.
        CrystalQuantityText.text = ResourceExtends.ConvertToDottedResource(crystalAnimQuantity);

        // Kristal deposu seviyesi.
        int buildingLevel = 0;

        // Kristal deposunu buluyoruz.
        UserPlanetBuildingDTO storageBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Assets.Scripts.Enums.Buildings.KristalDeposu);

        // Eğer deposu var ise kapasiteyi hesaplıyoruz.
        if (storageBuilding != null)
            buildingLevel = storageBuilding.BuildingLevel;

        // Kapasitesi.
        double buildingCapacity = StaticData.GetBuildingStorage(Assets.Scripts.Enums.Buildings.MetalDeposu, buildingLevel);

        // Eğer deposu yeterli ise beyaz değil ise kırmızı olacak.
        if (crystalAnimQuantity < buildingCapacity)
            CrystalQuantityText.color = Color.white;
        else
            CrystalQuantityText.color = Color.red;
    }

    #endregion

    #region Bor ve Animasyon

    private double boronAnimQuantity;

    private void DoBoronAnimation()
    {
        // Eğer metal animasyonu tamamlandıysa geri dön.
        if ((long)boronAnimQuantity == (long)GlobalPlanetController.GPC.CurrentPlanet.Boron)
            return;

        // Azalacaksa -1 olacak artacaksa 1.
        double multiply = boronAnimQuantity > GlobalPlanetController.GPC.CurrentPlanet.Boron ? -1 : 1;

        // Artış oranını hespalıyoruz.
        double rate;

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
            double quantity = boronAnimQuantity - GlobalPlanetController.GPC.CurrentPlanet.Boron;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                boronAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Boron;
        }
        else // Eğer miktar artıyor ise.
        {
            // Aradaki farkı alıyoruz.
            double quantity = GlobalPlanetController.GPC.CurrentPlanet.Boron - boronAnimQuantity;

            // Miktarı kontrol ediyoruz.
            if (quantity <= 0)
                boronAnimQuantity = GlobalPlanetController.GPC.CurrentPlanet.Boron;
        }

        // Boron miktarını güncelliyoruz.
        BoronQuantityText.text = ResourceExtends.ConvertToDottedResource(boronAnimQuantity);

        // Kristal deposu seviyesi.
        int buildingLevel = 0;

        // Boron deposunu buluyoruz.
        UserPlanetBuildingDTO storageBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Assets.Scripts.Enums.Buildings.BoronDeposu);

        // Eğer deposu var ise kapasiteyi hesaplıyoruz.
        if (storageBuilding != null)
            buildingLevel = storageBuilding.BuildingLevel;

        // Kapasitesi.
        double buildingCapacity = StaticData.GetBuildingStorage(Assets.Scripts.Enums.Buildings.BoronDeposu, buildingLevel);

        // Eğer deposu yeterli ise beyaz değil ise kırmızı olacak.
        if (boronAnimQuantity < buildingCapacity)
            BoronQuantityText.color = Color.white;
        else
            BoronQuantityText.color = Color.red;
    }

    #endregion

}
