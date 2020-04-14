using UnityEngine;

public class LoadingController : MonoBehaviour
{
    public static LoadingController LC { get; set; }

    private void Awake()
    {
        if (LC == null)
            LC = this;
        else
            Destroy(gameObject);
    }

    [Header("Oyun yüklenirken açılacak yada kapatılacak.")]
    public GameObject LoadingCanvas;

    [Header("Oyun yüklendiğinde doğru değeri dönecek.")]
    public bool IsGameLoaded;

    [Header("Bu oyunun hazır hale gelmesi için gereken yükleme sayısı.")]
    public int TotalLoadItemCount;

    [Header("Oyunun hazır hale gelmesi için anlık yükleme sayısı.")]
    public int CurrentLoadItemCount;

    private void Start()
    {
        // Yükleme panelini açıyoruz.
        LoadingCanvas.SetActive(true);
    }

    /// <summary>
    /// Aktif yükleme sayısını arttırır.
    /// </summary>
    public void IncreaseLoadCount()
    {
        // Anlık yükleme sayısı 1 arttırıldı.
        CurrentLoadItemCount++;

        // Eğer tüm yüklemeler tamamlandıysa yükleniyor paneli kapanıyor.
        if (CurrentLoadItemCount == TotalLoadItemCount)
            LoadingCanvas.SetActive(false);
    }

}
