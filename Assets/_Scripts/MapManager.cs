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
    
    public Color team1Color;
    public Color team2Color;
    public Color team3Color;

    [SerializeField]
    private Paintable paintableMap;
    [SerializeField]
    private Paintable paintableDelta;

    [SerializeField]
    private Texture2D paintableTexture;

    [SerializeField]
    private Material saveMaterial;
    private RenderTexture saveTexture;

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
        this.saveTexture = (RenderTexture)this.saveMaterial.GetTexture("_BaseMap");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            this.SaveLatestMap();
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            this.LoadLatestMap();
        }
    }
    
    private void OnDestroy()
    {     
    }
    #endregion

    #region Public Functions
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
        this.Cleanup();
        //StartCoroutine(this.UploadMap());
    }

    private IEnumerator UploadMap()
    {
        RenderTexture mapRendTex = this.paintableMap.GetSupportTexture();

        Texture2D savedMapTexture = new Texture2D(mapRendTex.width, mapRendTex.height, TextureFormat.RGBA32, false, true);
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

        this.Cleanup();
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

        File.WriteAllBytes(Application.dataPath + "/mapDataLoaded.png", loadedMapTexture.EncodeToPNG());

        Destroy(loadedMapTexture);
    }

    private void AppendNewDataToLoadedMap(byte[] data)
    {
        PaintManager.instance.DeltaPaint(this.paintableMap);
    }

    private void Cleanup()
    {
        RenderTexture rendTex = this.paintableMap.GetSupportTexture();

        //Update the texture displaying in game
        Texture2D savedMapTexture = new Texture2D(this.paintableMap.TEXTURE_SIZE, this.paintableMap.TEXTURE_SIZE, TextureFormat.RGBA32, 11, true);
        RenderTexture.active = rendTex;
        savedMapTexture.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0);
        savedMapTexture.Apply();
        RenderTexture.active = null;
        Graphics.CopyTexture(savedMapTexture, this.paintableTexture);

        File.WriteAllBytes(Application.dataPath + "/mapDataSaved.png", savedMapTexture.EncodeToPNG());

        this.paintableDelta.Clear();
        Destroy(savedMapTexture);
    }    
    #endregion
}
