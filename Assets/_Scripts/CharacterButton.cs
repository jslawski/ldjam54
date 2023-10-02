using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public Button buttonObject;

    public Team team;

    // Start is called before the first frame update
    void Start()
    {
        this.buttonObject = GetComponent<Button>();
    }
}
