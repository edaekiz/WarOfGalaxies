using System;
using System.Globalization;
using System.Linq;

namespace Assets.Scripts.Extends
{
    public static class ResourceExtends
    {
        /// <summary>
        /// Verilen kaynağı k / m / M gibi döner.
        /// </summary>
        /// <param name="resource">Kaynak</param>
        /// <returns></returns>
        public static string ConvertResource(double resource)
        {
            if (resource < 1000) return $"{(long)resource}";
            int exp = (int)(Math.Log(resource) / Math.Log(1000));
            return $"{Math.Round(resource / Math.Pow(1000, exp), 1)}{"kmMTPE".ElementAt(exp - 1)}";
        }

        /// <summary>
        /// 1m değerini 1.000.000 formatına çevirir.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static string ConvertToDottedResource(double resource)
        {
            if ((long)resource == 0)
                return "0";
            else
                return ((long)resource).ToString("#,##", new NumberFormatInfo { NumberDecimalSeparator = ",", NumberGroupSeparator = "." });
        }

    }
}
