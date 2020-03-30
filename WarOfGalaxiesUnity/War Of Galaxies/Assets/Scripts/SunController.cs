using UnityEngine;

public class SunController : MonoBehaviour
{
    public float RotateSpeed;
    public int OrbitalDetail;
    public GameObject OrbitLine;
    public Transform lineContent;
    public GameObject lineMover;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Kendi etrafında döndürüyoruz.
        transform.RotateAround(transform.position, transform.up, RotateSpeed * Time.deltaTime);
    }

    public void LoadLines(int[] orders)
    {
        GameObject obj = Instantiate(lineMover, transform.position, Quaternion.identity);
        Vector3 p2 = transform.position;

        for (int ii = 0; ii < orders.Length; ii++)
        {
            GameObject line = Instantiate(OrbitLine, lineContent);
            LineRenderer lr = line.GetComponent<LineRenderer>();

            // Her gezegen arasında 100 birim fark olacak.
            float offsetX = GalaxyController.GC.PlanetsDistancePer * orders[ii];

            // Gezegenin konumu.
            Vector3 p1 = new Vector3(offsetX, 0, 0);

            obj.transform.position = p1;

            lr.positionCount = OrbitalDetail + 1;

            float perDist = (float)360 / OrbitalDetail;

            for (int tt = 0; tt <= OrbitalDetail; tt++)
            {
                obj.transform.RotateAround(transform.position,transform.up,perDist);
                var vec = lineContent.InverseTransformPoint(obj.transform.position);
                lr.SetPosition(tt, vec);
            }
        }
    }

}
