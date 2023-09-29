using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LeaderboardEntryData
{
    public string username;
    public float value;
    public int placement;    
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntryData> entries;
}
