using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct MatchVect
{
    public MatchVect(Vector3 vec)
    {
        X = vec.x;
        Y = vec.y;
        Z = vec.z;
    }
    public float X;
    public float Y;
    public float Z;
}

[Serializable]
public class MatchData
{
    public MatchVect player1Position;
    public MatchVect player2Position;
    public MatchVect ballPosition;
    public MatchVect ballSpeed;
    public int player1Direction;
    public int player2Direction;
    public string customMessage;
    public int matchId;
    public int winner = -1;
    public long timestamp;
    public int score1 = 0;
    public int score2 = 0;

    public static Vector3 ToVector3(MatchVect matchVect)
    {
        return new Vector3(matchVect.X, matchVect.Y, matchVect.Z);
    }
}
