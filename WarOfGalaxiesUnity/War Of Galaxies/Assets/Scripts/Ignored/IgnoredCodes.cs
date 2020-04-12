namespace Assets.Scripts.Ignored
{
    //public void LoadLines(int[] orders)
    //{
    //    GameObject obj = Instantiate(lineMover, transform.position, Quaternion.identity);
    //    Vector3 p2 = transform.position;

    //    for (int ii = 0; ii < orders.Length; ii++)
    //    {
    //        GameObject line = Instantiate(OrbitLine, lineContent);
    //        LineRenderer lr = line.GetComponent<LineRenderer>();

    //        // Her gezegen arasında 100 birim fark olacak.
    //        float offsetX = GalaxyController.GC.PlanetsDistancePer * orders[ii];

    //        // Gezegenin konumu.
    //        Vector3 p1 = new Vector3(offsetX, 0, 0);

    //        obj.transform.position = p1;

    //        lr.positionCount = OrbitalDetail + 1;

    //        float perDist = (float)360 / OrbitalDetail;

    //        for (int tt = 0; tt <= OrbitalDetail; tt++)
    //        {
    //            obj.transform.RotateAround(transform.position, transform.up, perDist);
    //            var vec = lineContent.InverseTransformPoint(obj.transform.position);
    //            lr.SetPosition(tt, vec);
    //        }
    //    }
    //}

    //public void DisableLines()
    //{
    //    lineContent.gameObject.SetActive(false);
    //}

    //public void EnableLines()
    //{
    //    lineContent.gameObject.SetActive(true);
    //}


    //public void UpdatePlanetPosition()
    //{
    //    // Şuan.
    //    DateTime currentDate = DateTime.UtcNow.AddSeconds((SolarPlanetInfo.UserPlanetID % 5) * 5);

    //    // 1 yılda geçen süreyi hesaplıyoruz.
    //    TimeSpan passedDate = currentDate - new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, currentDate.Minute, 0);

    //    // Geçen süreyi dakika cinsinden tutuyoruz.
    //    double passedMinutes = passedDate.TotalSeconds;

    //    // İlerlemeyi kayıt ediyoruz.
    //    double perFrame = totalAngle - totalAngle * (passedMinutes / _sunRotationTime);

    //    // Eğerki ilerleme - deyse 0 yapıyoruz. Hataları önlemek için.
    //    if (perFrame < 0)
    //        perFrame = 0;

    //    // Başlangıç noktasına koyuyoruz.
    //    transform.localPosition = SpawnPosition;

    //    // Açısını sıfırlıyoruz.
    //    transform.localRotation = Quaternion.identity;

    //    // Kendi etrafında döndürüyoruz.
    //    transform.RotateAround(transform.position, transform.up, Time.time * 20);

    //    // Güneşin etrafında dönüyoruz.
    //    transform.RotateAround(Sun.transform.position, transform.up, (float)perFrame);
    //}

}
