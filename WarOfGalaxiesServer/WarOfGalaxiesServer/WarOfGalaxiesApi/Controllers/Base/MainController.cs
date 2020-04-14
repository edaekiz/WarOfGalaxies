using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Extends;
using WarOfGalaxiesApi.DTO.Helpers;

namespace WarOfGalaxiesApi.Controllers.Base
{
    public class MainController : Controller
    {
        public const string TOKEN_KEY = "TOKEN";

        protected TblUsers DBUser;

        protected IUnitOfWork _uow;

        public MainController(IUnitOfWork unitOfWork)
        {
            this._uow = unitOfWork;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            #region Yetkilendirme

            StringValues token = this.HttpContext.Request.Form[TOKEN_KEY];

            // Eğer token yok ise geri dön.
            if (token.Count == 0)
            {
                context.Result = base.Content(ResponseHelper.GetError("Token bulunamadı!").ToJson());
                return;
            }

            // Kullanıcıyı buluyoruz.
            DBUser = this._uow.GetRepository<TblUsers>().FirstOrDefault(x => x.UserToken == Guid.Parse(token));

            // Kullanıcı yok ise hata dön.
            if (DBUser == null)
            {
                context.Result = base.Content(ResponseHelper.GetError("Kullanıcı bulunamadı!").ToJson());
                return;
            }

            // Eğer kullanıcı banlandı ise geri dön.
            if (DBUser.IsBanned)
            {
                context.Result = base.Content(ResponseHelper.GetError("Kullanıcı yasaklandı!").ToJson());
                return;
            }

            // Methotu çağırıyoruz.
            base.OnActionExecuting(context);

            #endregion
        }
    }
}