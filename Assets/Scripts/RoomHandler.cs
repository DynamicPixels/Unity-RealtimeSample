using System;
using System.Collections;
using System.Collections.Generic;
using DynamicPixels;
using DynamicPixels.GameService.Models;
using DynamicPixels.GameService.Models.outputs;
using DynamicPixels.GameService.Services.MultiPlayer.Match;
using DynamicPixels.GameService.Services.MultiPlayer.Room;
using DynamicPixels.GameService.Services.Table.Models;
using DynamicPixels.GameService.Services.User.Models;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class RoomHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI player1Name;
    [SerializeField] private TextMeshProUGUI player2Name;
    public int otherPlayerId;
    private bool _checkForPlayers;

    private Room _room;
    private Match _match;

    public void SetRoom(Room room)
    {
        _room = room;
        roomName.text = room.Name;
        player1Name.text = room.Players[0].UserId.ToString();
        if (room.Players.Count > 1)
        {
            player2Name.text = room.Players[1].UserId.ToString();
        }

        _checkForPlayers = true;
        _room.OnMessageReceived += MessageReceived;
        GameManager.Instance.isCreator = ConnectionManager.Instance.User.Id == _room.CreatorId;
    }

    private void MessageReceived(object sender, Request e)
    {
        if (e.SenderId == ConnectionManager.Instance.User.Id)
            return;
        var intermediatePayload = JsonConvert.DeserializeObject<string>(e.Payload);
        var matchData = JsonConvert.DeserializeObject<MatchData>(intermediatePayload);
        switch (matchData.customMessage)
        {
            case "position":
                GameManager.Instance.update = true;
                if (GameManager.Instance.isCreator)
                {
                    GameManager.Instance.destinationPlayer2 = MatchData.ToVector3(matchData.player2Position);
                    GameManager.Instance.directionPlayer2 = matchData.player2Direction;
                }
                else
                {
                    GameManager.Instance.destinationPlayer1 = MatchData.ToVector3(matchData.player1Position);
                    GameManager.Instance.directionPlayer1 = matchData.player1Direction;
                    GameManager.Instance.destinationBall = MatchData.ToVector3(matchData.ballPosition);
                    GameManager.Instance.ballVelocity = MatchData.ToVector3(matchData.ballSpeed);
                    GameManager.Instance.player1Score = matchData.score1;
                    GameManager.Instance.player2Score = matchData.score2;
                }

                GameManager.Instance.lastTimeStamp = matchData.timestamp;
                break;
            case "start":
                GameManager.Instance.works.Add(() =>
                {
                    ConnectionManager.Instance.StartMatch(matchData.matchId);
                });
                break;
            case "win":
                GameManager.Instance.works.Add(() =>
                {
                    if (!GameManager.Instance.isCreator)
                        GameManager.Instance.FinishGame(matchData.winner);
                });
                
                
                break;
        }
    }

    public Room GetRoom()
    {
        return _room;
    }

    private void OnEnable()
    {
        if (_checkForPlayers)
        {
            StartCoroutine(CheckForNewUser());
        }
    }

    private IEnumerator CheckForNewUser()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            var result = ConnectionManager.Instance.services.MultiPlayer.RoomService.GetRoomById(_room.Id);
            yield return new WaitUntil(() => result.IsCompleted);
            _room = result.Result.Row;
            player1Name.text = _room.Players[0].UserId.ToString();
            if (_room.Players.Count > 1)
            {
                player2Name.text = _room.Players[1].UserId.ToString();
                otherPlayerId = GameManager.Instance.isCreator ? _room.Players[0].UserId : _room.Players[1].UserId;
                yield break;
            }
            else
            {
                player2Name.text = "";
            }

            if (!_room.Players.Exists(player => player.UserId == _room.CreatorId))
                LeaveRoom();
        }
    }

    public void LeaveRoom()
    {
        ConnectionManager.Instance.services.MultiPlayer.RoomService.Leave(_room.Id);
        UIManager.Instance.GoToMainMenu();
        StopAllCoroutines();
    }

    public void StartMatch(Match match)
    {
        _match = match;
        if (GameManager.Instance.isCreator)
        {
            _room.Broadcast(JsonConvert.SerializeObject(new MatchData()
            {
                ballPosition = new MatchVect(Vector3.zero), customMessage = "start",
                player1Position = new MatchVect(GameManager.Instance.player1.transform.position),
                matchId = match.Id
            }));
        }
        GameManager.Instance.StartGame(this);
    }

    public Match GetMatch()
    {
        return _match;
    }
}