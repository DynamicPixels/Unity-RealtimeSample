using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userName;

    public void SetUserName(string userName)
    {
        this.userName.text = userName;
    }
}
