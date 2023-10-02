using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Team { None, Team1, Team2, Team3 };

public class PlayerCharacter : MonoBehaviour
{    
    public Team playerTeam = Team.None;

    public int brushSize = 1;

    // Start is called before the first frame update
    public void Setup(Team selectedTeam)
    {
        this.playerTeam = selectedTeam;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.playerTeam == Team.None)
        {
            return;
        }

        GameMap.instance.UpdateMap(this);
    }
}
