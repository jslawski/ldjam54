using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    public SpawnPoint associatedSpawn;

    public GameObject spawnPointParent;

    private SpawnPoint[] spawnPoints;
    
    private void Start()
    {
        this.spawnPoints = GetComponentsInChildren<SpawnPoint>();

        StartCoroutine(this.WaitForSetup());
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
