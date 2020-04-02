using UnityEngine;

public class ZoomPanController : MonoBehaviour
{
    public static ZoomPanController ZPC { get; set; }

    [Header("Minimum yapılabilecek zoom in.")]
    public float ZoomOutMin;

    [Header("Maksimum yapılabilecek zoom out.")]
    public float ZoomOutMax;

    [HideInInspector]
    [Header("Default zoom rate bu değer olacak.")]
    public float DefaultZoomRate;

    [Header("Oyundaki aktif kamera.")]
    public Camera MainCamera;

    [Header("Kameranın sol sınırı.")]
    public float LeftBorder;

    [Header("Kameranın sağ sınırı.")]
    public float RightBorder;

    [Header("Kameranın üst sınırı.")]
    public float TopBorder;

    [Header("Kameranın alt sınırı.")]
    public float BottomBorder;

    private bool isInZoom;
    private Vector3 touchStart;

    private void Awake()
    {
        if (ZPC == null)
            ZPC = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = GetComponent<Camera>();

        // Kameranin orthographic boyutunu tutuyoruz.
        DefaultZoomRate = MainCamera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        // Ekrana dokunduğu andaki konumu alıyoruz.
        if (Input.GetMouseButtonDown(0))
        {
            // Zoom yapmadığından emin olmamız lazım.
            if (!isInZoom)
                touchStart = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        // Zoom işlemi.
        if (Input.touchCount == 2)
        {
            isInZoom = true;
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            float diffrence = currentMagnitude - prevMagnitude;
            DoZoom(diffrence * 0.01f);
        }
        else if (Input.GetMouseButton(0) && !isInZoom) // Eğer zoom yapmıyor ise ekranı hareket ettireceğiz.
        {
            Vector3 direction = touchStart - MainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Hedefe doğru kaydırıyoruz.
            MainCamera.transform.position += direction;
        }

        // Her zaman ekran içerisinde mi diye kontrol ediyoruz.
        CheckCameraBounds();

        // Yalnızca editördeyken mouse tekeri ile zoom yapılabilecek.
        if (Application.isEditor)
            DoZoom(Input.GetAxis("Mouse ScrollWheel"));

        // Dokunan parmak kalmadığında değer zoom modunu kapatıyoruz.
        if (Input.touchCount == 0)
            isInZoom = false;

    }

    void CheckCameraBounds()
    {
        // Ekran genişliğini hesaplıyoruz.
        Vector2 viewSize = new Vector2(MainCamera.orthographicSize * MainCamera.aspect, MainCamera.orthographicSize);

        // X ekseninin sınırlarını kontrol ediyoruz.
        float x = Mathf.Clamp(MainCamera.transform.position.x, LeftBorder + viewSize.x, RightBorder - viewSize.x);

        // Y ekseninin sınırlarını kontrol ediyoruz.
        float y = Mathf.Clamp(MainCamera.transform.position.y, BottomBorder + viewSize.y, TopBorder - viewSize.y);

        // Konumunu güncelliyoruz.
        MainCamera.transform.position = new Vector3(x, y, 0);
    }

    void DoZoom(float increment)
    {
        MainCamera.orthographicSize = Mathf.Clamp(MainCamera.orthographicSize - increment, ZoomOutMin, ZoomOutMax);
    }

}
