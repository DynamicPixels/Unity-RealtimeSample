using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThreeColumnSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI parameter1;
    [SerializeField] private TextMeshProUGUI parameter2;
    [SerializeField] private TextMeshProUGUI parameter3;
    public void Init(string parameter1, string parameter2, string parameter3)
    {
        this.parameter1.text = parameter1;
        this.parameter2.text = parameter2;
        this.parameter3.text = parameter3;
    }
}
