using WarOfGalaxiesApi.DAL.Models;
using static WarOfGalaxiesApi.DTO.Models.SimulationDTO;

namespace WarOfGalaxiesApi.DTO.Models
{
    public class SimulationDTO
    {
        public enum SimulationSides
        {
            Attacker,
            DefenderShip,
            DefenderDefense
        }
        public bool IsCivil { get; set; }
        public SimulationSides Side { get; set; }
        public int Id { get; set; }
        public int Health { get; set; }
        public TblShips ShipData { get; set; }
        public TblDefenses DefenseData { get; set; }
        public SimulationDTO Target { get; set; }
    }

    public class SimulationRepairDTO
    {
        public SimulationSides Side { get; set; }
        public int Id { get; set; }
        public int LostCount { get; set; }
        public int RepairCount { get; set; }
    }
}
