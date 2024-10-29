using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TwoColumnSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI parameter1;
    [SerializeField] private TextMeshProUGUI parameter2;

    private int id;


    public void Init(string parameter1, string parameter2, int id)
    {
        this.parameter1.text = parameter1;
        this.parameter2.text = parameter2;
        this.id = id;
    }

    public void Delete()
    {
        TableManager.Instance.DeleteFromTable(id);
    }
}
