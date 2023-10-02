using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TeamTile : Tile
{
    public Team owningTeam = Team.None;

    [SerializeField]
    private Sprite[] teamSprites;

    public void SetSprite(Sprite newSprite)
    {
        this.sprite = newSprite;
        return;
       
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        switch (this.owningTeam)
        {
            case Team.Team1:
                tileData.sprite = teamSprites[0];
                break;
            case Team.Team2:
                tileData.sprite = teamSprites[1];
                break;
            case Team.Team3:
                tileData.sprite = teamSprites[2];
                break;
            default:
                Debug.LogError("Unknown Team: " + this.owningTeam);
                break;
        }
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);
    }
}
