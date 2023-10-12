using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using CharacterCustomizer;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Team team;

    [SerializeField]
    private FadePanelManager fadeManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.team = (Team)PlayerPrefs.GetInt("team", -1);

        this.fadeManager.FadeFromBlack();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            this.fadeManager.OnFadeSequenceComplete += this.ReturnToMenu;
            this.fadeManager.FadeToBlack();
        }
    }

    private void ReturnToMenu()
    {
        this.fadeManager.OnFadeSequenceComplete -= this.ReturnToMenu;
        SceneManager.LoadScene("MainMenu");
    }

    public void StartHeartbeat()
    {
        MapManager.instance.StartHeartbeat();
        /*
        if (this.postJam == false)
        {
            StartCoroutine(this.SaveHeartbeat());
            StartCoroutine(this.LoadHeartbeat());
        }
        else
        {
            MapManager.instance.StartHeartbeat();
        }
        */
    }

    public void StopHeartbeat()
    {
        MapManager.instance.StopHeartbeat();
        /*
        if (this.postJam == false)
        {
            StartCoroutine(this.SaveHeartbeat());
            StartCoroutine(this.LoadHeartbeat());
        }
        else
        {
            MapManager.instance.StartHeartbeat();
        }
        */
    }

    /*
    public IEnumerator SaveHeartbeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            //GameMap.instance.SaveLatestMap();
        }
    }

    private IEnumerator LoadHeartbeat()
    {
        while (true)
        {                        
            yield return new WaitForSeconds(30.0f);
            GameMap.instance.LoadMap();
        }
    }
    */
}
