using UnityEngine;

public class PlanetRotater : MonoBehaviour
{
    public float RotateSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, transform.up, RotateSpeed * Time.deltaTime);
    }

    private void OnMouseDown()
    {
        ZoomInPlanet.ZIP.BeginZoom(this.transform);
    }

}
