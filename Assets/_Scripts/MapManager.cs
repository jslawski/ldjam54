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

    private Paintable paintableMap;

    private Coroutine heartbeatCoroutine;

    #region Unity Functions
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.paintableMap = GetComponentInChildren<Paintable>();
    }

    private void Start()
    {
        this.LoadLatestMap();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            this.LoadLatestMap();
        }
    }

    private void OnDestroy()
    {
        this.StopAllCoroutines();
    }
    #endregion

    #region Public Functions
    public void StartHeartbeat()
    {
        this.heartbeatCoroutine = StartCoroutine(this.Heartbeat());
    }

    public void StopHeartbeat()
    {
        if (this.heartbeatCoroutine != null)
        {
            StopCoroutine(this.heartbeatCoroutine);
        }
    }

    public void SaveLatestMap()
    {
        StartCoroutine(this.PreSaveLoadMap());
    }

    public void LoadLatestMap()
    {
        StartCoroutine(this.LoadMap());
    }

    public int[] GetLatestScores()
    {        
        if (Application.isEditor)
        {
            return ScoreCalculator.instance.GetScores(this.paintableMap.GetSupportTexture());
        }
        
#if UNITY_WEBGL
        ScoreCalculator.instance.GetLatestScores();
        return ScoreCalculator.instance.GetCachedScores();
#elif UNITY_WINDOWS
        return ScoreCalculator.instance.GetScores(this.paintableMap.GetSupportTexture());
#endif
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

            this.WriteLoadedMapToTexture(www.downloadHandler.data, false);            
        }

        this.AppendNewDataToLoadedMap();
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

            this.WriteLoadedMapToTexture(www.downloadHandler.data, true);
        }
    }
#endregion

#region Helper Functions
    private void WriteLoadedMapToTexture(byte[] data, bool shouldDraw = false)
    {        
        //Initialize texture to load 
        //When initializing a texture meant to Blit into a RenderTexture, 
        //it must be done in linear color space (Or don't?  I dunno...)
        Texture2D loadedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        loadedMapTexture.LoadImage(data);
        //loadedMapTexture.alphaIsTransparency = true;
        loadedMapTexture.Apply();

        //We maybe don't need to pass shouldDraw here
        //Just use it in a conditional statement to determine whether or not we Blit
        PaintManager.instance.Blit(loadedMapTexture, this.paintableMap, shouldDraw);

        File.WriteAllBytes(Application.dataPath + "/mapDataLoaded.png", loadedMapTexture.EncodeToPNG());

        Destroy(loadedMapTexture);
    }

    private void AppendNewDataToLoadedMap()
    {
        //Make loadedMapTexutre a global variable, send it as a parameter in this function
        //Set textureID to be loadedMapTexture, NOT support
        //Blit onto the support texture

        PaintManager.instance.DeltaPaint(this.paintableMap);
    }

    private void FinishSave()
    {
        this.paintableMap.ClearDelta();
        
        RenderTexture rendTex = this.paintableMap.GetSupportTexture();

        //Update the texture displaying in game
        
        Texture2D savedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        RenderTexture.active = rendTex;
        savedMapTexture.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0);
        savedMapTexture.Apply();
        RenderTexture.active = null;

        PaintManager.instance.Blit(savedMapTexture, this.paintableMap, true);

        File.WriteAllBytes(Application.dataPath + "/mapDataSaved.png", savedMapTexture.EncodeToPNG());

        ScoreCalculator.instance.SendScoreUpdate(ScoreCalculator.instance.GetScores(this.paintableMap.GetSupportTexture()));

        Destroy(savedMapTexture);
    }    


#endregion
}
