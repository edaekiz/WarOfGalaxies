using System;
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

    private int stepCount;
    private Vector2[] beginPositions;
    private RectTransform[] steps;
    private int offsetValue;

    public EventHandler<StepEventArgs> OnStepChanged;

    // Start is called before the first frame update
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
    }

    // Update is called once per frame
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

        // Sonraki adıma geçişi başlatıyoruz.
        offsetValue = (int)steps[CurrentStep - 1].sizeDelta.x;

        if (OnStepChanged != null)
            OnStepChanged.Invoke(this, new StepEventArgs { CurrentStep = CurrentStep });
    }

    public void GoToPrevStep()
    {
        // Eğer başka bir sayfa yok ise geri dön.
        if (CurrentStep <= 1)
            return;

        // Bir önceki adıma dönüyoruz.
        CurrentStep -= 1;

        // Önceki adıma geçişi başlatıyoruz.
        offsetValue = (int)-steps[CurrentStep - 1].sizeDelta.x;

        if (OnStepChanged != null)
            OnStepChanged.Invoke(this, new StepEventArgs { CurrentStep = CurrentStep });
    }


}
