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
    
    private float secondsBetweenUpdates = 3.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.ghostPlayersDict = new Dictionary<int, GhostPlayer>();

        StopAllCoroutines();
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
        string fullURL = TwitchSecrets.ServerName + "/updatePlayers.php";

        WWWForm form = new WWWForm();

        if (this.currentPlayer != null)
        {
            form.AddField("playerID", this.currentPlayer.playerID);
            form.AddField("playerData", JsonUtility.ToJson(this.currentPlayer));
        }
        else
        {
            form.AddField("playerID", -1);
            form.AddField("playerData", "");
        }

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            Debug.LogError("Data: " + www.downloadHandler.text);

            this.UpdateGhostPlayers(www.downloadHandler.text);
        }
    }

    private void UpdateGhostPlayers(string ghostPlayerJSON)
    {
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
        GameObject ghostPlayerInstance = Instantiate(this.ghostPlayerPrefab, this.transform);
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
