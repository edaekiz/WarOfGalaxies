using System;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class CordinateDTO
    {
        /// <summary>
        /// Galaksideki sırası.
        /// </summary>
        public int GalaxyIndex;

        /// <summary>
        /// Galaksideki güneş sistemi sırası.
        /// </summary>
        public int SolarySystemIndex;

        /// <summary>
        /// Gezegennin güneş sistemindeki sırası.
        /// </summary>
        public int SolarSystemOrderIndex;
    }
}
