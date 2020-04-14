using WarOfGalaxiesApi.DTO.Extends;
using WarOfGalaxiesApi.DTO.ResponseModels;

namespace WarOfGalaxiesApi.DTO.Helpers
{
    public static class ResponseHelper
    {
        #region Başarılı Mesajları

        public static ApiResult GetSuccess(object data)
        {
            return new ApiResult
            {
                Data = data.ToJson(),
                IsSuccess = true,
                Message = string.Empty
            };
        }

        public static ApiResult GetSuccess(object data, string message)
        {
            return new ApiResult
            {
                Data = data.ToJson(),
                IsSuccess = true,
                Message = message
            };
        }

        public static ApiResult GetSuccess(string message)
        {
            return new ApiResult
            {
                Data = null,
                IsSuccess = true,
                Message = message
            };
        }

        #endregion

        #region Hata mesajları

        public static ApiResult GetError(object data)
        {
            return new ApiResult
            {
                Data = data.ToJson(),
                IsSuccess = false,
                Message = string.Empty
            };
        }

        public static ApiResult GetError(object data, string message)
        {
            return new ApiResult
            {
                Data = data.ToJson(),
                IsSuccess = false,
                Message = message
            };
        }

        public static ApiResult GetError(string message)
        {
            return new ApiResult
            {
                Data = null,
                IsSuccess = false,
                Message = message
            };
        }

        #endregion

    }
}
