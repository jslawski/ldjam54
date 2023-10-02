using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;

    private void Start()
    {
        this.SpawnPlayerCharacter();
    }

    public void SpawnPlayerCharacter()
    {
        GameObject playerObject = Instantiate(this.playerPrefab, this.transform.position, new Quaternion());
        playerObject.GetComponent<PlayerCharacter>().Setup();

        GameManager.instance.StartHeartbeat();
    }
}
