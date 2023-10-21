using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using CharacterCustomizer;
using System;

public class GhostPlayerManager : MonoBehaviour
{
    public static GhostPlayerManager instance;

    [HideInInspector]
    public SinglePlayerData currentPlayer;

    private PlayerData ghostPlayerData;

    [SerializeField]
    private GameObject ghostPlayerPrefab;

    [SerializeField]
    private GameObject ghostPlayersParent;

    private Dictionary<int, GhostPlayer> ghostPlayersDict;
    
    private float secondsBetweenUpdates = 0.04f;

    private UnityWebRequestAsyncOperation updateWebRequest;

    Action<AsyncOperation> updateHandler;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.ghostPlayersDict = new Dictionary<int, GhostPlayer>();

        StartCoroutine(this.Heartbeat());
    }

    private void Start()
    {
        if (PostJamManager.instance.postJam == true)
        {
            this.EnableGhostPlayers();
        }
        else
        {
            this.DisableGhostPlayers();
        }
    }

    public void DisableGhostPlayers()
    {
        this.ghostPlayersParent.SetActive(false);
    }

    public void EnableGhostPlayers()
    {
        this.ghostPlayersParent.SetActive(true);
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
        string fullURL = TwitchSecrets.ServerName;
        WWWForm form = new WWWForm();

        if (this.currentPlayer != null)
        {
            fullURL += "/updatePlayers.php";
            form.AddField("playerID", this.currentPlayer.playerID);
            form.AddField("playerName", this.currentPlayer.playerName);
            form.AddField("playerData", JsonUtility.ToJson(this.currentPlayer.data));
        }
        else
        {
            fullURL += "/getPlayers.php";
        }

        UnityWebRequest www = UnityWebRequest.Post(fullURL, form);
        this.updateWebRequest = www.SendWebRequest();
        this.updateHandler = (data) => { this.UpdateGhostPlayers(www); };
        this.updateWebRequest.completed -= this.updateHandler;
        this.updateWebRequest.completed += this.updateHandler;

        //StartCoroutine(this.PlayerUpdateRequest());
    }
    /*
    private IEnumerator PlayerUpdateRequest()
    {
        string fullURL = TwitchSecrets.ServerName;
        WWWForm form = new WWWForm();

        if (this.currentPlayer != null)
        {
            fullURL += "/updatePlayers.php";
            form.AddField("playerID", this.currentPlayer.playerID);
            form.AddField("playerName", this.currentPlayer.playerName);
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
    */
    private void UpdateGhostPlayers(UnityWebRequest www)
    {
        string ghostPlayerJSON = www.downloadHandler.text;

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

        www.Dispose();
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
        GameObject ghostPlayerInstance = Instantiate(this.ghostPlayerPrefab, instantiationPosition, new Quaternion(), this.ghostPlayersParent.transform);
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
