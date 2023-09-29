using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterCustomizer;
using UnityEngine.Networking;

public class LeaderboardUpdate
{
    public string username;
    public float value;

    public LeaderboardUpdate(string newName, float newValue)
    {
        this.username = newName;
        this.value = newValue;
    }
}

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard instance;

    [SerializeField]
    private string getLeaderboardEndpointName;
    [SerializeField]
    private string getEntryEndpointName;
    [SerializeField]
    private string updateLeaderboardEndpointName;

    [SerializeField]
    private GameObject parentChatObject;

    [HideInInspector]
    public LeaderboardEntryObject[] entries;

    private Queue<LeaderboardUpdate> queuedUpdates;

    private bool readyToProcessUpdate = true;

    public LeaderboardData currentLeaderboard;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        this.entries = GetComponentsInChildren<LeaderboardEntryObject>(true);
        this.queuedUpdates = new Queue<LeaderboardUpdate>();

        StartCoroutine(this.RequestLeaderboard());
    }

    public LeaderboardData GetLatestLeaderboard()
    {
        return this.currentLeaderboard;
    }

    public LeaderboardEntryData GetTopPlayer()
    {
        if (this.currentLeaderboard != null)
        {
            return this.currentLeaderboard.entries[0];
        }
        else
        {
            return null;
        }
    }

    private IEnumerator RequestLeaderboard()
    {
        string fullURL = TwitchSecrets.ServerName + "/" + this.getLeaderboardEndpointName;

        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            this.currentLeaderboard = JsonUtility.FromJson<LeaderboardData>(www.downloadHandler.text);
            this.UpdateLeaderboardVisuals();
        }
    }

    public void UpdateLeaderboard(string username, float value)
    {
        this.queuedUpdates.Enqueue(new LeaderboardUpdate(username, value));        
    }

    private void FixedUpdate()
    {
        if (this.queuedUpdates.Count > 0 && this.readyToProcessUpdate == true)
        {
            this.ProcessUpdate(this.queuedUpdates.Dequeue());
        }
    }

    private void ProcessUpdate(LeaderboardUpdate updateValues)
    {
        this.readyToProcessUpdate = false;
        StartCoroutine(this.UpdateCurrentLeaderboardValue(updateValues));
    }

    private IEnumerator UpdateCurrentLeaderboardValue(LeaderboardUpdate updateValues)
    {
        string fullURL = TwitchSecrets.ServerName + "/" + this.getEntryEndpointName;

        WWWForm form = new WWWForm();
        form.AddField("username", updateValues.username);

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            LeaderboardEntryData currentData = JsonUtility.FromJson<LeaderboardEntryData>(www.downloadHandler.text);
            currentData.value += updateValues.value;

            StartCoroutine(SendUpdateLeaderboardRequest(currentData.username, currentData.value));
        }
    }

    private IEnumerator SendUpdateLeaderboardRequest(string username, float updatedValue)
    {
        string fullURL = TwitchSecrets.ServerName + "/" + this.updateLeaderboardEndpointName;

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("value", updatedValue.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            this.currentLeaderboard = JsonUtility.FromJson<LeaderboardData>(www.downloadHandler.text);
            this.UpdateLeaderboardVisuals();        
        }

        this.readyToProcessUpdate = true;
    }

    private void UpdateLeaderboardVisuals()
    {
        for (int i = 0; i < this.currentLeaderboard.entries.Count && i < this.entries.Length; i++)
        {
            this.entries[i].UpdateEntry(this.currentLeaderboard.entries[i].username, this.currentLeaderboard.entries[i].value);
        }        
    }
}
