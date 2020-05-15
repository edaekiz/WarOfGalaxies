using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Extends;
using WarOfGalaxiesApi.DTO.Helpers;

namespace WarOfGalaxiesApi.Controllers.Base
{
    public class MainController : Controller
    {
        // İstek tarihi.
        public DateTime RequestDate;

        // Form da gelen token keywordü.
        public const string TOKEN_KEY = "TOKEN";

        // İsteği yapan kullanıcı.
        public TblUsers DBUser;

        // Kullanıcıya özel veritabanı yöneticisi.
        public IUnitOfWork UnitOfWork;

        public MainController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
            RequestDate = DateTime.UtcNow;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            #region Yetkilendirme

            StringValues token = string.Empty;

#if DEBUG
            token = "D26FC6FE-F9A7-4FA9-97EE-22EA219CC5F2";
#else
                token = this.HttpContext.Request.Form[TOKEN_KEY];
#endif

            // Eğer token yok ise geri dön.
            if (token.Count == 0)
            {
                context.Result = base.Content(ResponseHelper.GetError("Token bulunamadı!").ToJson());
                return;
            }

            // Kullanıcıyı buluyoruz.
            DBUser = this.UnitOfWork.GetRepository<TblUsers>().Where(x => x.UserToken == Guid.Parse(token)).FirstOrDefault();

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