using UnityEngine;

public class ZoomPanController : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 1;
    public float zoomOutMax = 8;
    private Camera main;
    private bool isInZoom;
    // Start is called before the first frame update
    void Start()
    {
        main = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Ekrana dokunduğu andaki konumu alıyoruz.
        if (Input.GetMouseButtonDown(0))
        {
            // Zoom yapmadığından emin olmamız lazım.
            if (!isInZoom)
                touchStart = main.ScreenToWorldPoint(Input.mousePosition);
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
            zoom(diffrence * 0.01f);
        }
        else if (Input.GetMouseButton(0) && !isInZoom) // Eğer zoom yapmıyor ise ekranı hareket ettireceğiz.
        {
            Vector3 direction = touchStart - main.ScreenToWorldPoint(Input.mousePosition);
            main.transform.position += direction;
        }

        // Yalnızca editördeyken mouse tekeri ile zoom yapılabilecek.
        if (Application.isEditor)
            zoom(Input.GetAxis("Mouse ScrollWheel"));

        // Dokunan parmak kalmadığında değer zoom modunu kapatıyoruz.
        if (Input.touchCount == 0)
            isInZoom = false;

    }

    void zoom(float increment)
    {
        main.orthographicSize = Mathf.Clamp(main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

}
