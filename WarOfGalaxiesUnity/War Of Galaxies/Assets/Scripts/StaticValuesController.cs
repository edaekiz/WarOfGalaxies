using UnityEngine;

public class StaticValuesController : MonoBehaviour
{
    public static StaticValuesController SVC { get; set; }

    private void Awake()
    {
        if (SVC == null)
            SVC = this;
        else
            Destroy(gameObject);
    }
}
