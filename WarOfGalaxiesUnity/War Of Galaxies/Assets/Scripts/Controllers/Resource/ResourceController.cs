using Assets.Scripts.ApiModels;
using Assets.Scripts.Controllers.Base;
using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System.Collections;
using TMPro;
using UnityEngine;

public class ResourceController : BaseLanguageBehaviour
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
    public TMP_Text MetalQuantityText;

    [Header("Kullanıcının kristal miktarının basılacağı alan.")]
    public TMP_Text CrystalQuantityText;

    [Header("Kullanıcının boron miktarının basılacağı alan.")]
    public TMP_Text BoronQuantityText;

    [Header("Metal kaynağına tıklandığında detayları yazacak.")]
    public ResourceDetailController MetalResourceDetail;

    [Header("Kristal kaynağına tıklandığında detayları yazacak.")]
    public ResourceDetailController CrystalResourceDetail;

    [Header("Boron kaynağına tıklandığında detayları yazacak.")]
    public ResourceDetailController BoronResourceDetail;

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

        // Metal detaylarını tazele.
        RefreshMetalDetails();

        // Kristal detaylarını tazele.
        RefreshCrystalDetails();

        // Boron detaylarını tazele.
        RefreshBoronDetails();
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
        double buildingCapacity = StaticData.GetBuildingStorage(Assets.Scripts.Enums.Buildings.KristalDeposu, buildingLevel);

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
        double buildingCapacity = StaticData.GetBuildingStorage(Buildings.BoronDeposu, buildingLevel);

        // Eğer deposu yeterli ise beyaz değil ise kırmızı olacak.
        if (boronAnimQuantity < buildingCapacity)
            BoronQuantityText.color = Color.white;
        else
            BoronQuantityText.color = Color.red;
    }

    #endregion

    public void RefreshMetalDetails()
    {
        // Eğer panel açık değil ise geri dön.
        if (!MetalResourceDetail.ResourceDetailPanel.activeSelf)
            return;

        #region Metal Üretim hesaplaması.

        // Kullanıcının metal binası var mı?
        UserPlanetBuildingDTO metalBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.MetalMadeni);

        // Saatlik hesaplanmış üretim.
        double metalProducePerHour = StaticData.GetBuildingProdPerHour(Buildings.MetalMadeni, metalBuilding == null ? 0 : metalBuilding.BuildingLevel);

        #endregion

        #region Depo Kapasitesini hesaplıyoruz.

        // Kullanıcının metal binası.
        UserPlanetBuildingDTO metalStorageBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.MetalDeposu);

        // Kullanıcının metal deposu.
        double metalBuildingCapacity = StaticData.GetBuildingStorage(Buildings.MetalDeposu, metalStorageBuilding == null ? 0 : metalStorageBuilding.BuildingLevel);

        #endregion

        #region Ekrana basıyoruz.

        MetalResourceDetail.ContentField.text = base.GetLanguageText("KaynakDetay", ResourceExtends.ConvertToDottedResource(GlobalPlanetController.GPC.CurrentPlanet.Metal), ResourceExtends.ConvertToDottedResource(metalBuildingCapacity), ResourceExtends.ConvertToDottedResource(metalProducePerHour));

        #endregion

    }

    public void RefreshCrystalDetails()
    {
        // Eğer panel açık değil ise geri dön.
        if (!CrystalResourceDetail.ResourceDetailPanel.activeSelf)
            return;

        #region Kristal Üretim hesaplaması.

        // Kullanıcının Kristal binası var mı?
        UserPlanetBuildingDTO crystalBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.KristalMadeni);

        // Saatlik hesaplanmış üretim.
        double crystalProducePerHour = StaticData.GetBuildingProdPerHour(Buildings.KristalMadeni, crystalBuilding == null ? 0 : crystalBuilding.BuildingLevel);

        #endregion

        #region Depo Kapasitesini hesaplıyoruz.

        // Kullanıcının metal binası.
        UserPlanetBuildingDTO crystalStorageBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.KristalDeposu);

        // Kullanıcının kristal deposu.
        double crystalBuildingCapacity = StaticData.GetBuildingStorage(Buildings.KristalDeposu, crystalStorageBuilding == null ? 0 : crystalStorageBuilding.BuildingLevel);

        #endregion

        #region Ekrana basıyoruz.

        CrystalResourceDetail.ContentField.text = base.GetLanguageText("KaynakDetay", ResourceExtends.ConvertToDottedResource(GlobalPlanetController.GPC.CurrentPlanet.Crystal), ResourceExtends.ConvertToDottedResource(crystalBuildingCapacity), ResourceExtends.ConvertToDottedResource(crystalProducePerHour));

        #endregion
    }

    public void RefreshBoronDetails()
    {
        // Eğer panel açık değil ise geri dön.
        if (!BoronResourceDetail.ResourceDetailPanel.activeSelf)
            return;

        #region Boron Üretim hesaplaması.

        // Kullanıcının Boron binası var mı?
        UserPlanetBuildingDTO boronBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.BoronMadeni);

        // Saatlik hesaplanmış üretim.
        double boronProducePerHour = StaticData.GetBuildingProdPerHour(Buildings.BoronMadeni, boronBuilding == null ? 0 : boronBuilding.BuildingLevel);

        #endregion

        #region Depo Kapasitesini hesaplıyoruz.

        // Kullanıcının metal binası.
        UserPlanetBuildingDTO boronStorageBuilding = LoginController.LC.CurrentUser.UserPlanetsBuildings.Find(x => x.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId && x.BuildingId == Buildings.BoronDeposu);

        // Kullanıcının kristal deposu.
        double boronBuildingCapacity = StaticData.GetBuildingStorage(Buildings.BoronDeposu, boronStorageBuilding == null ? 0 : boronStorageBuilding.BuildingLevel);

        #endregion

        #region Ekrana basıyoruz.

        BoronResourceDetail.ContentField.text = base.GetLanguageText("KaynakDetay", ResourceExtends.ConvertToDottedResource(GlobalPlanetController.GPC.CurrentPlanet.Boron), ResourceExtends.ConvertToDottedResource(boronBuildingCapacity), ResourceExtends.ConvertToDottedResource(boronProducePerHour));

        #endregion
    }

}
