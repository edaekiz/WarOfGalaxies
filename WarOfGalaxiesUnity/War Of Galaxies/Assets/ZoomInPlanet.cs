using System.Collections;
using UnityEngine;

public class ZoomInPlanet : MonoBehaviour
{
    public static ZoomInPlanet ZIP { get; set; }

    [Header("Zoom efektinin hızı.")]
    public float ZoomEffectSpeed;

    private Transform _targetPlanet;
    private Camera _camera;
    private bool _isZoomingOut;
    private Quaternion _zoomBeginRotation;

    private void Awake()
    {
        if (ZIP == null)
            ZIP = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _camera = Camera.main;
    }

    private bool _isZooming = false;

    public void BeginZoom(Transform planet)
    {
        // Eğer zaten zoom yapıyor isek geri dön.
        if (_isZooming)
            return;

        // Zoom yapmaya başlıyoruz.
        _isZooming = true;

        // Zoom yapılacak gezegeni
        _targetPlanet = planet;
    }


    public void BeginZoomOut(Transform planet)
    {
        // Eğer zaten zoom yapılıyor ise geri dön.
        if (_isZoomingOut)
            return;

        // Zoom yapılacak gezegen.
        _targetPlanet = planet;

        // ZOom out yapıyoruz.
        _isZoomingOut = true;
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            BeginZoomOut(_targetPlanet);

        // Zooming yapıyorsa hedef pointe doğru gidiyoruz.
        if (_isZooming)
        {
            // Kameranın konumu.
            Vector3 camPosition = _camera.transform.position;

            // Hedef gezegenin konumu.
            Vector3 planetPosition = _targetPlanet.transform.position;

            // Kamera geminin üzerinde duracak.
            planetPosition.y += 250;

            // Oraya yürütüyoruz.
            _camera.transform.position = Vector3.MoveTowards(camPosition, planetPosition, Time.deltaTime * ZoomEffectSpeed * ZoomEffectSpeed);

            // Eğer yeterince yakın ise büyümeyi durduruyoruz.
            if (Vector3.Distance(_camera.transform.position, planetPosition) <= 1)
                _isZooming = false;
        }
    }
}
