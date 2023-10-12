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
    
    public Team owner = Team.None;

    private float captureThreshold = 0.66f;
    
    public int spawnID;

    public bool isSetup = false;

    [SerializeField]
    private MeshRenderer emblemRenderer;

    [SerializeField]
    private MeshRenderer targetRenderer;

    private SpawnPointCollider[] capturePoints;

    private void Start()
    {        
        this.Setup();
    }

    public void SpawnPlayerCharacter()
    {
        GameObject playerObject = Instantiate(this.playerPrefab, this.transform.position, new Quaternion());
        playerObject.GetComponent<PlayerCharacter>().Setup();
    }

    private void Setup()
    {
        this.capturePoints = GetComponentsInChildren<SpawnPointCollider>();

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

            //Debug.LogError(gameObject.name + "'s Owner: " + www.downloadHandler.text);

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
        if (newOwner != this.owner)
        {
            this.owner = newOwner;
            StartCoroutine(this.SendOwnerUpdate(newOwner));
        }
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

        this.UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        //Debug.LogError("Owner Updated: " + this.owner);

        switch (this.owner)
        {
            case Team.Team1:
                this.emblemRenderer.enabled = true;
                this.emblemRenderer.material = Resources.Load<Material>("Materials/emblemYellow");
                this.targetRenderer.material = Resources.Load<Material>("Materials/targetGlowYellow");                
                break;
            case Team.Team2:
                this.emblemRenderer.enabled = true;
                this.emblemRenderer.material = Resources.Load<Material>("Materials/emblemRed");
                this.targetRenderer.material = Resources.Load<Material>("Materials/targetGlowRed");
                break;
            case Team.Team3:
                this.emblemRenderer.enabled = true;
                this.emblemRenderer.material = Resources.Load<Material>("Materials/emblemBlue");
                this.targetRenderer.material = Resources.Load<Material>("Materials/targetGlowBlue");
                break;
            case Team.None:
                this.emblemRenderer.enabled = false;
                this.targetRenderer.material = Resources.Load<Material>("Materials/targetGlowGrey");
                break;
            default:
                Debug.LogError("Unknown Team: " + this.owner);
                break;
        }

        this.CaptureAllPoints();
    }

    private void UpdateCapturedStatus()
    {
        int[] teamPoints = new int[3];

        for (int i = 0; i < this.capturePoints.Length; i++)
        {
            switch (this.capturePoints[i].owner)
            {
                case Team.Team1:
                    teamPoints[0]++;
                    break;
                case Team.Team2:
                    teamPoints[1]++;
                    break;
                case Team.Team3:
                    teamPoints[2]++;
                    break;
                default:
                    break;
            }
        }

        float team1Ratio = (float)teamPoints[0] / (float)this.capturePoints.Length;
        float team2Ratio = (float)teamPoints[1] / (float)this.capturePoints.Length;
        float team3Ratio = (float)teamPoints[2] / (float)this.capturePoints.Length;
        /*
        if (this.gameObject.name == "targetSpawnPointPrefab (7)")
        {
            Debug.LogError(this.gameObject.name + "\n" +
                "Team 1: " + team1Ratio + "\n" +
                            "Team 2: " + team2Ratio + "\n" +
                            "Team 3: " + team3Ratio);
        }
        */
        if (team1Ratio >= this.captureThreshold)
        {
            this.UpdateOwner(Team.Team1);
        }
        else if (team2Ratio >= this.captureThreshold)
        {
            this.UpdateOwner(Team.Team2);
        }
        else if (team3Ratio >= this.captureThreshold)
        {
            this.UpdateOwner(Team.Team3);
        }
        else
        {
            this.UpdateOwner(Team.None);
        }
    }

    private void CaptureAllPoints()
    {
        for (int i = 0; i < this.capturePoints.Length; i++)
        {
            this.capturePoints[i].owner = this.owner;
        }
    }
}
