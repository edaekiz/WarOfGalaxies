using UnityEngine;

public class PlanetZoomController : MonoBehaviour
{
    public static PlanetZoomController PZC { get; set; }

    [Header("Zoom efektinin hızı.")]
    public float ZoomEffectSpeed;

    private Transform _targetPlanet;
    private Camera _camera;
    private bool _isZoomingOut;
    private bool _isZooming = false;

    private void Awake()
    {
        if (PZC == null)
            PZC = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _camera = Camera.main;
    }

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
        // Zoom out yapmaya başlıyoruz.
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

        // Gezegene zoom out yaparken kullanıyoruz.
        if (_isZoomingOut)
        {
            // Kameranın konumu.
            Vector3 camPosition = _camera.transform.position;

            // Hedef gezegenin konumu.
            Vector3 planetPosition = new Vector3(_camera.transform.position.x, 1500, _camera.transform.position.z);

            // Oraya yürütüyoruz.
            _camera.transform.position = Vector3.MoveTowards(camPosition, planetPosition, Time.deltaTime * ZoomEffectSpeed * (ZoomEffectSpeed / 2));

            // Eğer yeterince yakın ise büyümeyi durduruyoruz.
            if (Vector3.Distance(_camera.transform.position, planetPosition) <= 1)
                _isZoomingOut = false;
        }

    }
}
