using System;
using System.Collections.Generic;
using DynamicPixels;
using DynamicPixels.GameService;
using DynamicPixels.GameService.Services.Achievement.Models;
using DynamicPixels.GameService.Services.Authentication.Models;
using DynamicPixels.GameService.Services.Chat.Models;
using DynamicPixels.GameService.Services.Chat.Repositories;
using DynamicPixels.GameService.Services.Friendship.Models;
using DynamicPixels.GameService.Services.Leaderboard.Models;
using DynamicPixels.GameService.Services.MultiPlayer.Room.Models;
using DynamicPixels.GameService.Services.Party;
using DynamicPixels.GameService.Services.Party.Models;
using DynamicPixels.GameService.Services.Table;
using DynamicPixels.GameService.Services.User.Models;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI email;
    [SerializeField] private TextMeshProUGUI pass;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI id;
    [SerializeField] private TextMeshProUGUI friendReqField;
    [SerializeField] private TextMeshProUGUI partyName;
    [SerializeField] private RoomHandler roomHandler;
    [SerializeField] private JoinRoomButton roomButton;
    [SerializeField] private Transform content;

    private User user;
    private Conversation globalConversation;

    public User User => user;


    [HideInInspector] public Services services;

    public static ConnectionManager Instance;

    private string _token;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        services = ServiceHub.Services;
    }

    public string GetToken()
    {
        return _token;
    }

    public async void LoginWithEmail()
    {
        var result = await ServiceHub.Authentication.LoginWithEmail(
            new LoginWithEmailParams()
                { email = email.text, password = pass.text });
        // var result = await DynamicPixels.GameService.ServiceHub.Authentication.LoginWithEmail(
        //     new LoginWithEmailParams()
        //         { email = "amirferyg@gmail.com", password = "123456" });
        SignInSuccessful(result);
    }

    public async void LoginWithGoogle()
    {
        ServiceManager.GetService<OpenIDConnectService>().LoginCompleted += GoogleLoginCompleted;

        await ServiceManager.GetService<OpenIDConnectService>().OpenLoginPageAsync();

        Debug.Log("Opened");
    }

    private async void GoogleLoginCompleted(object sender, EventArgs args)
    {
        var accessToken = ServiceManager.GetService<OpenIDConnectService>().AccessToken;
        Debug.Log(accessToken);

        var result = await ServiceHub.Authentication.LoginWithGoogle(
            new LoginWithGoogleParams() { AccessToken = accessToken });
        SignInSuccessful(result);
    }

    public async void Signup()
    {
        var result = await ServiceHub.Authentication.RegisterWithEmail(
            new RegisterWithEmailParams()
                { Email = email.text, Password = pass.text, Name = name.text });
        SignInSuccessful(result);
    }

    public void Logout()
    {
        try
        {
            ServiceHub.Authentication.Logout();
            UIManager.Instance.GoToLogin();
            user = null;
            _token = "";
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public async void LoginAsGuest()
    {
        var result = await ServiceHub.Authentication.LoginAsGuest(new LoginAsGuestParams()
            { name = "Guest-" + Random.Range(0, 100000000) });
        SignInSuccessful(result);
    }

    private void SignInSuccessful(LoginResponse response)
    {
        JoinChatRoom();
        _token = response.Token;
        user = response.User;
        id.text = "Your ID: " + user.Id;
        UIManager.Instance.GoToMainMenu();
        GetFriends();
    }

    public async void LoadChatRoom()
    {
        var messages = await services.Chats.GetConversationMessages(new GetConversationMessagesParams() { });
    }

    public async void CreateRoom()
    {
        var results = await services.MultiPlayer.RoomService.GetAllRooms(new GetAllRoomsParams());
        if (!results.IsSuccessful)
            return;
        foreach (var room in results.List)
        {
            if (room.CreatorId == User.Id)
                await services.MultiPlayer.RoomService.DeleteRoom(room.Id);
        }

        var result = await services.MultiPlayer.RoomService.CreateAndOpenRoom(new CreateRoomParams()
        {
            Name = user.Name + "'s Room",
            IsPermanent = false,
            IsPrivate = false,
            MinPlayer = 2,
            MaxPlayer = 2,
            IsTurnBasedGame = false
        });
        Debug.Log(result.ErrorMessage);
        roomHandler.SetRoom(result.Row);
        UIManager.Instance.GoToRoomHost();
    }


    public async void JoinRoomPanel()
    {
        var results = await services.MultiPlayer.RoomService.GetAllRooms(new GetAllRoomsParams());
        for (int i = 0; i < content.childCount; i++)
            Destroy(content.GetChild(i).gameObject);
        foreach (var room in results.List)
        {
            if (room.Players.Count <= 0)
                continue;
            var button = Instantiate(roomButton, content);
            var temp = room.Id;
            button.SetRoomJoinButton(room.Name, () => JoinRoom(temp));
        }

        UIManager.Instance.GoToRoomJoin();
    }

    public async void LeaderboardPanel()
    {
        var results =
            await services.Leaderboard.GetUsersScores<GetScoresParams, UserScore>(new GetScoresParams()
                { Leaderboardid = 14, limit = 10 });
        UIManager.Instance.SetLeaderboard(results.List);
    }

    public async void GetParties()
    {
        var parties = await services.Party.GetParties(new GetPartiesParams() { Skip = 80 });
        var subscribes = await services.Party.GetSubscribedParties(new GetSubscribedPartiesParams());
        UIManager.Instance.SetParties(parties.List, subscribes.List);
    }

    public async void RemoveParty(int partyId)
    {
        await services.Party.LeaveParty(new LeavePartyParams() { PartyId = partyId });
        GetParties();
    }

    public async void JoinParty(int partyId)
    {
        await services.Party.JoinToParty(new JoinToPartyParams() { PartyId = partyId });
    }

    public async void GetFriends()
    {
        var friends = await services.Friendship.GetMyFriends(new GetMyFriendsParams());
        UIManager.Instance.UpdateFriendsList(friends.List);
        UpdateRequests();
    }

    public async void UpdateRequests()
    {
        var friendRequests = await services.Friendship.GetMyFriendshipRequests(new GetMyFriendshipRequestsParams());
        var subbedParties = await services.Party.GetSubscribedParties(new GetSubscribedPartiesParams());
        var partyRequests = new List<PartyMember>();
        foreach (var party in subbedParties.List)
        {
            if (party.Owner == User.Id)
            {
                partyRequests.AddRange((await services.Party.GetPartyWaitingMembers(new GetPartyWaitingMembersParams()
                    { PartyId = party.Id })).List);
            }
        }

        UIManager.Instance.UpdateRequests(friendRequests.List, partyRequests);
    }

    public async void AddFriend()
    {
        try
        {
            var results = await services.Friendship.RequestFriendship(new RequestFriendshipParams() { UserId = 78 });
            GetFriends();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public async void JoinChatRoom()
    {
        var results = await services.Chats.GetSubscribedConversations(new GetSubscribedConversationsParams());
        foreach (var conversation in results)
        {
            if (conversation.Name == "Global")
            {
                globalConversation = conversation;
                return;
            }
        }

        await services.Chats.Subscribe(new SubscribeParams() { ConversationName = "Global" });

        foreach (var conversation in results)
        {
            if (conversation.Name == "Global")
            {
                globalConversation = conversation;
                return;
            }
        }
    }

    public async void RemoveFriend(int id)
    {
        var result = await services.Friendship.DeleteFriend(new DeleteFriendParams() { UserId = id });
        GetFriends();
    }

    public async void AcceptFriendship(int id)
    {
        await services.Friendship.AcceptRequest(new AcceptRequestParams() { RequestId = id });
        GetFriends();
    }

    public async void RejectFriendship(int id)
    {
        await services.Friendship.RejectRequest(new RejectRequestParams() { RequestId = id });
        GetFriends();
    }

    public async void CreateParty()
    {
        await services.Party.CreateParty(new CreatePartyParams()
        {
            Data = new PartyInput
            {
                Name = partyName.text,
                MaxMemberCount = 10,
                IsPrivate = false
            }
        });
        GetParties();
    }

    public async void Win()
    {
        int score = 0;
        try
        {
            var result =
                await services.Leaderboard.GetMyScore<GetCurrentUserScoreParams, UserScore>(
                    new GetCurrentUserScoreParams()
                        { LeaderboardId = 14 });
            score = result.Row.Value;
        }
        catch (Exception e)
        {
        }

        try
        {
            if (score + 1 == 100)
            {
                await services.Achievement.UnlockAchievement(new UnlockAchievementParams
                {
                    AchievementId = 36,
                    StepId = 43
                });
            }
            else if (score + 1 == 1)
            {
                await services.Achievement.UnlockAchievement(new UnlockAchievementParams
                {
                    AchievementId = 36,
                    StepId = 42
                });
            }
        }
        catch (Exception e)
        {
        }


        await services.Leaderboard.SubmitScore<SubmitScoreParams, UserScore>(new SubmitScoreParams()
            { LeaderboardId = 14, Score = score + 1 });
    }

    public async void JoinRoom(int id)
    {
        var result = await services.MultiPlayer.RoomService.Join(id);
        roomHandler.SetRoom(result.Row);
        UIManager.Instance.GoToRoomHost();
    }

    public async void StartMatch()
    {
        var result = await services.MultiPlayer.MatchService.MakeAndStartMatch(roomHandler.GetRoom().Id);
        roomHandler.StartMatch(result.Row);
    }

    public async void StartMatch(int matchId)
    {
        var result = await services.MultiPlayer.MatchService.LoadMatch(matchId);
        roomHandler.StartMatch(result.Row);
    }
}