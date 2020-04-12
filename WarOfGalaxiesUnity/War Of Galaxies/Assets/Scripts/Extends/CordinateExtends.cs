using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Extends
{
    public static class CordinateExtends
    {
        public static CordinateDTO ToCordinate(this string source)
        {
            string[] strs = source.Split(':');

            // Kordinat beklediğimiz formatta değil ise hata dön.
            if (strs.Length != 3)
            {
                // Hatayı basıyoruz ekrana.
                Debug.LogError("Kordinat parse edilemedi.");

                // Kordinatı boş dönüyoruz.
                return null;
            }

            // Kordinatı oluşturuyoruz.
            return new CordinateDTO
            {
                GalaxyIndex = int.Parse(strs[0]),
                SolarSystemOrderIndex = int.Parse(strs[2]),
                SolarySystemIndex = int.Parse(strs[1])
            };
        }

        public static string ToString(CordinateDTO cordinate)
        {
            return $"{cordinate.GalaxyIndex}:{cordinate.SolarySystemIndex}:{cordinate.SolarSystemOrderIndex}";
        }

    }
}
