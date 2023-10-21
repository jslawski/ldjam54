using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using CharacterCustomizer;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField]
    private CharacterButton[] buttons;    

    public int selectedIndex = -1;    

    [SerializeField]
    private GameObject characterButtonsParent;

    [SerializeField]
    private FadePanelManager fadeManager;

    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private GameObject errorMessage;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isEditor)
        {
            PlayerPrefs.SetInt("playerID", -1);
            PlayerPrefs.SetInt("team", -1);
        }

        if (PlayerPrefs.GetInt("playerID", -1) != -1)
        {
            this.characterButtonsParent.SetActive(false);
            StartCoroutine(this.RequestTeam());
        }
        else
        {
            this.fadeManager.FadeFromBlack();
        }
    }

    public void ConfirmButtonPressed()
    {
        if (this.inputField.text.Length <= 0)
        {
            return;
        }

        if (this.inputField.text.All(Char.IsLetterOrDigit) == false)
        {
            this.errorMessage.SetActive(true);
        }
        else
        {
            this.errorMessage.SetActive(false);

            PlayerPrefs.SetString("playerName", this.inputField.text);

            this.fadeManager.OnFadeSequenceComplete += this.DisplayCharacterSelect;
            this.fadeManager.FadeToBlack();
        }
    }

    private void DisplayCharacterSelect()
    {
        this.fadeManager.OnFadeSequenceComplete -= this.DisplayCharacterSelect;
        this.characterButtonsParent.SetActive(true);
        this.inputField.gameObject.SetActive(false);
        this.fadeManager.FadeFromBlack();
    }

    public void SelectButton(int index)
    {
        this.selectedIndex = index;

        for (int i = 0; i < this.buttons.Length; i++)
        {
            if (i == this.selectedIndex)
            {
                this.buttons[i].buttonObject.interactable = false;
            }
        }

        StartCoroutine(this.ConfirmTeam());
    }
    
    private IEnumerator ConfirmTeam()
    {
        string fullURL = TwitchSecrets.ServerName + "/setPlayerTeam.php";

        int playerID = PlayerPrefs.GetInt("playerID", -1);

        if (playerID == -1)
        {
            playerID = UnityEngine.Random.Range(123456, 987654);
            PlayerPrefs.SetInt("playerID", playerID);
        }

        WWWForm form = new WWWForm();
        form.AddField("playerName", PlayerPrefs.GetString("playerName"));
        form.AddField("playerID", playerID);
        form.AddField("team", (int)this.buttons[this.selectedIndex].team);

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();            
        }

        PlayerPrefs.SetInt("team", (int)this.buttons[this.selectedIndex].team);

        this.FadeToMenu();
    }

    private IEnumerator RequestTeam()
    {
        string fullURL = TwitchSecrets.ServerName + "/getPlayerTeam.php";

        int playerID = PlayerPrefs.GetInt("playerID", -1);

        WWWForm form = new WWWForm();
        form.AddField("playerID", playerID);

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();

            PlayerPrefs.SetInt("team", int.Parse(www.downloadHandler.text));            
        }

        this.FadeToMenu();
    }

    private void FadeToMenu()
    {
        this.fadeManager.OnFadeSequenceComplete += this.TransitionToMainMenu;
        this.fadeManager.FadeToBlack();
    }

    private void TransitionToMainMenu()
    {
        this.fadeManager.OnFadeSequenceComplete -= this.TransitionToMainMenu;
        SceneManager.LoadScene("MainMenu");
    }
}
