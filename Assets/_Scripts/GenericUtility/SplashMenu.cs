using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashMenu : MonoBehaviour
{
    [SerializeField]
    private FadePanelManager fadePanel;

    private bool loading = false;

    private void Awake()
    {
        this.fadePanel.OnFadeSequenceComplete += this.DisplaySplashScreen;
        this.fadePanel.FadeFromBlack();

        Application.targetFrameRate = 60;
    }

    private void DisplaySplashScreen()
    {
        StartCoroutine(this.DisplayCoroutine());
    }

    private IEnumerator DisplayCoroutine()
    {
        yield return new WaitForSeconds(3.0f);

        this.fadePanel.OnFadeSequenceComplete += this.LoadMainMenu;
        this.fadePanel.FadeToBlack();
    }

    private void LoadMainMenu()
    {
        if (PlayerPrefs.GetInt("playerID", -1) != -1)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("CharacterSelect");
        }
    }
}
