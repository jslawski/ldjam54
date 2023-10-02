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
    private GameMap gameMap;

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
            menuImage.sprite = Resources.Load<Sprite>("MainMenu/NPP_mainmenu_blue");
        }

        switch (team)
        {
            case 1:
                menuImage.sprite = Resources.Load<Sprite>("MainMenu/NPP_mainmenu_yellow");
                break;
            case 2:
                menuImage.sprite = Resources.Load<Sprite>("MainMenu/NPP_mainmenu_red");
                break;
            case 3:
                menuImage.sprite = Resources.Load<Sprite>("MainMenu/NPP_mainmenu_blue");
                break;
            default:
                menuImage.sprite = Resources.Load<Sprite>("MainMenu/NPP_mainmenu_blue");
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
        while (true)
        {
            string fullURL = TwitchSecrets.ServerName + "/getScore.php";

            WWWForm form = new WWWForm();

            using (UnityWebRequest www = UnityWebRequest.Get(fullURL))
            {
                yield return www.SendWebRequest();

                Scoreboard currentScore = JsonUtility.FromJson<Scoreboard>(www.downloadHandler.text);

                this.blueScore.text = currentScore.team3Score.ToString();
                this.redScore.text = currentScore.team2Score.ToString();
                this.yellowScore.text = currentScore.team1Score.ToString();
            }
            
            yield return new WaitForSeconds(5.0f);

            //Update tilemap
            this.gameMap.LoadMap();

            yield return new WaitForSeconds(5.0f);
        }
    }
}
