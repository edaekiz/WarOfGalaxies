using System.Collections;
using TMPro;
using UnityEngine;

public class LanguageItemController : MonoBehaviour
{
    private TMP_Text text;

    [Header("Eşleşen dil metnini bulmak için kullanılır.")]
    public string KEYWORD;

    IEnumerator Start()
    {
        text = GetComponent<TMP_Text>();
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
            text = GetComponent<TMP_Text>();
        return text.text;
    }
}
