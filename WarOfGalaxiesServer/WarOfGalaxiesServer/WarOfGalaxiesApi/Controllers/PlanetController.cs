using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;

namespace WarOfGalaxiesApi.Controllers
{
    public class PlanetController : MainController
    {
        public PlanetController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
    }
}