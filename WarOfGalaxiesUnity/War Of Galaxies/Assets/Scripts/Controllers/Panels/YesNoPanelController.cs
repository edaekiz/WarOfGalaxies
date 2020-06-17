using System;
using TMPro;
using UnityEngine;

public class YesNoPanelController : BasePanelController
{
    [Header("Soru Başlığı burada yazacak.")]
    public TMP_Text QuestionTitle;

    [Header("Soru içeriği burada yazacak.")]
    public TMP_Text ContentTitle;

    /// <summary>
    /// Evete tıklandığında tetiklenecek.
    /// </summary>
    public Action OnClickYes;

    /// <summary>
    /// Hayıra tıklandığında tetiklenecek.
    /// </summary>
    public Action OnClickNo;

    private void Awake()
    {
        foreach (YesNoPanelController yesNo in FindObjectsOfType<YesNoPanelController>())
            if (yesNo != this)
                yesNo.ClosePanel();
    }

    public void OnYes()
    {
        if (OnClickYes != null)
            OnClickYes.Invoke();
        base.ClosePanel();
    }

    public void OnNo()
    {
        if (OnClickNo != null)
            OnClickNo.Invoke();
        base.ClosePanel();
    }

    public void LoadData(string title, string content, Action onClickYes = null, Action onClickNo = null)
    {
        this.QuestionTitle.text = title;
        this.ContentTitle.text = content;
        this.OnClickYes = onClickYes;
        this.OnClickNo = onClickNo;
    }

}
