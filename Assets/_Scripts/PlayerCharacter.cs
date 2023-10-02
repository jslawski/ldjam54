using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using CharacterCustomizer;

public enum Team { None, Team1, Team2, Team3 };

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private GameObject[] robotModels;

    public Team playerTeam = Team.None;

    public int brushSize = 1;

    // Start is called before the first frame update
    public void Setup()
    {
        this.playerTeam = GameManager.instance.team;

        this.SetupPlayerModel();

        this.BeginCameraFollow();
        
    }

    private void SetupPlayerModel()
    {
        GameObject robotModel;

        switch (this.playerTeam)
        {
            case Team.Team1:
                robotModel = this.robotModels[0];                
                break;
            case Team.Team2:
                robotModel = this.robotModels[1];
                break;
            case Team.Team3:
                robotModel = this.robotModels[2];
                break;
            default:
                Debug.LogError("Unknown Team: " + this.playerTeam);
                robotModel = this.robotModels[0];
                break;                
        }

        Instantiate(robotModel, this.transform);
    }

    public void BeginCameraFollow()
    {
        CameraFollow.instance.playerCharacter = this.GetComponent<PlayerMovement>();
        CameraFollow.ReturnToFollow();
        CameraFollow.instance.BeginFollow();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.playerTeam == Team.None)
        {
            return;
        }

        GameMap.instance.UpdateMap(this);
    }
}
