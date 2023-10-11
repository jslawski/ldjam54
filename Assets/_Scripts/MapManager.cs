using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using CharacterCustomizer;
using System.IO;
using System;
using UnityEngine.Rendering;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public Color[] teamColors;

    [SerializeField]
    private Paintable paintableMap;

    #region Unity Functions
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        this.LoadLatestMap();
    }
    #endregion

    #region Public Functions
    public void StartHeartbeat()
    {
        StartCoroutine(this.Heartbeat());
    }

    public void SaveLatestMap()
    {
        StartCoroutine(this.PreSaveLoadMap());
    }

    public void LoadLatestMap()
    {
        StartCoroutine(this.LoadMap());
    }
    #endregion


    #region Network Request Coroutines
    private IEnumerator Heartbeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            this.SaveLatestMap();
        }
    }
    
    //Load the latest map from the server
    private IEnumerator PreSaveLoadMap()
    {
        string fullURL = TwitchSecrets.ServerName + "/mapData.png";
        WWWForm form = new WWWForm();

        //Send request
        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            this.WriteLoadedMapToTexture(www.downloadHandler.data);
            this.AppendNewDataToLoadedMap(www.downloadHandler.data);
        }

        StartCoroutine(this.UploadMap());
    }

    private IEnumerator UploadMap()
    {
        RenderTexture mapRendTex = this.paintableMap.GetSupportTexture();

        Texture2D savedMapTexture = new Texture2D(mapRendTex.width, mapRendTex.height, TextureFormat.RGBA32, false, false);
        RenderTexture.active = mapRendTex;
        savedMapTexture.ReadPixels(new Rect(0, 0, mapRendTex.width, mapRendTex.height), 0, 0);
        savedMapTexture.Apply();
        RenderTexture.active = null;

        string fullURL = TwitchSecrets.ServerName + "/uploadMap.php";
        WWWForm form = new WWWForm();
        form.AddBinaryData("mapData", savedMapTexture.EncodeToPNG(), "mapData.png");
        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();
        }

        Destroy(savedMapTexture);

        this.FinishSave();
    }

    private IEnumerator LoadMap()
    {
        string fullURL = TwitchSecrets.ServerName + "/mapData.png";
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            this.WriteLoadedMapToTexture(www.downloadHandler.data);
        }
    }
    #endregion

    #region Helper Functions
    private void WriteLoadedMapToTexture(byte[] data)
    {
        //Initialize texture to load 
        //When initializing a texture meant to Blit into a RenderTexture, 
        //it must be done in linear color space (Or don't?  I dunno...)
        Texture2D loadedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        loadedMapTexture.LoadImage(data);
        loadedMapTexture.alphaIsTransparency = true;
        loadedMapTexture.Apply();

        RenderTexture supportTexture = this.paintableMap.GetSupportTexture();

        Graphics.Blit(loadedMapTexture, supportTexture);

        //File.WriteAllBytes(Application.dataPath + "/mapDataLoaded.png", loadedMapTexture.EncodeToPNG());

        Destroy(loadedMapTexture);
    }

    private void AppendNewDataToLoadedMap(byte[] data)
    {
        PaintManager.instance.DeltaPaint(this.paintableMap);
    }

    private void FinishSave()
    {
        RenderTexture rendTex = this.paintableMap.GetSupportTexture();

        //Update the texture displaying in game
        Texture2D savedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        RenderTexture.active = rendTex;
        savedMapTexture.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0);
        savedMapTexture.Apply();
        RenderTexture.active = null;

        Graphics.Blit(savedMapTexture, rendTex);

        //File.WriteAllBytes(Application.dataPath + "/mapDataSaved.png", savedMapTexture.EncodeToPNG());

        this.paintableMap.ClearDelta();
        Destroy(savedMapTexture);

        int[] teamScores = ScoreCalculator.instance.GetScores(this.paintableMap.GetSupportTexture());

        Debug.LogError(this.gameObject.name + "\n" +
                "Team1: " + teamScores[0] +  "\n" +
                "Team2: " + teamScores[1] +  "\n" +
                "Team3: " + teamScores[2]);
    }    


    #endregion
}
