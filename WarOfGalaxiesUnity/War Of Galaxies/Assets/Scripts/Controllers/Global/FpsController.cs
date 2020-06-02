using TMPro;
using UnityEngine;

public class FpsController : MonoBehaviour
{
    [Header("Fpsi buraya basacağız.")]
    public TMP_Text TXT_FpsCounter;

    private float deltaTime;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        TXT_FpsCounter.text = Mathf.Ceil(fps).ToString();
    }
}
