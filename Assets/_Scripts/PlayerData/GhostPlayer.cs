using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    [SerializeField]
    private GameObject[] robotModels;

    public SinglePlayerData playerData;

    private Transform playerTransform;

    private PaintBrush paintbrush;

    public void Setup(SinglePlayerData playerData)
    {
        this.playerData = playerData;

        this.playerTransform = GetComponent<Transform>();

        this.SetupPlayerModel();
    }

    private void SetupPlayerModel()
    {
        GameObject robotModel;

        this.paintbrush = GetComponent<PaintBrush>();
        this.paintbrush.paintCanvas = GameObject.Find("DrawPlane").GetComponent<Paintable>();

        switch ((Team)this.playerData.team)
        {
            case Team.Team1:
                robotModel = this.robotModels[0];
                this.paintbrush.brushColor = GameManager.GetScaledColor(253, 214, 137, 255);
                break;
            case Team.Team2:
                robotModel = this.robotModels[1];
                this.paintbrush.brushColor = GameManager.GetScaledColor(255, 87, 130, 255);
                break;
            case Team.Team3:
                robotModel = this.robotModels[2];
                this.paintbrush.brushColor = GameManager.GetScaledColor(82, 203, 255, 255);
                break;
            default:
                Debug.LogError("Unknown Team: " + this.playerData.team);
                robotModel = this.robotModels[0];
                break;
        }

        Instantiate(robotModel, this.transform);
    }

    public void UpdatePlayerData(SinglePlayerData playerData)
    {
        this.playerData = playerData;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 targetPosition = new Vector3(this.playerData.posX, 0.0f, this.playerData.posY);
        this.playerTransform.position = Vector3.Lerp(this.playerTransform.position, targetPosition, this.playerData.speed * Time.fixedDeltaTime);
        this.playerTransform.rotation = Quaternion.Euler(0.0f, this.playerData.rot, 0.0f);
    }
}
