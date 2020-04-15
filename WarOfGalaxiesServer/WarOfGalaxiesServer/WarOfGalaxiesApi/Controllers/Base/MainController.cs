using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Extends;
using WarOfGalaxiesApi.DTO.Helpers;

namespace WarOfGalaxiesApi.Controllers.Base
{
    public class MainController : Controller
    {
        // Form da gelen token keywordü.
        public const string TOKEN_KEY = "TOKEN";

        // İsteği yapan kullanıcı.
        protected TblUsers DBUser;

        // Kullanıcıya özel veritabanı yöneticisi.
        protected IUnitOfWork UnitOfWork;

        public MainController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
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
            DBUser = this.UnitOfWork.GetRepository<TblUsers>().FirstOrDefault(x => x.UserToken == Guid.Parse(token));

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

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

    }
}