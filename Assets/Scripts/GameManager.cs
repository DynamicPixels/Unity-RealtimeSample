using System;
using System.Collections;
using System.Collections.Generic;
using DynamicPixels;
using Newtonsoft.Json;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public int player1Score;
    public int player2Score;
    public static GameManager Instance;
    public bool move;
    public bool isCreator;
    public Player player1;
    public Player player2;
    public Ball ball;
    public RoomHandler room;
    public List<Action> works;
    public bool update;
    public Vector3 destinationPlayer1;
    public Vector3 destinationPlayer2;
    public int directionPlayer1;
    public int directionPlayer2;
    public Vector3 destinationBall;
    public Vector3 ballVelocity;
    public long lastTimeStamp;

    private void Awake()
    {
        Instance = this;
        works = new List<Action>();
    }

    public void StartGame(RoomHandler roomHandler)
    {
        player1Score = 0;
        player2Score = 0;
        ball.transform.position = Vector3.zero;
        room = roomHandler;
        UIManager.Instance.GoToGame();
        move = true;
        StartCoroutine(StartMatchLoop());
        RealtimeObserver.Instance.StartSync(roomHandler.GetRoom(), ConnectionManager.Instance.User, isCreator);
    }

    
    private IEnumerator StartMatchLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            SendMatchData();
        }
    }

    private void SendMatchData()
    {
        var matchData = new MatchData();
        if (isCreator)
        {
            matchData.ballPosition = new MatchVect(ball.transform.position);
            matchData.player1Position = new MatchVect(player1.transform.position);
            matchData.player1Direction = player1.direction;
            matchData.ballSpeed = new MatchVect(ball.GetVelocity());
            matchData.customMessage = "position";
            matchData.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            matchData.score1 = player1Score;
            matchData.score2 = player2Score;
        }
        else
        {
            matchData.player2Position = new MatchVect(player2.transform.position);
            matchData.player2Direction = player2.direction;
            matchData.customMessage = "position";
            matchData.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        room.GetRoom().Broadcast(JsonConvert.SerializeObject(matchData));
    }

    private void Update()
    {
        // if (isCreator && Input.GetKeyDown(KeyCode.Escape) && gameObject.activeSelf)
        //     PauseAndLeave();
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();
        if (update)
        {
            var timeDiff = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastTimeStamp;
            if (isCreator)
            {
                player2.transform.position = destinationPlayer2 + new Vector3(0, directionPlayer2 * player2.GetSpeed() * timeDiff / 1000f, 0);
                player2.SetDirection(directionPlayer2);
            }
            else
            {
                player1.transform.position = destinationPlayer1 + new Vector3(0, directionPlayer1 * player1.GetSpeed() * timeDiff / 1000f, 0);
                player1.SetDirection(directionPlayer1);
                ball.transform.position = destinationBall + (timeDiff / 1000f) * ballVelocity;
                ball.SetVelocity(ballVelocity);
            }
            update = false;
        }
        if (works.Count > 0)
        {
            foreach (var work in works)
            {
                work();
            }
            works.Clear();
        }
    }

    public void FinishGame(int i)
    {
        if (isCreator && i == 0)
            ConnectionManager.Instance.Win();
        else if (!isCreator && i == 1)
            ConnectionManager.Instance.Win();
        UIManager.Instance.GoToMainMenu();
        if (!isCreator)
            return;
        room.GetRoom().Broadcast(JsonConvert.SerializeObject(new MatchData(){customMessage = "win", winner = i}));
        room.GetMatch().Finish();
    }

    public void PauseAndLeave()
    {
        room.GetMatch().Pause("");
        PlayerPrefs.SetInt("LastMatch", room.GetMatch().Id);
    }
}
