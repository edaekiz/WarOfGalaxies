using Assets.Scripts.Enums;
using System;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class PanelData
    {
        public PanelTypes PanelType;
        public GameObject Prefab;
    }
}
