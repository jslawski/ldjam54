using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Team team;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.team = (Team)PlayerPrefs.GetInt("team", -1);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.SetupGame();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            PlayerPrefs.DeleteKey("playerID");
        }
    }

    private void SetupGame()
    {
        GameMap.instance.LoadMap();
    }

    public void StartHeartbeat()
    {
        StartCoroutine(this.SaveHeartbeat());
        StartCoroutine(this.LoadHeartbeat());
    }

    public IEnumerator SaveHeartbeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            GameMap.instance.SaveLatestMap();
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

}
