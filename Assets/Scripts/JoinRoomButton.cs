using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI name;
    public void SetRoomJoinButton(string text, Action joinRoom)
    {
        button.onClick.AddListener(joinRoom.Invoke);
        name.text = text;

    }
}
