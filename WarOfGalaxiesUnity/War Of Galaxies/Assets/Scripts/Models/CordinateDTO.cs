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
        public int SolarIndex;

        /// <summary>
        /// Gezegennin güneş sistemindeki sırası.
        /// </summary>
        public int OrderIndex;

        public CordinateDTO()
        {

        }

        public CordinateDTO(int _galaxyIndex,int _solarIndex,int _orderIndex)
        {
            this.GalaxyIndex = _galaxyIndex;
            this.SolarIndex = _solarIndex;
            this.OrderIndex = _orderIndex;
        }

    }
}
