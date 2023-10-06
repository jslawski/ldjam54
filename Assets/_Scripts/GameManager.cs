using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Team team;

    [SerializeField]
    private FadePanelManager fadeManager;

    [SerializeField]
    private Paintable paintableMap;

    public bool postJam = true;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.team = (Team)PlayerPrefs.GetInt("team", -1);

        this.fadeManager.OnFadeSequenceComplete += this.SetupGame;
        this.fadeManager.FadeFromBlack();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.SetupGame();
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

    private void SetupGame()
    {
        this.fadeManager.OnFadeSequenceComplete -= this.SetupGame;
        GameMap.instance.LoadMap();
    }

    public void StartHeartbeat()
    {
        if (this.postJam == false)
        {
            StartCoroutine(this.SaveHeartbeat());
            StartCoroutine(this.LoadHeartbeat());
        }
    }

    public IEnumerator SaveHeartbeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            GameMap.instance.SaveLatestMap();                        
        }
    }

    private IEnumerator LoadHeartbeat()
    {
        while (true)
        {                        
            yield return new WaitForSeconds(30.0f);
            GameMap.instance.LoadMap();
        }
    }

    public static Color GetScaledColor(float r, float g, float b, float a)
    {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
    }
}
