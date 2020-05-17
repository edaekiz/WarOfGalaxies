using UnityEngine;
using UnityEngine.UI;

public class FleetPanelController : BasePanelController
{
    [Header("Filo bilgilerini yükleyeceğimiz pref.")]
    public GameObject FleetItem;

    [Header("Filoların yükleneceği alan.")]
    public ScrollRect FleetItemContent;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
