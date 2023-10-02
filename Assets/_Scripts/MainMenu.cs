using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private FadePanelManager fadeManager;

    [SerializeField]
    private Image menuImage;

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
}
