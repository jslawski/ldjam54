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

        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;
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
        SceneManager.LoadScene("MenuScene");
    }

    public void PlayButtonClicked()
    {
        if (this.loading == false)
        {
            this.fadePanel.OnFadeSequenceComplete += this.LoadLevel;
            this.fadePanel.FadeToBlack();
            this.loading = true;
        }
    }

    public void ExitButtonClicked()
    {
        Application.Quit();
    }

    private void LoadLevel()
    {
        this.fadePanel.OnFadeSequenceComplete -= this.LoadLevel;
        SceneManager.LoadScene("GameplayScene");
    }
}
