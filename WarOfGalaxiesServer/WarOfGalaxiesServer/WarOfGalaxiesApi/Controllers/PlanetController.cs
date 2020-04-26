using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;

namespace WarOfGalaxiesApi.Controllers
{
    public class PlanetController : MainController
    {
        public PlanetController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}