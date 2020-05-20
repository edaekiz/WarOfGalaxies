using Assets.Scripts.ApiModels;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

public class ParameterController : MonoBehaviour
{
    public static ParameterController PC { get; set; }
    public List<ParameterDataDTO> Parameters { get; set; }

    private void Awake()
    {
        if (PC == null)
            PC = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadParameters();
    }

    public void LoadParameters()
    {
        StartCoroutine(ApiService.API.Post("GetParameters", null, (ApiResult response) =>
        {
            Parameters = response.GetDataList<ParameterDataDTO>();
            LoadingController.LC.IncreaseLoadCount();
        }));
    }

    public ParameterDataDTO GetParameter(ParameterTypes parameter) => Parameters.Find(x => x.ParameterId == (int)parameter);

}
