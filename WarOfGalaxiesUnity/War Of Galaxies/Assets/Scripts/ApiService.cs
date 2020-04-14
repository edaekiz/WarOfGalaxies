using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiService : MonoBehaviour
{
    [Serializable]
    public enum Environments { Development, Local, Real };
    public static ApiService API { get; set; }

    private void Awake()
    {
        if (API == null)
            API = this;
        else
            Destroy(gameObject);

        switch (Environment)
        {
            case Environments.Development:
                BaseUrl = "http://localhost:1317";
                break;
            case Environments.Local:
                BaseUrl = "http://3.122.182.106:8080/";
                break;
            case Environments.Real:
                BaseUrl = "http://3.122.182.106:8080/";
                break;
        }
    }

    [Header("Bağlantı yolu.")]
    public Environments Environment;

    [Header("Kullanıcıya ait token.")]
    public string Token;

    // Bağlantı yolu.
    private string BaseUrl;

    /// <summary>
    /// Verilen adrese Post işlemi yapar
    /// ÖRN URL : GetUserBuildings
    /// </summary>
    /// <typeparam name="TResponse">Her yanıt ApiResult nesnesi ile döner. ApiResult içerisindeki data kaydı barındırır.</typeparam>
    /// <param name="url">Çağrılacak methot.</param>
    /// <param name="formData"> Sadece nesneyi göndererek bu model oluşturulabilir. GetFormFromData methodu kullanılarak.</param>
    /// <param name="trigger">Çağırma işlemi gerçekleştiğinde burası çalışacak.</param>
    /// <returns></returns>
    public IEnumerator Post<TResponse>(string url, object data, Action<TResponse> trigger) where TResponse : class
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(BaseUrl + url, GetFormFromData(data, true)))
        {
            // Sunucuya isteği gönderiyoruz.
            yield return webRequest.SendWebRequest();

            // Hata var ise hatayı basacağız.
            if (!string.IsNullOrEmpty(webRequest.error))
            {
                Debug.Log(webRequest.url + "->" + webRequest.error);
                trigger.Invoke(JsonUtility.FromJson<TResponse>("{\"IsSuccess\":false}"));
            }
            else
            {
                // Her sonucu logluyoruz.
                Debug.LogWarning(webRequest.url + "->" + webRequest.downloadHandler.text);
                TResponse response = JsonUtility.FromJson<TResponse>(webRequest.downloadHandler.text);
                trigger.Invoke(response);
            }
        }
    }

    public WWWForm GetFormFromData(object request, bool isTokenActive = true)
    {

        WWWForm form = new WWWForm();

        if (request != null)
        {
            var properties = request.GetType().GetProperties();
            for (int ii = 0; ii < properties.Length; ii++)
            {
                var value = properties[ii].GetValue(request);
                form.AddField(properties[ii].Name, value == null ? "" : value.ToString());
            }
        }

        if (isTokenActive == true)
            form.AddField("TOKEN", Token);

        return form;
    }
}
