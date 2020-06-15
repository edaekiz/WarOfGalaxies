using System.Collections;
using TMPro;
using UnityEngine;

public class GalaxyChangePanelController : MonoBehaviour
{
    public static GalaxyChangePanelController GCPC { get; set; }
    private void Awake()
    {
        if (GCPC == null)
            GCPC = this;
        else
            Destroy(gameObject);
    }

    [Header("Aktif galaksi indexinin yazdığı alan.")]
    public TMP_InputField GalaxyIndexField;

    [Header("Aktif olan güneş sisteminin yazdığı alan.")]
    public TMP_InputField SolarIndexField;

    [Header("Main paneli tutuyoruz.")]
    public GameObject MainPanel;

    [Header("Paneli kapatıp açma da kullanılacak.")]
    public TMP_Text ToggleText;

    [Header("Aktif olan galaksi.")]
    public int CurrentGalaxyIndex;

    [Header("Aktif olan güneş sistemi.")]
    public int CurrentSolarIndex;

    IEnumerator Start()
    {
        // Oyun yüklenene kadar bekliyoruz.
        yield return new WaitUntil(() => LoadingController.LC.IsGameLoaded);

        // Kordinat atılana kadar bekliyoruz.
        yield return new WaitUntil(() => GlobalPlanetController.GPC.CurrentPlanetCordinate != null);

        // Gezegenin kordinatına gidiyoruz.
        GoToCordinate(GlobalPlanetController.GPC.CurrentPlanetCordinate.GalaxyIndex, GlobalPlanetController.GPC.CurrentPlanetCordinate.SolarIndex);
    }

    public void GoToCordinate(int galaxyIndex,int solarIndex)
    {
        // Galaksi indexi koyuyoruz.
        GalaxyIndexField.text = galaxyIndex.ToString();

        // Solar indexi koyuyoruz.
        SolarIndexField.text = solarIndex.ToString();

        // Hedef kordinata gider.
        GoToCordinate();
    }

    public void GoToPreviousCordinate()
    {
        // Aktif değeri alıyoruz.
        int currentSolarIndex = int.Parse(SolarIndexField.text);

        // Ve 1 arttırıyoruz.
        currentSolarIndex -= 1;

        SolarIndexField.text = $"{currentSolarIndex}";

        // Kordinata gidiyoruz.
        GoToCordinate();
    }

    public void GoToNextCordinate()
    {
        // Aktif değeri alıyoruz.
        int currentSolarIndex = int.Parse(SolarIndexField.text);

        // Ve 1 arttırıyoruz.
        currentSolarIndex += 1;

        SolarIndexField.text = $"{currentSolarIndex}";

        // Kordinata gidiyoruz.
        GoToCordinate();
    }

    public void GoToCordinate()
    {
        ValidateInputFields();

        // Aktif değeri alıyoruz.
        int currentSolarIndex = int.Parse(SolarIndexField.text);

        int currentGalaxyIndex = int.Parse(GalaxyIndexField.text);

        CurrentSolarIndex = currentSolarIndex;

        CurrentGalaxyIndex = currentGalaxyIndex;

        GalaxyController.GC.LoadSolarSystem(currentGalaxyIndex, currentSolarIndex);
    }

    public void ToggleView()
    {
        if (ToggleText.text == "-")
        {
            MainPanel.SetActive(false);
            ToggleText.text = "+";
        }
        else
        {
            MainPanel.SetActive(true);
            ToggleText.text = "-";
        }
    }

    public void ValidateInputFields()
    {
        string solarIndexText = SolarIndexField.text;

        // Aktif değeri alıyoruz.
        int currentSolarIndex = 1;

        // Eğer metin var ise çeviriyoruz.
        if (!string.IsNullOrWhiteSpace(solarIndexText))
            currentSolarIndex = int.Parse(SolarIndexField.text);

        bool isIncreased = false;
        bool isDecrased = false;

        // Eğer X den büyük olduysa başa dönüyoruz ancak bir sonraki galaksiye geçiyoruz.
        if (currentSolarIndex > DataController.DC.GetParameter(Assets.Scripts.Enums.ParameterTypes.SolarSystemCount).ParameterIntValue)
        {
            currentSolarIndex = 1;
            isIncreased = true;
        }
        else if (currentSolarIndex < 1)
        {
            currentSolarIndex = DataController.DC.GetParameter(Assets.Scripts.Enums.ParameterTypes.SolarSystemCount).ParameterIntValue;
            isDecrased = true;
        }

        // Galaksi değeri string olarak.
        string galaxyIndexText = GalaxyIndexField.text;

        // Galaksi alanı.
        int currentGalaxyIndex = 1;

        // Eğer galaksi değeri boş değil ise galaksi değerini alıyoruz.
        if (!string.IsNullOrWhiteSpace(galaxyIndexText))
            currentGalaxyIndex = int.Parse(GalaxyIndexField.text);

        if (isIncreased)
            currentGalaxyIndex++;
        else if (isDecrased)
            currentGalaxyIndex--;

        // Galaksi indeksinin sınırlarını kontrol ediyoruz.
        if (currentGalaxyIndex > DataController.DC.GetParameter(Assets.Scripts.Enums.ParameterTypes.GalaxyCount).ParameterIntValue)
            currentGalaxyIndex = 1;
        else if (currentGalaxyIndex < 1)
            currentGalaxyIndex = DataController.DC.GetParameter(Assets.Scripts.Enums.ParameterTypes.GalaxyCount).ParameterIntValue;

        // Güneş sistemini değiştiriyoruz.
        SolarIndexField.text = $"{currentSolarIndex}";

        // Galaksi değerini güncelliyoruz.
        GalaxyIndexField.text = $"{currentGalaxyIndex}";
    }

}
