using CharacterCustomizer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class GameMap : MonoBehaviour
{
    public static GameMap instance;

    private Dictionary<Tuple<int, int>, int> archivedPoints;

    private Dictionary<Tuple<int, int>, MapChunk> mapChunks;
    
    public Tilemap gameMap;

    [SerializeField]
    private Grid mapGrid;

    public Dictionary<Team, TileBase> teamTileDict;

    public TileBase[] teamTiles;

    //List<Texture2D> mapChunks;

    int loadedChunks = 0;

    private static int fullMapWidth;
    private static int fullMapHeight;

    private static int chunkWidth;
    private static int chunkHeight;

    [SerializeField]
    private TextAsset[] mapChunkFiles;

    public bool isSetup = false;

    private void Awake()
    {
        fullMapHeight = 804;
        fullMapWidth = 804;
        chunkWidth = 268;
        chunkHeight = 268;

        if (instance == null)
        {
            instance = this;
        }
        
        this.archivedPoints = new Dictionary<Tuple<int, int>, int>();
        this.mapChunks = new Dictionary<Tuple<int, int>, MapChunk>();

        this.SetupDict();
    }

    private void SetupDict()
    {
        this.teamTiles = Resources.LoadAll<TileBase>("Tiles");

        this.teamTileDict = new Dictionary<Team, TileBase>();
        this.teamTileDict.Add(Team.Team1, this.teamTiles[0]);
        this.teamTileDict.Add(Team.Team2, this.teamTiles[1]);
        this.teamTileDict.Add(Team.Team3, this.teamTiles[2]);
    }

    public void LoadMap()
    {
        StartCoroutine(this.RequestMapData());
    }
    
    private IEnumerator RequestMapData()
    {        
        int numChunkColumns = GameMap.fullMapWidth / GameMap.chunkWidth;
        int numChunkRows = GameMap.fullMapHeight / GameMap.chunkHeight;

        for (int i = 0; i < numChunkRows; i++)
        {
            for (int j = 0; j < numChunkColumns; j++)
            {
                string chunkID = j.ToString() + "_" + i.ToString();

                string fullURL = TwitchSecrets.ServerName + "/" + chunkID + ".txt";
                
                using (UnityWebRequest www = UnityWebRequest.Get(fullURL))
                {
                    //Debug.Log("Requesting Chunk: " + j.ToString() + "_" + i.ToString());

                    yield return www.SendWebRequest();

                    //Debug.Log("Obtained Chunk: " + j.ToString() + "_" + i.ToString());

                    this.mapChunks[new Tuple<int, int>(j, i)] = new MapChunk(chunkID, www.downloadHandler.text);
                    this.LoadChunk(j, i);
                }

                yield return null;
            }
        }        
    }

    private void LoadChunk(int x, int y)
    {
        StartCoroutine(this.LoadChunkCoroutine(this.mapChunks[new Tuple<int, int>(x, y)]));
    }

    private IEnumerator LoadChunkCoroutine(MapChunk chunk)
    {
        //Debug.Log("Loading Chunk: " + chunk.chunkID);

        int batchSize = 10000;
        int currentBatchSize = 0;

        int tilesChanged = 0;

        using (StreamReader reader = new StreamReader(chunk.GetChunkFilePath()))
        {
            string currentLine;

            int lineIndex = 0;

            int tilesCompleted = 0;

            while (!reader.EndOfStream)
            {
                currentLine = reader.ReadLine();
                
                string[] chunkArray = currentLine.Split(' ');

                for (int i = 0; i < chunkArray.Length; i++)
                {
                    if (currentBatchSize >= batchSize)
                    {
                        currentBatchSize = 0;
                        yield return null;
                    }

                    currentBatchSize++;
                    tilesCompleted++;
                    if (chunkArray[i] == "0")
                    {
                        continue;
                    }

                    int tileStatus = int.Parse(chunkArray[i]);

                    Vector2Int chunkSpaceCoord = new Vector2Int(i, lineIndex);
                    Vector2Int chunkIndices = new Vector2Int(chunk.chunkColumn, chunk.chunkRow);
                    Vector3Int cellSpacePoint = GameMap.ChunkToCellSpace(chunkSpaceCoord, chunkIndices);

                    //Debug.LogError("Chunk Space Coord: " + chunkSpaceCoord + "\n" +
                    //    "CellSpacePoint: " + cellSpacePoint);

                    tilesChanged++;

                    this.gameMap.SetTile(cellSpacePoint, GameMap.instance.teamTileDict[(Team)tileStatus]);                    
                }

                lineIndex++;
            }
        }

        //Debug.Log("Done Loading Chunk: " + chunk.chunkID);
        //Debug.LogError("Tiles Changed: " + tilesChanged);

        this.loadedChunks++;
    }

    public void SaveLatestMap()
    {        
        foreach (KeyValuePair<Tuple<int, int>, MapChunk> entry in this.mapChunks)
        {            
            if (entry.Value.isDirty == true)
            {
                this.UploadChunk(entry.Key.Item1, entry.Key.Item2);
            }
            
        }
    }

    private void UploadChunk(int xIndex, int yIndex)
    {
        MapChunk chunk = this.mapChunks[new Tuple<int, int>(xIndex, yIndex)];

        StartCoroutine(this.UpdateChunkFile(chunk));
    }

    private void UpdateChunk(Vector3Int cellPosition, Team playerTeam)
    {
        Vector2Int chunkIndices = GameMap.GetChunkIndices(cellPosition);        

        this.mapChunks[new Tuple<int, int>(chunkIndices.x, chunkIndices.y)].UpdatePoint(cellPosition, playerTeam);
    }

    private IEnumerator UpdateChunkFile(MapChunk chunk)
    {
        chunk.isUpdating = true;

        string filePath = chunk.GetChunkFilePath();
        
        string[] allChunkLines = File.ReadAllLines(chunk.GetChunkFilePath());

        int batchSize = 100;
        int currentBatchSize = 0;

        int tilesSaved = 0;

        List<Vector3Int> tempList = new List<Vector3Int>(chunk.dirtyPoints);

        for (int i = 0; i < tempList.Count; i++)
        {
            //Get the line

            //Debug.Log("Chunk: " + chunk.chunkID + " Dirty Point: " + tempList[i]);

            string[] lineValues = allChunkLines[tempList[i].y].Split(' ');

            //Replace value in line
            lineValues[tempList[i].x] = tempList[i].z.ToString();

            //Create new string to replace line
            string tempString = "";
            for (int j = 0; j < lineValues.Length - 1; j++)
            {
                tempString += lineValues[j] + " ";
            }

            tempString += lineValues[lineValues.Length - 1];

            //Replace Line
            allChunkLines[tempList[i].y] = tempString;

            currentBatchSize++;

            if (currentBatchSize >= batchSize)
            {
                currentBatchSize = 0;
                yield return null;
            }

            tilesSaved++;
        }

        string fullString = "";
        for (int i = 0; i < allChunkLines.Length; i++)
        {
            fullString += allChunkLines[i] + '\n';
        }
        
        File.WriteAllLines(chunk.GetChunkFilePath(), allChunkLines, Encoding.UTF8);
        
        string fullURL = TwitchSecrets.ServerName + "/saveMapData.php";

        TextAsset chunkFile = Resources.Load<TextAsset>("MapChunks/" + chunk.chunkID);

        WWWForm form = new WWWForm();
        
        form.AddField("chunkID", chunk.chunkID);                
        form.AddBinaryData("chunkData",  Encoding.UTF8.GetBytes(fullString));

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            www.useHttpContinue = false;

            yield return www.SendWebRequest();

        }        

        chunk.isDirty = false;
        chunk.isUpdating = false;

        chunk.TransferInterimDirtyPoints();

        //Debug.LogError("Tiles Saved: " + tilesSaved);
    }    

    public void UpdateMap(PlayerCharacter player)
    {
        if (this.loadedChunks < 9)
        {
            return;
        }

        this.isSetup = true;

        List<Vector3Int> targetCells = new List<Vector3Int>();

        Vector3Int cellPosition = this.mapGrid.WorldToCell(player.transform.position);

        //Debug.LogError("Chunk: " + GetChunkIndices(cellPosition));

        this.GetTargetCells(cellPosition, player.brushSize, 0, ref targetCells);

        for (int i = 0; i < targetCells.Count; i++)
        {
            TileBase currentTile = this.gameMap.GetTile(targetCells[i]);
            if (currentTile == null || currentTile != this.teamTileDict[player.playerTeam])
            {
                this.gameMap.SetTile(targetCells[i], this.teamTileDict[player.playerTeam]);                
                this.UpdateChunk(targetCells[i], player.playerTeam);
            }
        }
    }

    private void GetTargetCells(Vector3Int cellPosition, int brushSize, int currentIteration, ref List<Vector3Int> targetCells)
    {
        if (currentIteration >= brushSize || targetCells.Contains(cellPosition) == true)
        {
            return;
        }        

        targetCells.Add(cellPosition);

        Vector3Int northPos = new Vector3Int(cellPosition.x, cellPosition.y + 1);
        Vector3Int northEastPos = new Vector3Int(cellPosition.x + 1,  cellPosition.y + 1);
        Vector3Int eastPos = new Vector3Int(cellPosition.x + 1, cellPosition.y);
        Vector3Int southEastPos = new Vector3Int(cellPosition.x + 1, cellPosition.y - 1);
        Vector3Int southPos = new Vector3Int(cellPosition.x, cellPosition.y - 1);
        Vector3Int southWestPos = new Vector3Int(cellPosition.x - 1, cellPosition.y - 1);
        Vector3Int westPos = new Vector3Int(cellPosition.x - 1, cellPosition.y);
        Vector3Int northWestPos = new Vector3Int(cellPosition.x - 1, cellPosition.y + 1);

        this.GetTargetCells(northPos, brushSize, currentIteration + 1, ref targetCells);
        this.GetTargetCells(northEastPos, brushSize, currentIteration + 1, ref targetCells);
        this.GetTargetCells(eastPos, brushSize, currentIteration + 1, ref targetCells);
        this.GetTargetCells(southEastPos, brushSize, currentIteration + 1, ref targetCells);
        this.GetTargetCells(southPos, brushSize, currentIteration + 1, ref targetCells);
        this.GetTargetCells(southWestPos, brushSize, currentIteration + 1, ref targetCells);
        this.GetTargetCells(westPos, brushSize, currentIteration + 1, ref targetCells);
        this.GetTargetCells(northWestPos, brushSize, currentIteration + 1, ref targetCells);
    }

    public static Vector2Int CellToChunkSpace(Vector3Int cellPosition)
    {
        int adjustedCellPosX = cellPosition.x + (GameMap.fullMapWidth / 2);
        int adjustedCellPosY = cellPosition.y - (GameMap.fullMapHeight / 2);

        Vector2Int chunkIndices = GameMap.GetChunkIndices(cellPosition);
        
        int chunkSpaceX = adjustedCellPosX - (GameMap.chunkWidth * chunkIndices.x);
        int chunkSpaceY = adjustedCellPosY + (GameMap.chunkHeight * (chunkIndices.y + 1));

        chunkSpaceX = Mathf.Max(0, chunkSpaceX);
        chunkSpaceX = Mathf.Min(chunkSpaceX, GameMap.chunkWidth);

        chunkSpaceY = Mathf.Max(0, chunkSpaceY);
        chunkSpaceY = Mathf.Min(chunkSpaceY, GameMap.chunkHeight);

        return new Vector2Int(chunkSpaceX, chunkSpaceY);
    }

    public static Vector3Int ChunkToCellSpace(Vector2Int chunkPosition,  Vector2Int chunkIndices)
    {
        int cellPosX = chunkPosition.x + (GameMap.chunkWidth * chunkIndices.x) - (GameMap.fullMapWidth / 2);
        int cellPosY = chunkPosition.y - (GameMap.chunkHeight * (chunkIndices.y +1)) + (GameMap.fullMapHeight / 2);        

        return new Vector3Int(cellPosX, cellPosY, 0);
    }

    public static Vector2Int GetChunkIndices(Vector3Int cellPosition)
    {
        Vector2Int adjustedCellPos = new Vector2Int(cellPosition.x + (GameMap.fullMapWidth / 2), cellPosition.y - (GameMap.fullMapHeight / 2));

        int xIndex = Mathf.FloorToInt((float)adjustedCellPos.x / (float)GameMap.chunkWidth);
        int yIndex = Mathf.FloorToInt((float)-adjustedCellPos.y / (float)GameMap.chunkHeight);

        xIndex = Mathf.Max(0, xIndex);
        xIndex = Mathf.Min(xIndex, 2);

        yIndex = Mathf.Max(0, yIndex);
        yIndex = Mathf.Min(yIndex, 2);

        return new Vector2Int(xIndex, yIndex);

    }
}
