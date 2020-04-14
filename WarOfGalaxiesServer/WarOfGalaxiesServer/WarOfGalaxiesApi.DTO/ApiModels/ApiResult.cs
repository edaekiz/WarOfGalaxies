using System;

namespace WarOfGalaxiesApi.DTO.ApiModels
{
    [Serializable]
    public class ApiResult
    {
        /// <summary>
        /// Başarılı yada başarısız olma durumu.
        /// </summary>
        public bool IsSuccess;

        /// <summary>
        /// Gönderilecek olan data json formatına otomatik dönüştürülür.
        /// </summary>
        public string Data;

        /// <summary>
        /// Herhangi bir mesaj içeriyor mu?
        /// </summary>
        public string Message;
    }
}
