using UnityEngine;

public class PlanetRotateController : MonoBehaviour
{
    private float rotationSpeed = 240.0f;

    void OnMouseDrag()
    {
        //Dünyayı döndürmek için.
        if (Input.touchCount <= 1)
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Mathf.Deg2Rad;

            transform.Rotate(Vector3.up, -rotX, 0);
            transform.Rotate(Vector3.right, rotY, 0);
        }
    }

    private void OnMouseDown()
    {
        transform.parent.GetComponent<PlanetController>().OnMouseDown();
    }
}
