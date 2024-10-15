using System.Collections;
using System.Collections.Generic;
using DynamicPixels.GameService.Services.Friendship.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendsObjects : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;
    private int _id;

    public Button GetButton()
    {
        return button;
    }

    public TextMeshProUGUI GetText()
    {
        return text;
    }

    public void Init(Friendship friendship)
    {
        _id = friendship.Id;
        text.text = friendship.Name;
        button.onClick.AddListener(()=>ConnectionManager.Instance.RemoveFriend(_id));
    }
}
