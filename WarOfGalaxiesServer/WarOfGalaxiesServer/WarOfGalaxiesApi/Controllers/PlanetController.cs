using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public class PlanetController : MainController
    {
        public PlanetController(IUnitOfWork unitOfWork, StaticValues staticValues) : base(unitOfWork, staticValues)
        {
        }
    }
}