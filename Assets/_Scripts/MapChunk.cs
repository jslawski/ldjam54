using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapChunk
{
    public string chunkID = string.Empty;

    public int chunkColumn = 0;
    public int chunkRow = 0;

    public bool isDirty = false;

    public bool isUpdating = false;
    
    public List<Vector3Int> dirtyPoints;
    public List<Vector3Int> interimDirtyPoints;

    public MapChunk(string chunkID, string chunkData)
    {
        this.isDirty = false;
        this.SetupChunk(chunkID, chunkData);
    }

    public MapChunk(MapChunk newChunk)
    {
        this.isDirty = newChunk.isDirty;
    }

    public void UpdatePoint(Vector3Int pointCoord, Team playerTeam)
    {
        this.isDirty = true;

        Vector2Int chunkSpaceCoord = GameMap.CellToChunkSpace(pointCoord);

        //Debug.LogError("Chunk Index: " + this.chunkID + " Cell: " + pointCoord + " Chunk: " + chunkSpaceCoord);

        //Debug.LogError("Dirty Point Chunk: " + new Vector3Int(chunkSpaceCoord.x, chunkSpaceCoord.y, (int)playerTeam));

        if (this.isUpdating == false)
        {
            this.dirtyPoints.Add(new Vector3Int(chunkSpaceCoord.x, chunkSpaceCoord.y, (int)playerTeam));
        }
        else
        {
            this.interimDirtyPoints.Add(new Vector3Int(chunkSpaceCoord.x, chunkSpaceCoord.y, (int)playerTeam));
        }
    }

    public void TransferInterimDirtyPoints()
    {
        this.dirtyPoints = new List<Vector3Int>(this.interimDirtyPoints);
        
        this.interimDirtyPoints = new List<Vector3Int>();

        if (this.dirtyPoints.Count > 0)
        {
            this.isDirty = true;
        }
    }

    public void SetupChunk(string chunkID, string chunkData)
    {
        this.chunkID = chunkID;

        string[] chunkIDDigets = chunkID.Split("_");
        this.chunkColumn = int.Parse(chunkIDDigets[0]);
        this.chunkRow = int.Parse(chunkIDDigets[1]);        

        File.WriteAllText(this.GetChunkFilePath(), chunkData, Encoding.UTF8);
        
        this.dirtyPoints = new List<Vector3Int>();
        this.interimDirtyPoints = new List<Vector3Int>();
    }

    public string GetChunkFilePath()
    {
        return Application.persistentDataPath + this.chunkID;
    }
}
