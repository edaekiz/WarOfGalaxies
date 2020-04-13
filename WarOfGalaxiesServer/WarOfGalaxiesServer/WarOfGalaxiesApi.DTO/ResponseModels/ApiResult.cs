namespace WarOfGalaxiesApi.DTO.ResponseModels
{
    public class ApiResult
    {
        /// <summary>
        /// Başarılı yada başarısız olma durumu.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gönderilecek olan data json formatına otomatik dönüştürülür.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Herhangi bir mesaj içeriyor mu?
        /// </summary>
        public string Message { get; set; }
    }
}
