using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using CharacterCustomizer;

public class ScoreCalculator : MonoBehaviour
{
    public static ScoreCalculator instance;

    public ComputeShader scoreShader;
    private ComputeBuffer scoreBuffer;
    private int[] result;
    private int kernalMain;
    private int kernalInit;

    private int[] cachedScores;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.cachedScores = new int[3];
    }

    public int[] GetScores(RenderTexture mapTexture)
    {
        this.kernalMain = this.scoreShader.FindKernel("CSMain");
        this.kernalInit = this.scoreShader.FindKernel("CSInit");
        this.scoreBuffer = new ComputeBuffer(1, sizeof(int) * 4);
        this.result = new int[4];

        this.scoreShader.SetTexture(this.kernalMain, "InputImage", mapTexture);
        this.scoreShader.SetTexture(this.kernalInit, "InputImage", mapTexture);
        this.scoreShader.SetBuffer(this.kernalMain, "ResultBuffer", this.scoreBuffer);
        this.scoreShader.SetBuffer(this.kernalInit, "ResultBuffer", this.scoreBuffer);

        this.scoreShader.Dispatch(this.kernalInit, 1, 1, 1);
        this.scoreShader.Dispatch(this.kernalMain, mapTexture.width / 8, mapTexture.height / 8, 1);

        this.scoreBuffer.GetData(result);

        this.scoreBuffer.Release();
        this.scoreBuffer = null;

        this.cachedScores[0] = result[0];
        this.cachedScores[1] = result[1];
        this.cachedScores[2] = result[2];

        return result;
    }

    public void SendScoreUpdate(int[] scores)
    {
        StartCoroutine(this.SendScoreUpdateRequest(scores));
    }

    private IEnumerator SendScoreUpdateRequest(int[] scores)
    {
        string fullURL = TwitchSecrets.ServerName + "/updateLatestScores.php";
        WWWForm form = new WWWForm();
        form.AddField("team1Score", scores[0]);
        form.AddField("team2Score", scores[1]);
        form.AddField("team3Score", scores[2]);

        //Send request
        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();
        }
    }

    public void GetLatestScores()
    {
        StartCoroutine(this.GetLatestScoresRequest());
    }

    private IEnumerator GetLatestScoresRequest()
    {
        string fullURL = TwitchSecrets.ServerName + "/getLatestScores.php";
        WWWForm form = new WWWForm();

        //Send request
        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            Scoreboard currentScores = JsonUtility.FromJson<Scoreboard>(www.downloadHandler.text);
            this.cachedScores[0] = currentScores.team1Score;
            this.cachedScores[1] = currentScores.team2Score;
            this.cachedScores[2] = currentScores.team3Score;
        }
    }

    public int[] GetCachedScores()
    {
        return this.cachedScores;
    }
}
