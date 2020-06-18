using System;
using UnityEngine;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class GameController : MonoBehaviour
{
    private DateTime focusOutDate;
    private void OnApplicationFocus(bool focus)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (focus)
        {
            if ((DateTime.Now - focusOutDate).TotalSeconds > 30)
                SceneManager.LoadScene(0);
        }
        else // Eğer arkaya düşerse.
        {
            focusOutDate = DateTime.Now;
        }
#endif
    }
}
