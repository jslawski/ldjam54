using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Team { None, Team1, Team2, Team3 };

public class PlayerCharacter : MonoBehaviour
{
    public Team playerTeam;

    public TileBase teamTile;

    // Start is called before the first frame update
    void Start()
    {
        switch (this.playerTeam)
        {
            case Team.Team1:
                this.teamTile = Resources.Load<TileBase>("Tiles/Team1Tile");
                break;
            case Team.Team2:
                this.teamTile = Resources.Load<TileBase>("Tiles/Team2Tile");
                break;
            case Team.Team3:
                this.teamTile = Resources.Load<TileBase>("Tiles/Team3Tile");
                break;
            default:
                Debug.LogError("Unknown team: " + this.playerTeam);
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameMap.instance.UpdateMap(this);
    }
}
