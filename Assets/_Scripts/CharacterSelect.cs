using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using CharacterCustomizer;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField]
    private CharacterButton[] buttons;

    [SerializeField]
    private Button confirmButton;

    public int selectedIndex = -1;

    private Canvas canvasObject;

    [SerializeField]
    private GameObject spawnButtonsParent;
    [SerializeField]
    private GameObject characterButtonsParent;

    // Start is called before the first frame update
    void Start()
    {
        this.canvasObject = GetComponentInParent<Canvas>();

        if (PlayerPrefs.GetInt("playerID", -1) != -1)
        {
            this.characterButtonsParent.SetActive(false);
            StartCoroutine(this.RequestTeam());
        }
    }

    public void SelectButton(int index)
    {
        this.selectedIndex = index;

        for (int i = 0; i < this.buttons.Length; i++)
        {
            if (i == index)
            {
                this.buttons[i].buttonObject.interactable = false;
            }
            else
            {
                this.buttons[i].buttonObject.interactable = true;
            }
        }

        this.confirmButton.interactable = true;
    }

    public void ConfirmButton()
    {
        StartCoroutine(this.ConfirmTeam());
    }

    private IEnumerator ConfirmTeam()
    {
        string fullURL = TwitchSecrets.ServerName + "/setPlayerTeam.php";

        int playerID = PlayerPrefs.GetInt("playerID", -1);

        if (playerID == -1)
        {
            playerID = Random.Range(123456, 987654);
            PlayerPrefs.SetInt("playerID", playerID);
        }

        WWWForm form = new WWWForm();
        form.AddField("playerID", playerID);
        form.AddField("team", (int)this.buttons[this.selectedIndex].team);

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();
        }

        this.characterButtonsParent.SetActive(false);
        this.spawnButtonsParent.SetActive(true);

        GameManager.instance.team = this.buttons[this.selectedIndex].team;
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

            GameManager.instance.team = (Team)int.Parse(www.downloadHandler.text);
        }

        this.spawnButtonsParent.SetActive(true);
    }
}
