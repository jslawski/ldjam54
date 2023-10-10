using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCameraFollow : MonoBehaviour
{
    public PlayerMovement player;
    
    // Start is called before the first frame update
    void Start()
    {
        CameraFollow.instance.playerCharacter = player;
        CameraFollow.instance.BeginFollow();
    }

}
