using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using CharacterCustomizer;
using TMPro;

public enum Team { None, Team1, Team2, Team3 };

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    protected GameObject[] robotModels;

    public Team playerTeam = Team.None;

    public int brushSize;

    protected PaintBrush paintbrush;

    private Rigidbody playerRigidbody;

    public SinglePlayerData playerData;

    [SerializeField]
    protected Transform avatarParent;

    [SerializeField]
    protected TextMeshProUGUI nametag;

    // Start is called before the first frame update
    public void Setup()
    {
        this.playerRigidbody = GetComponent<Rigidbody>();

        this.playerData = new SinglePlayerData();
        this.playerData.playerID = PlayerPrefs.GetInt("playerID", -1);
        this.playerData.data.team = PlayerPrefs.GetInt("team", -1);
        this.playerData.playerName = PlayerPrefs.GetString("playerName");
        
        this.playerTeam = GameManager.instance.team;
        
        this.SetupPlayerModel();

        this.SetupNametag();

        this.BeginCameraFollow();

        GhostPlayerManager.instance.currentPlayer = playerData;
    }

    protected void SetupNametag()
    {
        this.nametag.text = this.playerData.playerName;
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

        Instantiate(robotModel, this.avatarParent);
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
        this.playerData.data.rot = this.transform.rotation.eulerAngles.y;        
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
