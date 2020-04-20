using System;
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

    }
}
