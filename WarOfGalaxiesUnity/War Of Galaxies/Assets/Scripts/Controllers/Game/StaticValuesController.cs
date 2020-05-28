using UnityEngine;

public class StaticValuesController : MonoBehaviour
{
    public static StaticValuesController SVC { get; set; }

    [Header("Mobil de default 30dur. Bu yüzden biz belirliyoruz. YÜksek olması şarj ömrünü tüketir.")]
    public int MaxFps = 60;
    private void Awake()
    {
        if (SVC == null)
            SVC = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Application.targetFrameRate = MaxFps;
    }

}
