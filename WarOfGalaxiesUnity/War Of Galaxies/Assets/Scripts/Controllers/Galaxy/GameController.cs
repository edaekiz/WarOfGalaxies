using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private void OnApplicationFocus(bool focus)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (focus)
            SceneManager.LoadScene(0);
#endif
    }
}
