namespace WarOfGalaxiesApi.DTO.Models
{
    public class CordinateDTO
    {
        /// <summary>
        /// Galaksideki sırası.
        /// </summary>
        public int GalaxyIndex { get; set; }

        /// <summary>
        /// Galaksideki güneş sistemi sırası.
        /// </summary>
        public int SolarIndex { get; set; }

        /// <summary>
        /// Gezegennin güneş sistemindeki sırası.
        /// </summary>
        public int OrderIndex { get; set; }

        public CordinateDTO()
        {

        }

        public CordinateDTO(int _galaxyIndex, int _solarIndex, int _orderIndex)
        {
            this.GalaxyIndex = _galaxyIndex;
            this.SolarIndex = _solarIndex;
            this.OrderIndex = _orderIndex;
        }
    }
}
