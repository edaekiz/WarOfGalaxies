using Assets.Scripts.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.ApiModels
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

        /// <summary>
        /// Eğer data tek bir data ise GetData kullanılır.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>() where T : class
        {
            try
            {
                return JsonUtility.FromJson<T>(Data);
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Eğer beklenen data liste ise ToList kullanılır.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetDataList<T>() where T : class
        {
            return JsonHelper.getJsonArray<T>(Data).ToList();
        }
    }
}
