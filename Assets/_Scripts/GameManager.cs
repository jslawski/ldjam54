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

        this.SpawnPlayer();

        StartCoroutine(this.Heartbeat());
    }

    private void SpawnPlayer()
    {
        double offset = TimeZoneInfo.Local.BaseUtcOffset.TotalHours;

        GameObject playerObject;

        if (offset <= -8)
        {
            playerObject = Instantiate(this.playerPrefab, new Vector3(-5.0f, 0.0f, 0.0f), new Quaternion());
            playerObject.GetComponent<PlayerCharacter>().Setup(Team.Team1);
        }
        else if (offset <= -7)
        {
            playerObject = Instantiate(this.playerPrefab, new Vector3(0.0f, 0.0f, -5.0f), new Quaternion());
            playerObject.GetComponent<PlayerCharacter>().Setup(Team.Team2);
        }
        else
        {
            playerObject = Instantiate(this.playerPrefab, new Vector3(5.0f, 0.0f, 0.0f), new Quaternion());
            playerObject.GetComponent<PlayerCharacter>().Setup(Team.Team3);
        }

        CameraFollow.instance.playerCharacter = playerObject.GetComponent<PlayerMovement>();
        CameraFollow.ReturnToFollow();
        CameraFollow.instance.BeginFollow();
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
