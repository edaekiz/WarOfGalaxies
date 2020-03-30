using UnityEngine;

public class PlanetZoomController : MonoBehaviour
{
    public static PlanetZoomController PZC { get; set; }

    public enum ZoomStates { Zooming, Zoomed, ZoomingOut, ZoomedOut };

    [Header("Zoom efektinin hareket hızı.")]
    public float ZoomMoveSpeed;

    [Header("Zoom hızı.")]
    public float ZoomSpeed;

    [Header("Zoom stateini tutuyoruz.")]
    public ZoomStates ZoomState;

    private Transform _targetPlanet;
    private Camera _camera;
    private float zoomSpeed = 0;
    private Vector3 beginPosition;

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

        // Hedefe gezegene merkezliyoruz.
        Vector3 destination = _targetPlanet.transform.position;

        destination.y = _camera.transform.position.y;
        beginPosition = destination;
    }

    public void BeginZoomOut(Transform planet)
    {
        // Eğer zaten zoom yapılıyor ise geri dön.
        if (ZoomState == ZoomStates.ZoomingOut)
            return;

        // Statei zoom out olarak ayarlıyoruz.
        ZoomState = ZoomStates.ZoomingOut;

    }

    private void LateUpdate()
    {
        // Eğer gezegen yok ise geri dön.
        if (_targetPlanet == null)
            return;

        // Zoom out yapmaya başlıyoruz.
        if (Input.GetKeyDown(KeyCode.Space))
            BeginZoomOut(_targetPlanet);

        // Zooming yapıyorsa hedef pointe doğru gidiyoruz.
        if (ZoomState == ZoomStates.Zooming)
        {
            // Büyük halde mi.
            bool isGrowed = false;

            // Yeterince büyümediyse büyütüyoruz.
            if (transform.localScale.x < .4f && transform.localScale.y < .4f && transform.localScale.z < .4f)
                transform.localScale += Vector3.one * Time.deltaTime * ZoomSpeed;
            else
                isGrowed = true;

            // Eğer büyüdüyse de state değişiyor.
            if (isGrowed)
            {
                // Hedefe gezegene merkezliyoruz.
                Vector3 destination = _targetPlanet.transform.position;

                destination.y = _camera.transform.position.y;

                // Hedef konum ile arasındaki mesafe.
                float distance = Vector3.Distance(_camera.transform.position, destination);

                // Mesafe uzak ise hedef konuma doğru ilerleyeceğiz.
                if (distance > .1f)
                {
                    // Konuma doğru yürütüyoruz kamerayı.
                    _camera.transform.position = Vector3.MoveTowards(_camera.transform.position, destination, Time.deltaTime * ZoomMoveSpeed);

                    // Hedef konum ile mesafeyi tekrar hesaplıyoruz.
                    distance = Vector3.Distance(_camera.transform.position, destination);
                }

                // Eğer yeterince yakın ise büyümeyi durduruyoruz.
                if (distance <= .1f)
                {
                    // State zoom yaptığımızı söylüyoruz.
                    ZoomState = ZoomStates.Zoomed;

                    // Touch sistemi kapatıyoruz ki kaydırmalar yapılmasın.
                    GalaxyController.GC.DisableTouchSystem();
                }
            }
        }

        // Gezegene zoom out yaparken kullanıyoruz.
        if (ZoomState == ZoomStates.ZoomingOut)
        {
            // Eski haline getiriyoruz.
            Vector3 targetScale = Vector3.one / 5;

            // Büyük halde mi.
            bool isSmalled = false;

            // Yeterince büyümediyse büyütüyoruz.
            if (transform.localScale.x > targetScale.x && transform.localScale.y > targetScale.y && transform.localScale.z > targetScale.z)
                transform.localScale -= Vector3.one * Time.deltaTime * ZoomSpeed;
            else
                isSmalled = true;

            // Hedefe gezegene merkezliyoruz.
            Vector3 destination = _targetPlanet.transform.position;

            // Y ekseni hiç değişmemeli.
            destination.y = _camera.transform.position.y;

            // Konuma doğru yürütüyoruz kamerayı.
            _camera.transform.position = destination;//= Vector3.MoveTowards(_camera.transform.position, destination, Time.deltaTime * (ZoomMoveBaseSpeed / 2));

            // Eğer yeterince yakın ise büyümeyi durduruyoruz.
            if (isSmalled && Vector3.Distance(_camera.transform.position, destination) <= 0.1f)
            {
                // Statei güncelliyoruzz.
                ZoomState = ZoomStates.ZoomedOut;

                // Touch sistemi açıyorzz ki kaydırmalar yapılsın.
                GalaxyController.GC.EnableTouchSystem();
            }
        }
    }
}
