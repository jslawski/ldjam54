using CharacterCustomizer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class GameMap : MonoBehaviour
{
    public static GameMap instance;

    private List<Vector3> archivedPoints;

    [SerializeField]
    private Tilemap gameMap;

    [SerializeField]
    private Grid mapGrid;

    private TileBase[] teamTiles;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.archivedPoints = new List<Vector3>();

        this.teamTiles = Resources.LoadAll<TileBase>("Tiles");

        this.LoadMap();
    }

    private void LoadMap()
    {
        StartCoroutine(this.RequestMapData());
    }

    private IEnumerator RequestMapData()
    {
        string fullURL = TwitchSecrets.ServerName + "/getMapData.php";

        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            this.ParseMapData(www.downloadHandler.text);
        }
    }

    private void ParseMapData(string mapData)
    {
        string[] mapArray = mapData.Split('\n');

        foreach (string mapLine in mapArray)
        {
            if (mapLine == string.Empty)
            {
                return;
            }

            string[] mapValues = mapLine.Split(',');
            Vector3Int mapIndex = new Vector3Int(int.Parse(mapValues[0]), int.Parse(mapValues[1]), 0);
            int teamID = int.Parse(mapValues[2]);

            switch (teamID)
            {
                case 1:
                    this.gameMap.SetTile(mapIndex, this.teamTiles[0]);
                    break;
                case 2:
                    this.gameMap.SetTile(mapIndex, this.teamTiles[1]);
                    break;
                case 3:
                    this.gameMap.SetTile(mapIndex, this.teamTiles[2]);
                    break;
                default:
                    Debug.LogError("Unknown Team ID: " + mapValues[2]);
                    break;
            }
        }
    }

    private void ArchivePoint(Vector3Int gridIndex, Team teamId)
    {
        this.archivedPoints.Add(new Vector3(gridIndex.x, gridIndex.y, (int)teamId));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            this.SaveLatestMap();
        } 
    }

    private void SaveLatestMap()
    {
        string fullString = "";
        
        for (int i = 0; i < this.archivedPoints.Count; i++)
        {
            fullString += this.archivedPoints[i].x.ToString() + ", " + 
                                this.archivedPoints[i].y.ToString() + ", " +
                                this.archivedPoints[i].z.ToString() + "\n";
        }

        StartCoroutine(this.UploadMap(fullString));
    }

    private IEnumerator UploadMap(string mapData)
    {
        string fullURL = TwitchSecrets.ServerName + "/saveMapData.php";

        WWWForm form = new WWWForm();
        form.AddField("mapdata", mapData);

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            Debug.LogError(www.downloadHandler.text);
        }
    }

    public void UpdateMap(PlayerCharacter player)
    {
        Vector3Int targetCell = this.mapGrid.WorldToCell(player.transform.position);

        TileBase currentTileSprite = this.gameMap.GetTile(targetCell);

        if (currentTileSprite != player.teamTile)
        {
            this.gameMap.SetTile(targetCell, player.teamTile);
            this.ArchivePoint(targetCell, player.playerTeam);
        }

    }
}
