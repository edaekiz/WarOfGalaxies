using Microsoft.AspNetCore.Mvc;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Extends;
using WarOfGalaxiesApi.DTO.Models;
using WarOfGalaxiesApi.DTO.RequestModels;
using WarOfGalaxiesApi.DTO.ResponseModels;

namespace WarOfGalaxiesApi.Controllers
{
    public class UserController : Controller
    {
        private IUnitOfWork uow;
        public UserController(IUnitOfWork unitOfWork)
        {
            uow = unitOfWork;
        }

        [HttpPost("Login")]
        public ApiResult Login(LoginRequestDTO request)
        {
            // Kullanıcıyı buluyoruz.
            TblUsers user = uow.GetRepository<TblUsers>().FirstOrDefault(x => x.UserToken == request.Token);

            // Eğer kullanıcı yok ise geri dön. 
            if (user == null)
                return ResponseHelper.GetError("Kullanıcı bulunamadı!");

            // Kullanıcıyı bulduk şimdi blokeli mi diye kontrol ediyoruz.
            if (user.IsBanned)
                return ResponseHelper.GetError("Kullanıcı banlandı!");

            // Başarılı sonucunu dönüyoruz.
            return ResponseHelper.GetSuccess(new UserDTO
            {
                UserToken = user.UserToken,
                CreateDate = user.CreateDate,
                GoogleToken = user.GoogleToken,
                IosToken = user.IosToken,
                IsBanned = user.IsBanned,
                UserId = user.UserId,
                UserLanguage = user.UserLanguage,
                Username = user.Username
            });

        }
    }
}