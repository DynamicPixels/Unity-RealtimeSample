using System;
using System.Collections;
using System.Collections.Generic;
using DynamicPixels.GameService.Services.Friendship.Models;
using DynamicPixels.GameService.Services.Leaderboard.Models;
using DynamicPixels.GameService.Services.Party;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject addFriendPanel;
    [SerializeField] private GameObject partyPanel;
    [SerializeField] private GameObject gameScene;
    [SerializeField] private GameObject startButton;
    [SerializeField] private Transform leaderboardContent;
    [SerializeField] private Transform partiesContent;
    [SerializeField] private Transform subsContent;
    [SerializeField] private Transform friendsContent;
    [SerializeField] private Transform reqsContent;
    [SerializeField] private Transform addFriendContent;
    [SerializeField] private LeaderboardScore scoreObject;
    [SerializeField] private PartyButton partyObject;
    [SerializeField] private FriendsObjects friendsObject;
    [SerializeField] private FriendReqsObject reqObject;
    public static UIManager Instance;

    private GameObject _currentPage;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    private void Start()
    {
        _currentPage = loginPanel;
        loginPanel.SetActive(true);
        mainPanel.SetActive(false);
        roomPanel.SetActive(false);
        joinPanel.SetActive(false);
        gameScene.SetActive(false);
    }

    public void GoToMainMenu()
    {
        _currentPage.SetActive(false);
        _currentPage = mainPanel;
        _currentPage.SetActive(true);
    }

    public void GoToLogin()
    {
        _currentPage.SetActive(false);
        _currentPage = loginPanel;
        _currentPage.SetActive(true);
    }
    
    public void GoToRoomHost()
    {
        _currentPage.SetActive(false);
        _currentPage = roomPanel;
        _currentPage.SetActive(true);
        startButton.SetActive(GameManager.Instance.isCreator);
    }

    public void GoToRoomJoin()
    {
        _currentPage.SetActive(false);
        _currentPage = joinPanel;
        _currentPage.SetActive(true);
    }

    public void GoToGame()
    {
        _currentPage.SetActive(false);
        _currentPage = gameScene;
        _currentPage.SetActive(true);
    }

    public void SetLeaderboard(List<UserScore> scores)
    {
        for (int i = 0; i < leaderboardContent.childCount; i++)
            Destroy(leaderboardContent.GetChild(i).gameObject);
        for (int i = 0; i < scores.Count; i++)
        {
            var button = Instantiate(scoreObject, leaderboardContent);
            button.SetUserName((i + 1) + "- " + scores[i].Name);
        }
        _currentPage.SetActive(false);
        _currentPage = leaderboardPanel;
        _currentPage.SetActive(true);
    }
    
    public void SetParties(List<Party> parties, List<Party> subscribes)
    {
        for (int i = 0; i < partiesContent.childCount; i++)
            Destroy(partiesContent.GetChild(i).gameObject);
        for (int i = 0; i < parties.Count; i++)
        {
            var button = Instantiate(partyObject, partiesContent);
            button.Init(parties[i], false);
        }
        for (int i = 0; i < subsContent.childCount; i++)
            Destroy(subsContent.GetChild(i).gameObject);
        for (int i = 0; i < subscribes.Count; i++)
        {
            var button = Instantiate(partyObject, subsContent);
            button.Init(subscribes[i], true);
        }
        _currentPage.SetActive(false);
        _currentPage = partyPanel;
        _currentPage.SetActive(true);
    }

    public void UpdateFriendsList(List<Friendship> friends)
    {
        for (int i = 0; i < friendsContent.childCount; i++)
            Destroy(friendsContent.GetChild(i).gameObject);
        for (int i = 0; i < friends.Count; i++)
        {
            var button = Instantiate(friendsObject, friendsContent);
            button.Init(friends[i]);
        }
    }

    public void UpdateRequests(List<Friendship> friendReqs, List<PartyMember> partyMembers)
    {
        for (int i = 0; i < reqsContent.childCount; i++)
            Destroy(reqsContent.GetChild(i).gameObject);
        for (int i = 0; i < friendReqs.Count; i++)
        {
            var button = Instantiate(reqObject, reqsContent);
            button.Init(friendReqs[i]);
        }
        for (int i = 0; i < partyMembers.Count; i++)
        {
            var button = Instantiate(reqObject, reqsContent);
            button.Init(partyMembers[i]);
        }
    }
}
