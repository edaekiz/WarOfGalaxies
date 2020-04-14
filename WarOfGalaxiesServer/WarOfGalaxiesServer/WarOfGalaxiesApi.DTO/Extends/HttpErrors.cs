using Newtonsoft.Json;
using System.Text;
using WarOfGalaxiesApi.DTO.Helpers;

namespace WarOfGalaxiesApi.DTO.Extends
{
    public static class HttpErrors
    {

        public static byte[] GetException(string message)
        {
            string error = JsonConvert.SerializeObject(ResponseHelper.GetError("Kullanıcı yasaklandı!"));
            byte[] bytes = Encoding.UTF8.GetBytes(error);
            return bytes;
        }
    }
}
