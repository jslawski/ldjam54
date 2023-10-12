using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CharacterCustomizer;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;

public class PortMap : GameMap
{
    public BrushPool yellowPool;
    public BrushPool redPool;
    public BrushPool bluePool;

    private Texture2D portedMapTexture;

    protected override IEnumerator LoadChunkCoroutine(MapChunk chunk)
    {
        int batchSize = 10;
        int currentBatchSize = 0;

        using (StreamReader reader = new StreamReader(chunk.GetChunkFilePath()))
        {
            string currentLine;

            int lineIndex = 0;

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

                    if (chunkArray[i] == "0")
                    {
                        continue;
                    }

                    int tileStatus = int.Parse(chunkArray[i]);

                    Vector2Int chunkSpaceCoord = new Vector2Int(i, lineIndex);
                    Vector2Int chunkIndices = new Vector2Int(chunk.chunkColumn, chunk.chunkRow);
                    Vector3Int cellSpacePoint = GameMap.ChunkToCellSpace(chunkSpaceCoord, chunkIndices);

                    //this.gameMap.SetTile(cellSpacePoint, GameMap.instance.teamTileDict[(Team)tileStatus]);

                    Vector3 worldPosition = this.gameMap.CellToWorld(cellSpacePoint);
                    this.PaintCell(worldPosition, (Team)tileStatus);
                }

                lineIndex++;
            }
        }
    }
    
    private void PaintCell(Vector3 newPos, Team brushTeam)
    {
        switch (brushTeam)
        {
            case Team.Team1:
                this.yellowPool.CreateBrush(newPos);
                break;
            case Team.Team2:
                this.redPool.CreateBrush(newPos);
                break;
            case Team.Team3:
                this.bluePool.CreateBrush(newPos);
                break;
            default:
                break;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            this.SavePortedMap(this.yellowPool.paintCanvas);
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            this.LoadMap();
        }
    }
    
    private void SavePortedMap(Paintable paintCanvas)
    {
        this.portedMapTexture = new Texture2D(paintCanvas.GetSupportTexture().width, paintCanvas.GetSupportTexture().height, TextureFormat.RGBA32, false, true);
        StartCoroutine(this.SendSaveRequest(paintCanvas));
    }

    private IEnumerator SendSaveRequest(Paintable paintCanvas)
    {
        Debug.LogError("Saving Map...");

        //Upload the new map        
        this.LoadIntoMapTexture(paintCanvas.GetSupportTexture());

        File.WriteAllBytes(Application.dataPath + "/testmapData.png", this.portedMapTexture.EncodeToPNG());

        string fullURL = TwitchSecrets.ServerName + "/uploadMap.php";
        WWWForm form = new WWWForm();
        form.AddBinaryData("mapData", this.portedMapTexture.EncodeToPNG(), "mapData.png");
        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();
        }

        Debug.LogError("Saved!");
    }

    private void LoadIntoMapTexture(RenderTexture rendTex)
    {
        RenderTexture.active = rendTex;
        this.portedMapTexture.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0);
        this.portedMapTexture.filterMode = FilterMode.Bilinear;
        //this.portedMapTexture.alphaIsTransparency = true;
        this.portedMapTexture.Apply();
        RenderTexture.active = null;
    }

    private void OnDestroy()
    {
        Destroy(this.portedMapTexture);
    }
}
