using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointCollider : MonoBehaviour
{
    public Team owner = Team.None;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.owner = other.GetComponent<PlayerCharacter>().playerTeam;
        }
    }
}
