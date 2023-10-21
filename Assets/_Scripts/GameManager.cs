using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using CharacterCustomizer;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Team team;

    [SerializeField]
    private FadePanelManager fadeManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.team = (Team)PlayerPrefs.GetInt("team", -1);

        this.fadeManager.FadeFromBlack();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            this.fadeManager.OnFadeSequenceComplete += this.ReturnToMenu;
            this.fadeManager.FadeToBlack();
        }
    }

    private void ReturnToMenu()
    {
        this.fadeManager.OnFadeSequenceComplete -= this.ReturnToMenu;
        SceneManager.LoadScene("MainMenu");
    }
}
