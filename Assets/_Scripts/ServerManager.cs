using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    [SerializeField]
    private Transform ghostPlayerParent;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.Heartbeat());    
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator Heartbeat()
    {
        yield return new WaitForSeconds(10.0f);

        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            if (this.ghostPlayerParent.childCount > 0)
            {
                this.SaveMap();
            }
        }
    }

    private void SaveMap()
    {
        MapManager.instance.SaveLatestMap();
    }
}
