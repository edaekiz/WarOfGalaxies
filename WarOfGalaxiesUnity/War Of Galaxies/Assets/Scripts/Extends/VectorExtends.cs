using UnityEngine;
namespace Assets.Scripts.Extends
{
    public class VectorExtends
    {
        /// <summary>
        /// CosA değerini döner.
        /// </summary>
        /// <param name="a">A Konumu.</param>
        /// <param name="b">B Konumu.</param>
        /// <param name="c">C Konumu.</param>
        /// <returns></returns>
        public static float Angle(Vector3 a, Vector3 b, Vector3 c)
        {
            var aa = (c - b).magnitude;
            var bb = (a - c).magnitude;
            var cc = (a - b).magnitude;

            float angle = (Mathf.Pow(bb, 2) + Mathf.Pow(cc, 2) - Mathf.Pow(aa, 2)) / (2 * bb * cc);

            return angle;

        }
    }
}