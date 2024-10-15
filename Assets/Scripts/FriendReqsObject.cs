using System.Collections;
using System.Collections.Generic;
using DynamicPixels.GameService.Services.Friendship.Models;
using DynamicPixels.GameService.Services.Party;
using TMPro;
using UnityEngine;

public class FriendReqsObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private int _id;
    
    public TextMeshProUGUI GetText()
    {
        return text;
    }

    public void Init(Friendship friendship)
    {
        _id = friendship.Id;
        text.text = friendship.Name;
    }
    
    public void Init(PartyMember partyMember)
    {
        _id = partyMember.Id;
        text.text = partyMember.Player.ToString();
    }

    public void Accept()
    {
        ConnectionManager.Instance.AcceptFriendship(_id);
    }

    public void Reject()
    {
        ConnectionManager.Instance.RejectFriendship(_id);
    }
}
