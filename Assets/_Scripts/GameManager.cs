using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private GameObject playerPrefab;

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

    private void SpawnPlayer()
    {
        double offset = TimeZoneInfo.Local.BaseUtcOffset.TotalHours;

        GameObject playerObject;

        if (offset <= -8)
        {
            playerObject = Instantiate(this.playerPrefab, new Vector3(-5.0f, 0.0f, 0.0f), new Quaternion());
            playerObject.GetComponent<PlayerCharacter>().Setup();
        }
        else if (offset <= -7)
        {
            playerObject = Instantiate(this.playerPrefab, new Vector3(0.0f, 0.0f, -5.0f), new Quaternion());
            playerObject.GetComponent<PlayerCharacter>().Setup();
        }
        else
        {
            playerObject = Instantiate(this.playerPrefab, new Vector3(5.0f, 0.0f, 0.0f), new Quaternion());
            playerObject.GetComponent<PlayerCharacter>().Setup();
        }

        CameraFollow.instance.playerCharacter = playerObject.GetComponent<PlayerMovement>();
        CameraFollow.ReturnToFollow();
        CameraFollow.instance.BeginFollow();
    }

    public void StartHeartbeat()
    {
        StartCoroutine(this.Heartbeat());
    }

    private IEnumerator Heartbeat()
    {
        while (true)
        {            
            yield return new WaitForSeconds(10.0f);
            
            GameMap.instance.SaveLatestMap();
            
            yield return new WaitForSeconds(5.0f);

            GameMap.instance.LoadMap();
        }
    }

}
