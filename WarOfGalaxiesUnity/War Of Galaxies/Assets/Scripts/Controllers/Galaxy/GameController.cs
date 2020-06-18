using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
