using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using CharacterCustomizer;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;

    public int spawnPointSize = 10;

    private List<Vector3Int> cellIndices;

    [SerializeField]
    private Tilemap gameMap;

    public Team owner = Team.None;

    public float captureThreshold = 1.0f;

    [SerializeField]
    private int spawnID;

    private bool isSetup = false;

    private void Start()
    {
        this.SpawnPlayerCharacter();
        this.Setup();
    }

    public void SpawnPlayerCharacter()
    {
        GameObject playerObject = Instantiate(this.playerPrefab, this.transform.position, new Quaternion());
        playerObject.GetComponent<PlayerCharacter>().Setup();

        GameManager.instance.StartHeartbeat();
    }

    private void Setup()
    {
        this.gameMap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        this.cellIndices = new List<Vector3Int>();

        Vector3Int spawnPosition = this.gameMap.WorldToCell(this.transform.position);

        Vector3Int lowerLeft = new Vector3Int(spawnPosition.x - (this.spawnPointSize / 2), spawnPosition.y - (this.spawnPointSize / 2));        

        for (int i = 0; i < this.spawnPointSize / 2; i++)
        {
            for (int j = 0; j < this.spawnPointSize / 2; j++)
            {
                this.cellIndices.Add(new Vector3Int(lowerLeft.x + j, lowerLeft.y + i));
            }
        }

        StartCoroutine(this.RequestOwner());
    }

    private IEnumerator RequestOwner()
    {
        string fullURL = TwitchSecrets.ServerName + "/getSpawnPoint.php";

        WWWForm form = new WWWForm();
        form.AddField("spawnID", spawnID);        

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            this.owner = (Team)int.Parse((www.downloadHandler.text));
        }        

        this.UpdateVisuals();

        this.isSetup = true;
    }

    private void FixedUpdate()
    {
        if (this.isSetup == false)
        {
            return;
        }

        this.UpdateCapturedStatus();
    }

    private void UpdateOwner(Team newOwner)
    {
        StartCoroutine(this.SendOwnerUpdate(newOwner));
    }

    private IEnumerator SendOwnerUpdate(Team newOwner)
    {
        string fullURL = TwitchSecrets.ServerName + "/updateSpawnPoint.php";

        WWWForm form = new WWWForm();
        form.AddField("spawnID", this.spawnID);
        form.AddField("owner", (int)newOwner);

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();            
        }

        this.owner = newOwner;

        this.UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        //Do visual update stuff here
    }

    private void UpdateCapturedStatus()
    {
        int numSpacesRequiredToCapture = (int)(this.cellIndices.Count / this.captureThreshold);

        int team1Spaces = 0;
        int team2Spaces = 0;
        int team3Spaces = 0;

        for (int i = 0; i < this.cellIndices.Count; i++)
        {            
            TileBase currentTile = this.gameMap.GetTile(this.cellIndices[i]);

            if (currentTile.name == "Team1Tile")
            {
                team1Spaces++;
            }
            else if (currentTile.name == "Team2Tile")
            {
                team2Spaces++;
            }
            else if (currentTile.name == "Team3Tile")
            {
                team3Spaces++;
            }
        }

        if (team1Spaces >= numSpacesRequiredToCapture && this.owner != Team.Team1)
        {
            this.UpdateOwner(Team.Team1);
        }
        if (team2Spaces >= numSpacesRequiredToCapture && this.owner != Team.Team2)
        {
            this.UpdateOwner(Team.Team2);
        }
        if (team3Spaces >= numSpacesRequiredToCapture && this.owner != Team.Team3)
        {
            this.UpdateOwner(Team.Team3);
        }
        if (team1Spaces < numSpacesRequiredToCapture && 
            team2Spaces < numSpacesRequiredToCapture && 
            team3Spaces < numSpacesRequiredToCapture && 
            this.owner != Team.None)
        {
            this.UpdateOwner(Team.None);
        }

    }
}
