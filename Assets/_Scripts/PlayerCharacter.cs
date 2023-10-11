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

    public int brushSize;

    private PaintBrush paintbrush;

    private Rigidbody playerRigidbody;

    public SinglePlayerData playerData;

    // Start is called before the first frame update
    public void Setup()
    {
        this.playerRigidbody = GetComponent<Rigidbody>();

        this.playerData = new SinglePlayerData();
        this.playerData.playerID = PlayerPrefs.GetInt("playerID", -1);
        this.playerData.data.team = PlayerPrefs.GetInt("team", -1);

        this.playerTeam = GameManager.instance.team;
        
        this.SetupPlayerModel();

        this.BeginCameraFollow();

        GhostPlayerManager.instance.currentPlayer = playerData;
    }

    private void SetupPlayerModel()
    {
        GameObject robotModel;

        this.paintbrush = GetComponent<PaintBrush>();
        this.paintbrush.paintCanvas = GameObject.Find("DrawPlane").GetComponent<Paintable>();

        switch (this.playerTeam)
        {
            case Team.Team1:
                robotModel = this.robotModels[0];
                this.paintbrush.brushColor = MapManager.instance.teamColors[0];
                break;
            case Team.Team2:
                this.paintbrush.brushColor = MapManager.instance.teamColors[1];
                robotModel = this.robotModels[1];
                break;
            case Team.Team3:
                this.paintbrush.brushColor = MapManager.instance.teamColors[2];
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

    private void UpdatePlayerData()
    {
        this.playerData.data.posX = this.transform.position.x;
        this.playerData.data.posY = this.transform.position.z;
        this.playerData.data.rot = this.transform.rotation.y;
        this.playerData.data.speed = this.playerRigidbody.velocity.magnitude;
        this.playerData.data.team = (int)this.playerTeam;
    }

    // Update is called once per frame
    private void Update()
    {
        if (this.playerTeam == Team.None)
        {
            return;
        }

        this.UpdatePlayerData();
        /*
        if (GameManager.instance.postJam == false)
        {
            GameMap.instance.UpdateMap(this);
        }
        */
    }
}
