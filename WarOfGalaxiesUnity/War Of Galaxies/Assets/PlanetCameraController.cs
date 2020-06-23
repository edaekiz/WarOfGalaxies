using UnityEngine;

public class PlanetCameraController : MonoBehaviour
{
    //private Camera camera;

    //public static PlanetCameraController PCC;

    //[HideInInspector]
    //public LeanCameraMove LeanCameraMove;
    //[HideInInspector]
    //public LeanCameraZoom LeanCameraZoom;

    //public float MinimumXPosition;
    //public float MinimumZPosition;
    //public float MaximumXPosition;
    //public float MaximumZPosition;
    //private float mainHeight = 0;
    //private float mainWidth = 0;

    //// Start is called before the first frame update
    //void Start()
    //{

    //    #region Singleton

    //    PCC = this;

    //    #endregion

    //    camera = GetComponent<Camera>();

    //    LeanCameraMove = GetComponent<LeanCameraMove>();

    //    LeanCameraZoom = GetComponent<LeanCameraZoom>();

    //    // Kameranın sahip olduğu yükseklik ve genişlik.
    //    mainHeight = camera.fieldOfView * 2f;
    //    mainWidth = mainHeight * camera.aspect;

    //}

    //private void LateUpdate()
    //{

    //    // Kameranın görüntülediği toplam yükseklik.
    //    float tempHeight = camera.orthographicSize * 2f;

    //    // Kameranın görüntülediği toplam genişlik.
    //    float tempWidth = tempHeight * camera.aspect;

    //    // Kameranın gidebileceği minimum ve maksimum kordinatlar hesaplanır. Bunu hesaplarken kameranın kapsama alanı da hesaplanır.
    //    float tempMinXPosition = MinimumXPosition - ((mainWidth - tempWidth) / 2);
    //    float tempMaxXPosition = MaximumXPosition + ((mainWidth - tempWidth) / 2);
    //    float tempMinZPosition = MinimumZPosition - ((mainHeight - tempHeight) / 2);
    //    float tempMaxZPosition = MaximumZPosition + ((mainHeight - tempHeight) / 2);

    //    // Kameranın pozisyonu.
    //    Vector3 cameraPosition = camera.transform.position;

    //    // Kameranın pozisyonu sınırları kontrol ederken bunu değiştireceğiz.
    //    Vector3 tempPosition = cameraPosition;

    //    if (tempPosition.x < tempMinXPosition)
    //        tempPosition = new Vector3(tempMinXPosition, tempPosition.y, tempPosition.z);
    //    if (tempPosition.x > tempMaxXPosition)
    //        tempPosition = new Vector3(tempMaxXPosition, tempPosition.y, tempPosition.z);
    //    if (tempPosition.z < tempMinZPosition)
    //        tempPosition = new Vector3(tempPosition.x, tempPosition.y, tempMinZPosition);
    //    if (tempPosition.z > tempMaxZPosition)
    //        tempPosition = new Vector3(tempPosition.x, tempPosition.y, tempMaxZPosition);

    //    // Eğer iki değişken birbirine eşit değil ise demekki kameranın konumu değişmesi gerekiyor.
    //    if (cameraPosition != tempPosition)
    //        camera.transform.position = Vector3.Lerp(camera.transform.position, tempPosition, 0.4f);

    //}


    ///// <summary>
    ///// Kamerayı yürütmek için kullanılan componenti açar yada kapar.
    ///// </summary>
    ///// <param name="state">Açmak için true, kapamak için false</param>
    //public void ChangeStateOfLeanCameraMove(bool state) => LeanCameraMove.enabled = state;

}
