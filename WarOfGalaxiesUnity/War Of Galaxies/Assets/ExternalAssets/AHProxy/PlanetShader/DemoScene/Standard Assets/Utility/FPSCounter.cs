using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Utility
{
    [RequireComponent(typeof (Text))]
    public class FPSCounter : MonoBehaviour
    {
        private Text m_Text;

        private float deltaTime;
        private void Start()
        {
            m_Text = GetComponent<Text>();
        }


        private void Update()
        {
            #region Fps Deðeri

            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            m_Text.text = Mathf.Ceil(fps).ToString();

            #endregion
        }
    }
}
