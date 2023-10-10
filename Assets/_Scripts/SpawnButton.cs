using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    public SpawnPoint associatedSpawn;

    public GameObject spawnPointParent;

    private SpawnPoint[] spawnPoints;

    [SerializeField]
    private Image buttonImage;

    private Color buttonColor;

    private Button buttonObject;

    private void Start()
    {
        this.spawnPoints = GetComponentsInChildren<SpawnPoint>();

        this.buttonObject = GetComponent<Button>();
        this.buttonObject.interactable = false;

        StartCoroutine(this.WaitForSetup());
    }

    private void SetButtonColor()
    {
        if (this.associatedSpawn.spawnID == 0 || this.associatedSpawn.owner == Team.None)
        {
            return;
        }

        switch (this.associatedSpawn.owner)
        {
            case Team.Team1:
                this.buttonColor = MapManager.instance.team1Color;
                break;
            case Team.Team2:
                this.buttonColor = MapManager.instance.team2Color;
                break;
            case Team.Team3:
                this.buttonColor = MapManager.instance.team3Color;
                break;
            default:
                Debug.LogError("Unknown team: " + this.associatedSpawn.owner);
                break;
        }

        this.buttonObject.image.color = this.buttonColor;        
    }

    private IEnumerator WaitForSetup()
    {
        while (this.ReadyToEvaluate() == false)
        {
            yield return null;
        }

        StartCoroutine(this.Evaluate());
    }

    private bool ReadyToEvaluate()
    {
        for (int i = 0; i < this.spawnPoints.Length; i++)
        {
            if (this.spawnPoints[i].isSetup == false)
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerator Evaluate()
    {
        while (true)
        {
            if (associatedSpawn.owner == GameManager.instance.team || associatedSpawn.spawnID == 0)
            {
                GetComponent<Button>().interactable = true;
            }
            else
            {
                GetComponent<Button>().interactable = false;
            }

            this.SetButtonColor();

            yield return null;
        }
    }

    public void SpawnButtonClicked()
    {
        StopAllCoroutines();
        this.associatedSpawn.SpawnPlayerCharacter();
        this.transform.parent.gameObject.SetActive(false);
    }
}
