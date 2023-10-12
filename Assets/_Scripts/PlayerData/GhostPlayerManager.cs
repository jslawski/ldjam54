using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using CharacterCustomizer;

public class GhostPlayerManager : MonoBehaviour
{
    public static GhostPlayerManager instance;

    [HideInInspector]
    public SinglePlayerData currentPlayer;

    private PlayerData ghostPlayerData;

    [SerializeField]
    private GameObject ghostPlayerPrefab;

    private Dictionary<int, GhostPlayer> ghostPlayersDict;
    
    private float secondsBetweenUpdates = 0.04f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.ghostPlayersDict = new Dictionary<int, GhostPlayer>();
    }

    private void Start()
    {
        if (PostJamManager.instance.postJam == true)
        {
            this.EnableGhostPlayers();
        }
    }

    public void DisableGhostPlayers()
    {
        this.StopAllCoroutines();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    public void EnableGhostPlayers()
    {
        StartCoroutine(this.Heartbeat());
    }

    private IEnumerator Heartbeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(this.secondsBetweenUpdates);
            this.SendUpdate();
        }
    }

    private void SendUpdate()
    {
        StartCoroutine(this.PlayerUpdateRequest());
    }

    private IEnumerator PlayerUpdateRequest()
    {
        string fullURL = TwitchSecrets.ServerName;
        WWWForm form = new WWWForm();

        if (this.currentPlayer != null)
        {
            fullURL += "/updatePlayers.php";
            form.AddField("playerID", this.currentPlayer.playerID);
            form.AddField("playerData", JsonUtility.ToJson(this.currentPlayer.data));
        }
        else
        {
            fullURL += "/getPlayers.php";
        }

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            this.UpdateGhostPlayers(www.downloadHandler.text);
        }
    }

    private void UpdateGhostPlayers(string ghostPlayerJSON)
    {
        if (ghostPlayerJSON == string.Empty)
        {
            return;
        }

        if (ghostPlayerJSON == "{\"players\":[]}")
        {
            return;
        }

        this.ghostPlayerData = JsonUtility.FromJson<PlayerData>(ghostPlayerJSON);

        this.UpdateActivePlayers();
        this.PruneInactivePlayers();
    }

    private void UpdateActivePlayers()
    {
        for (int i = 0; i < this.ghostPlayerData.players.Count; i++)
        {
            SinglePlayerData currentData = this.ghostPlayerData.players[i];

            if (this.ghostPlayersDict.ContainsKey(currentData.playerID))
            {
                this.ghostPlayersDict[currentData.playerID].UpdatePlayerData(new SinglePlayerData(currentData));
            }
            else
            {
                this.CreateNewGhostPlayer(new SinglePlayerData(currentData));
            }
        }
    }

    private void CreateNewGhostPlayer(SinglePlayerData playerData)
    {
        Vector3 instantiationPosition = new Vector3(playerData.data.posX, 0.0f, playerData.data.posY);
        GameObject ghostPlayerInstance = Instantiate(this.ghostPlayerPrefab, instantiationPosition, new Quaternion(), this.transform);
        GhostPlayer ghostPlayerComponent = ghostPlayerInstance.GetComponent<GhostPlayer>();
        ghostPlayerComponent.Setup(playerData);
        this.ghostPlayersDict[ghostPlayerComponent.playerData.playerID] = ghostPlayerComponent;
    }

    private void PruneInactivePlayers()
    {
        List<int> playersToRemove = new List<int>();

        foreach (KeyValuePair<int, GhostPlayer> entry in this.ghostPlayersDict)
        {
            bool isInactive = true;

            for (int i = 0; i < this.ghostPlayerData.players.Count; i++)
            {
                if (entry.Key == this.ghostPlayerData.players[i].playerID)
                {
                    isInactive = false;
                }
            }

            if (isInactive == true)
            {
                playersToRemove.Add(entry.Key);
            }
        }

        for (int i = 0; i < playersToRemove.Count; i++)
        {
            GameObject objectToDestroy = this.ghostPlayersDict[playersToRemove[i]].gameObject;
            this.ghostPlayersDict.Remove(playersToRemove[i]);
            Destroy(objectToDestroy);
        }
    }
}
