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

    [SerializeField]
    private Paintable paintableArea;

    public Team owner = Team.None;

    public float captureThreshold = 0.75f;
    
    public int spawnID;

    public bool isSetup = false;

    [SerializeField]
    private MeshRenderer emblemRenderer;

    [SerializeField]
    private MeshRenderer targetRenderer;

    private void Start()
    {        
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
        Debug.LogError("Owner Updated: " + this.owner);

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
    }

    private void UpdateCapturedStatus()
    {
        int[] playerScores = this.paintableArea.GetScores();

        int totalPixels = this.paintableArea.TEXTURE_SIZE * this.paintableArea.TEXTURE_SIZE;

        float[] teamRatios = new float[3];
        
        teamRatios[0] = (float)playerScores[0] / (float)totalPixels;
        teamRatios[1] = (float)playerScores[1] / (float)totalPixels;
        teamRatios[2] = (float)playerScores[2] / (float)totalPixels;

        //Debug.LogError(this.gameObject.name + " Total Pixels: " + playerScores[3]);
        /*
        if (this.gameObject.name == "targetSpawnPointPrefab")
        {
            TestPixelChecker.instance.GetScores(this.paintableArea.GetSupportTexture());
            Debug.LogError(this.gameObject.name + "\n" +
                "Team1: " + playerScores[0] + " " + teamRatios[0] + "\n" +
                "Team2: " + playerScores[1] + " " + teamRatios[1] + "\n" +
                "Team3: " + playerScores[2] + " " + teamRatios[2]);
        }
        */
        int majorityTeamIndex = -1;
        float biggestRatio = float.NegativeInfinity;

        for (int i = 0; i < teamRatios.Length; i++)
        {
            if (teamRatios[i] > biggestRatio)
            {
                majorityTeamIndex = i;
                biggestRatio = teamRatios[i];
            }
        }

        if (biggestRatio >= this.captureThreshold)
        {
            PaintManager.instance.Paint(this.paintableArea, this.transform.position, 1.0f, 1.0f, 1.0f, MapManager.instance.teamColors[majorityTeamIndex]);
            this.UpdateOwner((Team)(majorityTeamIndex + 1));
        }
        else
        {
            this.UpdateOwner(Team.None);
        }
    }
}
