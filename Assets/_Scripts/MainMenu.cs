using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using CharacterCustomizer;
using TMPro;

public class Scoreboard
{
    public int team1Score;
    public int team2Score;
    public int team3Score;
}

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private FadePanelManager fadeManager;

    [SerializeField]
    private Image menuImage;

    [SerializeField]
    private TextMeshProUGUI blueScore;
    [SerializeField]
    private TextMeshProUGUI redScore;
    [SerializeField]
    private TextMeshProUGUI yellowScore;

    // Start is called before the first frame update
    void Start()
    {
        int team = PlayerPrefs.GetInt("team", -1);

        if (team == -1)
        {
            menuImage.sprite = Resources.Load<Sprite>("MainMenu/RoverUnder_mainmenu_blue");
        }

        switch (team)
        {
            case 1:
                menuImage.sprite = Resources.Load<Sprite>("MainMenu/RoverUnder_mainmenu_yellow");
                break;
            case 2:
                menuImage.sprite = Resources.Load<Sprite>("MainMenu/RoverUnder_mainmenu_red");
                break;
            case 3:
                menuImage.sprite = Resources.Load<Sprite>("MainMenu/RoverUnder_mainmenu_blue");
                break;
            default:
                menuImage.sprite = Resources.Load<Sprite>("MainMenu/RoverUnder_mainmenu_blue");
                break;
        }

        this.fadeManager.FadeFromBlack();

        this.StartCoroutine(this.RefreshScore());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.End))
        {
            this.ResetGame();
        }
    }

    private void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        this.fadeManager.OnFadeSequenceComplete += this.TransitionScenes;
        this.fadeManager.FadeToBlack();
    }

    private void TransitionScenes()
    {
        this.fadeManager.OnFadeSequenceComplete -= this.TransitionScenes;
        SceneManager.LoadScene("CharacterSelect");
    }

    private IEnumerator RefreshScore()
    {
        yield return null;

        while (true)
        {
            int[] scores = MapManager.instance.GetLatestScores();

            this.blueScore.text = scores[2].ToString();
            this.redScore.text = scores[1].ToString();
            this.yellowScore.text = scores[0].ToString();

            yield return new WaitForSeconds(1.0f);
        }
    }

    public void StartGame()
    {
        this.fadeManager.OnFadeSequenceComplete += this.TransitionToGame;
        this.fadeManager.FadeToBlack();
    }

    private void TransitionToGame()
    {
        this.fadeManager.OnFadeSequenceComplete -= this.TransitionToGame;
        SceneManager.LoadScene("GameplayScene");
    }

    public void EndGame()
    {
        this.fadeManager.OnFadeSequenceComplete += this.CloseGame;
        this.fadeManager.FadeToBlack();
    }

    private void CloseGame()
    {
        this.fadeManager.OnFadeSequenceComplete -= this.CloseGame;
        Application.Quit();
    }
}
