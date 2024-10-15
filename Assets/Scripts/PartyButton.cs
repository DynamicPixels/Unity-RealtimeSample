using System.Collections;
using System.Collections.Generic;
using DynamicPixels.GameService.Services.Party;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;
    private int _partyId;
    private bool _subscribed;

    public Button GetButton()
    {
        return button;
    }

    public TextMeshProUGUI GetText()
    {
        return text;
    }

    public void Init(Party party, bool subscribed)
    {
        _subscribed = subscribed;
        _partyId = party.Id;
        text.text = party.Name;
    }

    public void OnClick()
    {
        if (_subscribed)
        {
            ConnectionManager.Instance.RemoveParty(_partyId);
        }
        else
        {
            ConnectionManager.Instance.JoinParty(_partyId);
        }
    }
    
}
