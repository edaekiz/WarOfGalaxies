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
    public ZoomStates ZoomState = ZoomStates.ZoomedOut;

    [Header("Seçili olan gezegen.")]
    public PlanetController SelectedPlanet;


    private float zoomRate = 0;
    private float zoomMoveRate = 0;

    private void Awake()
    {
        if (PZC == null)
            PZC = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Kontrol eder seçili olan gezegen bu gezegen mi diye.
    /// </summary>
    /// <param name="planet"></param>
    /// <returns></returns>
    public bool IsPlanetSelected(PlanetController planet)
    {
        return ReferenceEquals(SelectedPlanet, planet);
    }

    /// <summary>
    /// Zoom yapmaya başlar.
    /// </summary>
    /// <param name="planet"></param>
    public void BeginZoom(PlanetController planet)
    {
        // Eğer zaten zoom yapıyor isek geri dön.
        if (ZoomState == ZoomStates.Zooming)
            return;

        // Zoom yapmaya başlıyoruz.
        ZoomState = ZoomStates.Zooming;

        // Zoom yapılacak gezegeni
        SelectedPlanet = planet;

        // Zoom yapma oranı.
        zoomRate = (ZoomPanController.ZPC.MainCamera.orthographicSize / 1) * Time.deltaTime;

        // Zoom yaparken ki hızı.
        zoomMoveRate = Time.deltaTime * ZoomMoveSpeed;

        // Planetin altına atıyoruz.
        ZoomPanController.ZPC.MainCamera.transform.SetParent(SelectedPlanet.transform);

        // Touch sistemi kapatıyoruz ki kaydırmalar yapılmasın.
        GalaxyController.GC.DisableTouchSystem();
    }

    /// <summary>
    /// Zoom out yapmaya başlar.
    /// </summary>
    public void BeginZoomOut()
    {
        // Eğer zaten zoom yapılıyor ise geri dön.
        if (ZoomState == ZoomStates.ZoomingOut)
            return;

        // Statei zoom out olarak ayarlıyoruz.
        ZoomState = ZoomStates.ZoomingOut;

        // Planetin altından alıyoruz.
        ZoomPanController.ZPC.MainCamera.transform.SetParent(null);

        // Bütün gezegenleri açıyoruz.
        SelectedPlanet.Sun.EnableAllPlanets();

        // Zoom yapılacak gezegeni iptal ediyoruz.
        SelectedPlanet = null;
    }

    private void LateUpdate()
    {
        #region Zoom out yapma işleri

        // Gezegene zoom out yaparken kullanıyoruz.
        if (ZoomState == ZoomStates.ZoomingOut)
        {
            // Uzaklaşmaya devam et.
            if (ZoomPanController.ZPC.MainCamera.orthographicSize < ZoomPanController.ZPC.DefaultZoomRate)
                ZoomPanController.ZPC.MainCamera.orthographicSize += zoomRate;

            // Eğer yeterince yakın ise büyümeyi durduruyoruz.
            if (ZoomPanController.ZPC.MainCamera.orthographicSize >= ZoomPanController.ZPC.DefaultZoomRate)
            {
                // Size sabitleniyor.
                ZoomPanController.ZPC.MainCamera.orthographicSize = ZoomPanController.ZPC.DefaultZoomRate;


                // Statei güncelliyoruzz.
                ZoomState = ZoomStates.ZoomedOut;

                // Touch sistemi açıyorzz ki kaydırmalar yapılsın.
                GalaxyController.GC.EnableTouchSystem();
            }
        }

        #endregion

        #region Klavyeden Zoom out yapmak

        // Zoom out yapmaya başlıyoruz.
        if (Input.GetKeyDown(KeyCode.Space) && ZoomState == ZoomStates.Zoomed)
            BeginZoomOut();

        #endregion

        #region Zoom Yapma işleri

        // Zooming yapıyorsa hedef pointe doğru gidiyoruz.
        if (ZoomState == ZoomStates.Zooming)
        {
            // Hedef konum ile arasındaki mesafe.
            float distance = Vector3.Distance(ZoomPanController.ZPC.MainCamera.transform.localPosition, Vector3.zero);

            // Küçültmeye devam et.
            if (ZoomPanController.ZPC.MainCamera.orthographicSize > ZoomPanController.ZPC.ZoomOutMin)
                ZoomPanController.ZPC.MainCamera.orthographicSize -= zoomRate;

            // Konuma doğru yürütüyoruz kamerayı.
            if (distance > .1f)
                ZoomPanController.ZPC.MainCamera.transform.localPosition = Vector3.MoveTowards(ZoomPanController.ZPC.MainCamera.transform.localPosition, Vector3.zero, zoomMoveRate);

            // Hedef konum ile arasındaki mesafe.
            distance = Vector3.Distance(ZoomPanController.ZPC.MainCamera.transform.localPosition, Vector3.zero);

            // Eğer yeterince yakın ise büyümeyi durduruyoruz.
            if (distance <= .1f && ZoomPanController.ZPC.MainCamera.orthographicSize <= ZoomPanController.ZPC.ZoomOutMin)
            {
                // Size sabitleniyor.
                ZoomPanController.ZPC.MainCamera.orthographicSize = ZoomPanController.ZPC.ZoomOutMin;

                // State zoom yaptığımızı söylüyoruz.
                ZoomState = ZoomStates.Zoomed;

                // Diğer gezegenleri kapatıyoruz.
                SelectedPlanet.Sun.DisableNotSelectedPlanets(SelectedPlanet);
            }
        }

        #endregion

    }
}
