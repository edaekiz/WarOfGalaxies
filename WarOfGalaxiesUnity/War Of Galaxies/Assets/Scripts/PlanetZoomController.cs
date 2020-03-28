using UnityEngine;

public class PlanetZoomController : MonoBehaviour
{
    public static PlanetZoomController PZC { get; set; }

    public enum ZoomStates { Zooming, Zoomed, ZoomingOut, ZoomedOut };

    [Header("Zoom efektinin hızı.")]
    public float ZoomEffectSpeed;

    [Header("Zoom stateini tutuyoruz.")]
    public ZoomStates ZoomState;

    private Transform _targetPlanet;
    private Camera _camera;

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
        // Main kamerayı tutuyoruz.
        _camera = Camera.main;

        // Başlangıç da state zoomed out.
        ZoomState = ZoomStates.ZoomedOut;
    }

    public void BeginZoom(Transform planet)
    {
        // Eğer zaten zoom yapıyor isek geri dön.
        if (ZoomState == ZoomStates.Zooming)
            return;

        // Zoom yapmaya başlıyoruz.
        ZoomState = ZoomStates.Zooming;

        // Zoom yapılacak gezegeni
        _targetPlanet = planet;
    }

    public void BeginZoomOut(Transform planet)
    {
        // Eğer zaten zoom yapılıyor ise geri dön.
        if (ZoomState == ZoomStates.ZoomingOut)
            return;

        // Statei zoom out olarak ayarlıyoruz.
        ZoomState = ZoomStates.ZoomingOut;

        // Zoom yapılacak gezegen.
        _targetPlanet = planet;
    }

    private void Update()
    {
        // Eğer gezegen yok ise geri dön.
        if (_targetPlanet == null)
            return;

        // Zoom out yapmaya başlıyoruz.
        if (Input.GetKeyDown(KeyCode.Space))
            BeginZoomOut(_targetPlanet);

        // Eğer zoom yapıldıysa gezegeni takip etmeye başlıyoruz.
        if (ZoomState == ZoomStates.Zoomed)
        {
            // Kameranın konumu.
            Vector3 camPosition = _camera.transform.position;

            // Hedef gezegenin konumu.
            Vector3 planetPosition = _targetPlanet.transform.position;

            // Kamera geminin üzerinde duracak.
            planetPosition.y += 250;

            // Oraya yürütüyoruz.
            _camera.transform.position = Vector3.MoveTowards(camPosition, planetPosition, Time.deltaTime * ZoomEffectSpeed * ZoomEffectSpeed);
        }

        // Zooming yapıyorsa hedef pointe doğru gidiyoruz.
        if (ZoomState == ZoomStates.Zooming)
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
            {
                // State zoom yaptığımızı söylüyoruz.
                ZoomState = ZoomStates.Zoomed;

                // Touch sistemi kapatıyoruz ki kaydırmalar yapılmasın.
                GalaxyController.GC.DisableTouchSystem();
            }
        }

        // Gezegene zoom out yaparken kullanıyoruz.
        if (ZoomState == ZoomStates.ZoomingOut)
        {
            // Kameranın konumu.
            Vector3 camPosition = _camera.transform.position;

            // Hedef gezegenin konumu.
            Vector3 planetPosition = new Vector3(_camera.transform.position.x, 1500, _camera.transform.position.z);

            // Oraya yürütüyoruz.
            _camera.transform.position = Vector3.MoveTowards(camPosition, planetPosition, Time.deltaTime * ZoomEffectSpeed * (ZoomEffectSpeed / 2));

            // Eğer yeterince yakın ise büyümeyi durduruyoruz.
            if (Vector3.Distance(_camera.transform.position, planetPosition) <= 1)
            {
                // Statei güncelliyoruzz.
                ZoomState = ZoomStates.ZoomedOut;

                // Touch sistemi açıyorzz ki kaydırmalar yapılsın.
                GalaxyController.GC.EnableTouchSystem();
            }
        }

    }
}
