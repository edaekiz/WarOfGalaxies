using Newtonsoft.Json;

namespace WarOfGalaxiesApi.DTO.Extends
{
    public static class ClassExtends
    {
        public static string ToJson(this object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}
