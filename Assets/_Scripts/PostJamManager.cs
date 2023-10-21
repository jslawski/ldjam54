using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostJamManager : MonoBehaviour
{
    public static PostJamManager instance;

    public bool postJam = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        /*
        int cachedValue = PlayerPrefs.GetInt("postJam", -1);

        if (cachedValue > 0)
        {
            this.postJam = true;
        }
        else
        {
            this.postJam = false;
        }*/
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            this.postJam = !this.postJam;

            if (this.postJam == false)
            {
                GhostPlayerManager.instance.DisableGhostPlayers();
                PlayerPrefs.SetInt("postJam", 0);
            }
            else
            {
                GhostPlayerManager.instance.EnableGhostPlayers();
                PlayerPrefs.SetInt("postJam", 1);
            }
        }
    }
}
