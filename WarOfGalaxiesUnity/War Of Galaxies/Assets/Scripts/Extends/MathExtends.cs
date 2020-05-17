using System;

namespace Assets.Scripts.Extends
{
    public static class MathExtends
    {
        /// <summary>
        /// Tam karesini alır.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static double TamKare(double first, double second) => Math.Pow(first, 2) + (2 * first * second) + Math.Pow(second, 2);

    }
}
