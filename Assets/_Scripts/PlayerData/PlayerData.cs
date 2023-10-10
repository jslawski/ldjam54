using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SinglePlayerDataValues
{
    public float posX;
    public float posY;
    public float rot;
    public float speed;
    public int team;

    public SinglePlayerDataValues()
    {
        this.posX = 0.0f;
        this.posY = 0.0f;
        this.rot = 0.0f;
        this.speed = 0.0f;
        this.team = -1;
    }

    public SinglePlayerDataValues(SinglePlayerDataValues dataToCopy)
    {
        this.posX = dataToCopy.posX;
        this.posY = dataToCopy.posY;
        this.rot = dataToCopy.rot;
        this.speed = dataToCopy.speed;
        this.team = dataToCopy.team;
    }
}

[Serializable]
public class SinglePlayerData
{
    public int playerID;
    public SinglePlayerDataValues data;

    public SinglePlayerData()
    {
        this.playerID = -1;
        this.data = new SinglePlayerDataValues();
    }

    public SinglePlayerData(SinglePlayerData dataToCopy)
    {
        this.playerID = dataToCopy.playerID;
        this.data = new SinglePlayerDataValues(dataToCopy.data);
    }
}

[Serializable]
public class PlayerData
{
    public List<SinglePlayerData> players;
}
