using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Helpers;
using WarOfGalaxiesApi.DTO.Models;
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public class MailController : MainController
    {
        public MailController(IUnitOfWork unitOfWork, StaticValues staticValues) : base(unitOfWork, staticValues)
        {
        }

        [HttpPost("GetLatestUnReadedMails")]
        [Description("Okunmamış olan maillerin listesi.")]
        public ApiResult GetLatestUnReadedMails(LatestUnReadedUserMailRequestDTO request)
        {
            List<UserMailDTO> userMails = base.UnitOfWork.GetRepository<TblUserMails>()
                .Where(x => x.UserId == DBUser.UserId && x.UserMailId > request.LastUnReadedMailId)
                .Select(x => new UserMailDTO
                {
                    UserMailId = x.UserMailId,
                    IsReaded = x.IsReaded,
                    MailCategoryId = x.MailCategoryId,
                    MailContent = x.MailContent,
                    MailDate = x.MailDate
                })
                .ToList();

            return ResponseHelper.GetSuccess(userMails);
        }

        [HttpPost("SetMailAsRead")]
        [Description("Bir maili okundu olarak işaretler.")]
        public ApiResult SetMailAsRead(MailReadRequestDTO request)
        {
            // Kullanıcının mailini buluyoruz.
            TblUserMails userMail = base.UnitOfWork.GetRepository<TblUserMails>()
                .FirstOrDefault(x => x.UserId == DBUser.UserId && x.UserMailId == request.UserMailId);

            // Eğer mail yok ise hata dön.
            if (userMail == null)
                return ResponseHelper.GetError("Posta bulunamadı!");

            // Okundu olarak ayarlıyoruz.
            userMail.IsReaded = true;

            // Ve kayıt ediyoruz.
            base.UnitOfWork.SaveChanges();

            // Ve başarılı sonucunu dönüyoruz.
            return ResponseHelper.GetSuccess();

        }

        [HttpPost("DeleteMail")]
        [Description("Bir maili okundu olarak işaretler.")]
        public ApiResult DeleteMail(MailDeleteRequestDTO request)
        {
            // Kullanıcının mailini buluyoruz.
            TblUserMails userMail = base.UnitOfWork.GetRepository<TblUserMails>()
                .FirstOrDefault(x => x.UserId == DBUser.UserId && x.UserMailId == request.UserMailId);

            // Eğer mail yok ise hata dön.
            if (userMail == null)
                return ResponseHelper.GetError("Posta bulunamadı!");

            // Okundu olarak ayarlıyoruz.
            base.UnitOfWork.GetRepository<TblUserMails>().Delete(userMail);

            // Ve kayıt ediyoruz.
            base.UnitOfWork.SaveChanges();

            // Ve başarılı sonucunu dönüyoruz.
            return ResponseHelper.GetSuccess();

        }
    }
}
