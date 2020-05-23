using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ToastController : MonoBehaviour
{
    public static ToastController TC { get; set; }

    [Header("Toast mesajı buraya basılacak.")]
    public GameObject ToastItem;

    [Header("Toast paneli.")]
    public GameObject ToastPanel;

    [Header("Ekrana basılan toastlar.")]
    public List<TMP_Text> ShownTexts = new List<TMP_Text>();

    [Header("Kaybolma hızı.")]
    public float FadeSpeed;

    [Header("Aktif Toast panel.")]
    public GameObject ShownToastPanel;

    private void Awake()
    {
        if (TC == null)
            TC = this;
        else
            Destroy(gameObject);
    }
    /// <summary>
    /// Bir toast mesajı basar.
    /// </summary>
    /// <param name="message">Basılacak olan mesaj.</param>
    public void ShowToast(string message)
    {
        // Toast panel yok ise oluşturoyurz.
        if (ShownToastPanel == null)
            ShownToastPanel = Instantiate(ToastPanel);

        // Toast panel altında bir tane toast oluşturuyoruz.
        GameObject toastMessage = Instantiate(ToastItem, ShownToastPanel.transform.Find("ToastContent"));

        // Ve verilen mesajı basıyoruz.
        TMP_Text toastText = toastMessage.GetComponent<TMP_Text>();
        toastText.text = message;

        // Gösterilenler listesine ekliyoruz.
        ShownTexts.Add(toastText);
    }

    /// <summary>
    /// Bütün toast mesajlarını temizler ekranda gösterilen.
    /// </summary>
    public void ClearToastMesages()
    {
        ShownTexts.Clear();
        Destroy(ShownToastPanel.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

        // Eğer toast message yok ise geri dön.
        if (ShownTexts.Count == 0)
            return;

        // Gösterilen her bir toast zaman içerisinde kaybolacak.
        foreach (TMP_Text shownText in ShownTexts)
        {
            // Eski rengini opacity değerini değiştiriyoruz.
            Color oldColor = shownText.color;

            // Kaybolma hızıyla orantılı olarak değiştiriyoruz.
            oldColor.a -= FadeSpeed * Time.deltaTime;

            // YEni değeri atıyoruz.
            shownText.color = oldColor;
        }

        // Eğer bütün toastlar kaybolduysa toast panelini siliyoruz.
        if (ShownTexts.All(x => x.color.a <= 0))
            ClearToastMesages();
    }
}
