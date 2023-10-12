using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PostJamCanvas : MonoBehaviour
{
    private TextMeshProUGUI instructions;
    
    // Start is called before the first frame update
    void Start()
    {
        this.instructions = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PostJamManager.instance.postJam == true)
        {
            instructions.text = "Press 'P' to disable 'Post-Jam'";
        }
        else
        {
            instructions.text = "Press 'P' to enable 'Post-Jam'\n(Real-Time Players)";
        }
    }
}
