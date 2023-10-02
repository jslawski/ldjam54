using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.SetupGame();
    }

    private void SetupGame()
    {
        GameMap.instance.LoadMap();
    }

    public void StartHeartbeat()
    {
        StartCoroutine(this.Heartbeat());
    }

    private IEnumerator Heartbeat()
    {
        while (true)
        {            
            yield return new WaitForSeconds(30.0f);
            
            GameMap.instance.SaveLatestMap();
            
            yield return new WaitForSeconds(5.0f);

            GameMap.instance.LoadMap();
        }
    }

}
