using System.Collections;
using TMPro;
using UnityEngine;

public class LanguageItemController : MonoBehaviour
{
    private TMP_Text text;

    [Header("Eşleşen dil metnini bulmak için kullanılır.")]
    public string KEYWORD;

    [Header("Metnin sonuna yerleştirilecek olan.")]
    public string Append;

    [Header("Metnin başına yerleştirlecek olan.")]
    public string Prepend;

    IEnumerator Start()
    {
        text = GetComponent<TMP_Text>();
        yield return new WaitUntil(() => LanguageController.LC.IsInitialized == true);
        text.text = $"{Prepend}{LanguageController.LC.GetText(KEYWORD)}{Append}";
    }
}
