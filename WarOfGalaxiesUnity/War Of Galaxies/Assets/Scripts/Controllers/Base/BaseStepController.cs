using System;
using System.Linq;
using UnityEngine;

public class BaseStepController : MonoBehaviour
{
    public class StepEventArgs : EventArgs
    {
        public int CurrentStep;
    }
    [Header("Steplerin bulunduğu transform.")]
    public Transform StepParent;

    [Header("Aktif step.")]
    public int CurrentStep = 1;

    [Header("Geçiş Hızı")]
    public int TransitionSpeed;

    [Header("Toplam adım miktarı, otomatik hesaplanacak.")]
    private int stepCount;

    [Header("Her bir adımın başlangıç konumları.")]
    private Vector2[] beginPositions;

    [Header("Transformlarını tutuyoruz yürütmek için.")]
    private RectTransform[] steps;

    [Header("Toplam offset değeri stepper hareket ederken.")]
    private int offsetValue;

    /// <summary>
    /// Step değiştiğinde burası tetiklenecek.
    /// </summary>
    public EventHandler<StepEventArgs> OnStepChanged;

    protected virtual void Start()
    {
        stepCount = StepParent.childCount;
        beginPositions = new Vector2[stepCount];
        steps = new RectTransform[stepCount];
        for (int ii = 0; ii < stepCount; ii++)
        {
            Transform step = StepParent.GetChild(ii);
            RectTransform stepRect = step.GetComponent<RectTransform>();
            beginPositions[ii] = stepRect.anchoredPosition;
            steps[ii] = stepRect;
        }

        // Değişen step.
        if (stepCount > 0)
        {
            // Panel değiştiğinde diğerini kapatıyoruz.
            OnStepChanged += new EventHandler<StepEventArgs>((s, o) =>
            {
                for (int ii = 0; ii < steps.Length; ii++)
                {
                    if (ii != o.CurrentStep - 1)
                        steps[ii].gameObject.SetActive(false);
                    else
                        steps[ii].gameObject.SetActive(true);
                }
            });

            OnStepChanged.Invoke(this, new StepEventArgs { CurrentStep = this.CurrentStep });
        }
    }

    protected virtual void Update()
    {
        if (offsetValue == 0)
            return;

        if (offsetValue > 0)
        {
            // Geçiş hızı.
            int ts = TransitionSpeed;

            // Eğer geçiş hızı değerden daha büyük ise değere eşitliyoruz ki fazla kaymasın.
            if (ts > offsetValue)
                ts = offsetValue;

            // Ve geçiş hızı kadar düşüyoruz.
            offsetValue -= ts;

            for (int ii = 0; ii < stepCount; ii++)
            {
                RectTransform stepRect = steps[ii];
                stepRect.anchoredPosition -= new Vector2(ts, 0);
            }
        }

        if (offsetValue < 0)
        {
            // Geçiş hızı.
            int ts = TransitionSpeed;

            if (ts > Mathf.Abs(offsetValue))
                ts = -offsetValue;

            offsetValue += ts;

            for (int ii = 0; ii < stepCount; ii++)
            {
                RectTransform stepRect = steps[ii];
                stepRect.anchoredPosition += new Vector2(ts, 0);
            }
        }

    }

    public void GoToNextStep()
    {
        // Eğer başka bir sayfa yok ise geri dön.
        if (CurrentStep >= stepCount)
            return;

        // Sonraki adıma geçiyoruz.
        CurrentStep += 1;

        // Bir sonraki stepper.
        RectTransform nextStep = steps[CurrentStep - 1];

        // Bir sonraki stepi açıyoruz.
        nextStep.gameObject.SetActive(true);

        // Sonraki adıma geçişi başlatıyoruz.
        offsetValue = (int)nextStep.sizeDelta.x;

        // Değişimi bekleyenleri tetikliyoruz.
        if (OnStepChanged != null)
            OnStepChanged.Invoke(this, new StepEventArgs { CurrentStep = this.CurrentStep });
    }

    public void GoToPrevStep()
    {
        // Eğer başka bir sayfa yok ise geri dön.
        if (CurrentStep <= 1)
            return;

        // Bir önceki adıma dönüyoruz.
        CurrentStep -= 1;

        // Bir önceki stepper.
        RectTransform prevStep = steps[CurrentStep - 1];

        // Önceki adıma geçişi başlatıyoruz.
        offsetValue = (int)-prevStep.sizeDelta.x;

        // Değişimi bekleyenleri tetikliyoruz.
        if (OnStepChanged != null)
            OnStepChanged.Invoke(this, new StepEventArgs { CurrentStep = this.CurrentStep });
    }
}
