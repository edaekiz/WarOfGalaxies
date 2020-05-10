using System.Collections;
using TMPro;
using UnityEngine;

public class LanguageItemController : MonoBehaviour
{
    private TextMeshProUGUI text;

    [Header("Eşleşen dil metnini bulmak için kullanılır.")]
    public string KEYWORD;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(RefreshText());
    }

    /// <summary>
    /// Textini günceller.
    /// </summary>
    public IEnumerator RefreshText()
    {
        // Dilin yüklenmesini bekliyoruz.
        if (!LanguageController.LC.IsInitialized)
            yield return new WaitUntil(() => LanguageController.LC.IsInitialized == true);

        text.text = LanguageController.LC.GetText(KEYWORD);
    }

    /// <summary>
    /// Şuan ki text değerini verir.
    /// </summary>
    /// <returns></returns>
    public string GetCurrentText()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();
        return text.text;
    }
}
