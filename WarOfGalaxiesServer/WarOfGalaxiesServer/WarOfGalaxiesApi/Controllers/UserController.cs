using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;
using WarOfGalaxiesApi.DTO.ResponseModels;

namespace WarOfGalaxiesApi.Controllers
{
    public class UserController : MainController
    {
        public UserController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpPost("Login")]
        [Description("Kullanıcının çağıracağı ilk methot login olmak için kullanılacak.")]
        public ApiResult Login()
        {
            // Başarılı sonucunu dönüyoruz.
            return ResponseHelper.GetSuccess(new UserDTO
            {
                UserToken = DBUser.UserToken,
                CreateDate = DBUser.CreateDate,
                GoogleToken = DBUser.GoogleToken,
                IosToken = DBUser.IosToken,
                IsBanned = DBUser.IsBanned,
                UserId = DBUser.UserId,
                UserLanguage = DBUser.UserLanguage,
                Username = DBUser.Username
            });
        }
    }
}