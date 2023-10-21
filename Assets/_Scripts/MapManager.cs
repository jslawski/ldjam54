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

    public bool isServer = false;
    
    private UnityWebRequestAsyncOperation uploadWebRequest;
    private UnityWebRequestAsyncOperation loadWebRequest;

    Action<AsyncOperation> uploadHandler;
    Action<AsyncOperation> loadHandler;

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
        #if UNITY_WEBGL
        this.StartHeartbeat();
        #endif
        this.LoadLatestMap();
    }

    private void OnDestroy()
    {
        this.StopAllCoroutines();
    }
#endregion

#region Public Functions
    private void StartHeartbeat()
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
        //We probably don't need to pre-load if just one instance is doing all of the saving!
        RenderTexture support = this.paintableMap.GetSupportTexture();

        Texture2D loadedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        RenderTexture.active = support;
        loadedMapTexture.ReadPixels(new Rect(0, 0, support.width, support.height), 0, 0);
        loadedMapTexture.Apply();
        RenderTexture.active = null;

        string fullURL = TwitchSecrets.ServerName + "/uploadMapNew.php";
        WWWForm form = new WWWForm();
        form.AddBinaryData("mapData", loadedMapTexture.EncodeToPNG(), "mapDataNew.png");

        UnityWebRequest www = UnityWebRequest.Post(fullURL, form);

        this.uploadWebRequest = www.SendWebRequest();
        this.uploadHandler = (data) => { this.UploadCompleted(www); };
        this.uploadWebRequest.completed -= this.uploadHandler;
        this.uploadWebRequest.completed += this.uploadHandler;


        //StartCoroutine(this.PreSaveLoadMap());
        Texture2D.Destroy(loadedMapTexture);
    }

    public void LoadLatestMap()
    {
        string fullURL = TwitchSecrets.ServerName + "/getMap.php";
        WWWForm form = new WWWForm();

        UnityWebRequest www = UnityWebRequest.Post(fullURL, form);

        this.loadWebRequest = www.SendWebRequest();
        this.loadHandler = (data) => { this.LoadCompleted(www); };
        this.loadWebRequest.completed -= this.loadHandler;
        this.loadWebRequest.completed += this.loadHandler;

        //StartCoroutine(this.LoadMap());
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
        #elif UNITY_STANDALONE_WIN
        return ScoreCalculator.instance.GetScores(this.paintableMap.GetSupportTexture());
        #endif
    }
    #endregion

    #region NetworkRequestHandlers
    private void PreSaveLoadCompleted(UnityWebRequest www)
    {
        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Pre-Save Load Error: " + www.error);
        }
        else
        {
            this.WriteLoadedMapToTexture(www.downloadHandler.data);
        }
    }

    private void UploadCompleted(UnityWebRequest www)
    {
        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Upload Error: " + www.error);
        }
        else
        {
            this.FinishSave();
        }

        www.Dispose();
    }

    private void LoadCompleted(UnityWebRequest www)
    {
        //Debug.LogError("I'm Here.  Result: " + www.downloadHandler.data.ToString());

        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Load Error: " + www.error);
        }
        else
        {
            this.WriteLoadedMapToTexture(www.downloadHandler.data);
        }

        www.Dispose();
    }

    #endregion

    private IEnumerator Heartbeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(10.0f);
            this.LoadLatestMap();
        }
    }

    /*
    #region Network Request Coroutines
    
    
    //Load the latest map from the server
    private IEnumerator PreSaveLoadMap()
    {
        string fullURL = TwitchSecrets.ServerName + "/getMap.php";
        WWWForm form = new WWWForm();

        //Send request
        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("ERROR: " + www.error);
            }

            ///Debug.LogError("Data Size: " + www.downloadedBytes);
            ///Debug.LogError("Data: " + www.downloadHandler.data.ToString());
            ///Debug.LogError("Data Text: " + www.downloadHandler.text);

            this.WriteLoadedMapToTexture(www.downloadHandler.data, true);
        }

        //this.AppendNewDataToLoadedMap();
        StartCoroutine(this.UploadMap());
    }
    
    private IEnumerator UploadMap()
    {
        RenderTexture support = this.paintableMap.GetSupportTexture();

        Texture2D loadedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        RenderTexture.active = support;
        loadedMapTexture.ReadPixels(new Rect(0, 0, support.width, support.height), 0, 0);
        loadedMapTexture.Apply();
        RenderTexture.active = null;
        
        if (Application.isEditor == true)
        {
            File.WriteAllBytes(Application.dataPath + "/05_mapToUpload.png", this.loadedMapTexture.EncodeToPNG());
        }
        
        string fullURL = TwitchSecrets.ServerName + "/uploadMapNew.php";
        WWWForm form = new WWWForm();
        form.AddBinaryData("mapData", loadedMapTexture.EncodeToPNG(), "mapDataNew.png");
        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();
        }

        this.FinishSave();

        Texture2D.Destroy(loadedMapTexture);
    }

    private IEnumerator LoadMap()
    {
        string fullURL = TwitchSecrets.ServerName + "/getMap.php";
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            UnityWebRequestAsyncOperation asyncOp = new UnityWebRequestAsyncOperation();
            asyncOp.webRequest = www;

                //yield return www.SendWebRequest();            
            this.WriteLoadedMapToTexture(www.downloadHandler.data, true);
        }
        
    }
#endregion
*/
    #region Helper Functions    
    private void WriteLoadedMapToTexture(byte[] data)
    {
        //Initialize texture to load 
        //When initializing a texture meant to Blit into a RenderTexture, 
        //it must be done in linear color space (Or don't?  I dunno...)

        Texture2D loadedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        loadedMapTexture.LoadImage(data);
        loadedMapTexture.Apply();

        PaintManager.instance.OverwriteCanvas(this.paintableMap, loadedMapTexture);
        
        Texture2D.Destroy(loadedMapTexture);
    }

    private void FinishSave()
    {
        this.paintableMap.ClearDelta();
        
        if (Application.isEditor == true)
        {
            ScoreCalculator.instance.SendScoreUpdate(ScoreCalculator.instance.GetScores(this.paintableMap.GetSupportTexture()));
        }
#if UNITY_STANDALONE_WIN
        ScoreCalculator.instance.SendScoreUpdate(ScoreCalculator.instance.GetScores(this.paintableMap.GetSupportTexture()));
#endif

    }
    /*
    private void SaveMapStages()
    {
        RenderTexture mask = this.paintableMap.GetMaskTexture();
        RenderTexture support = this.paintableMap.GetSupportTexture();
        RenderTexture delta = this.paintableMap.GetDeltaTexture();
        RenderTexture temp = this.paintableMap.GetTempTexture();

        File.WriteAllBytes(Application.dataPath + "/00_mapDataLoaded.png", this.loadedMapTexture.EncodeToPNG());

        this.loadedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        RenderTexture.active = mask;
        this.loadedMapTexture.ReadPixels(new Rect(0, 0, mask.width, mask.height), 0, 0);
        this.loadedMapTexture.Apply();
        RenderTexture.active = null;
        File.WriteAllBytes(Application.dataPath + "/01_mask.png", this.loadedMapTexture.EncodeToPNG());

        this.loadedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        RenderTexture.active = delta;
        this.loadedMapTexture.ReadPixels(new Rect(0, 0, delta.width, delta.height), 0, 0);
        this.loadedMapTexture.Apply();
        RenderTexture.active = null;
        File.WriteAllBytes(Application.dataPath + "/02_delta.png", this.loadedMapTexture.EncodeToPNG());

        this.loadedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        RenderTexture.active = temp;
        this.loadedMapTexture.ReadPixels(new Rect(0, 0, temp.width, temp.height), 0, 0);
        this.loadedMapTexture.Apply();
        RenderTexture.active = null;
        File.WriteAllBytes(Application.dataPath + "/03_temp.png", this.loadedMapTexture.EncodeToPNG());

        this.loadedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, false);
        RenderTexture.active = support;
        this.loadedMapTexture.ReadPixels(new Rect(0, 0, support.width, support.height), 0, 0);
        this.loadedMapTexture.Apply();
        RenderTexture.active = null;
        File.WriteAllBytes(Application.dataPath + "/04_support.png", this.loadedMapTexture.EncodeToPNG());
    }
*/
#endregion
}
