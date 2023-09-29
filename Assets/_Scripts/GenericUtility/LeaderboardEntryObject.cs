using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterCustomizer;
using UnityEngine.Networking;

public class LeaderboardEntryObject : MonoBehaviour
{
    public TextMeshProUGUI username;

    [SerializeField]
    private CustomCharacter character;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private void Start()
    {
        if (this.scoreText.text == "0")
        {
            this.gameObject.SetActive(false);
        }
    }

    public void UpdateEntry(string username, float score)
    {
        this.gameObject.SetActive(true);

        if (this.character != null)
        {

            if (CharacterCache.IsCached(username) == true)
            {
                this.character.LoadCharacterFromJSON(CharacterCache.GetCachedSettings(username));
            }
            else
            {
                this.LoadUserCharacter(username);
            }
        }

        this.username.text = username;
        this.scoreText.text = Mathf.RoundToInt(score).ToString();
    }

    //TODO I hate this a lot.  Find a way to not duplicate all of the below code

    private void LoadUserCharacter(string username)
    {
        StartCoroutine(this.RequestCharacter(username));
    }

    private IEnumerator RequestCharacter(string username)
    {
        string fullURL = TwitchSecrets.ServerName + "/getCurrentPreset.php";

        WWWForm form = new WWWForm();
        form.AddField("username", username);

        using (UnityWebRequest www = UnityWebRequest.Post(fullURL, form))
        {
            yield return www.SendWebRequest();            

            this.character.LoadCharacterFromJSON(www.downloadHandler.text);

            //Cache Attribute Settings
            CharacterCache.UpdateCache(username, www.downloadHandler.text);
        }

        this.ScaleCharacter();
    }

    private void ScaleCharacter()
    {
        CharacterAttribute baseAttribute = this.character.GetAttribute(AttributeType.BaseCabbage);

        float xDiff = (baseAttribute.GetScaleX() - 1.0f) / 2.0f;
        float yDiff = (baseAttribute.GetScaleY() - 1.0f) / 2.0f;

        //Scale up
        if (xDiff <= 0 && yDiff <= 0)
        {
            return;
        }

        float newDiff = 0.0f;

        //Scale based on the biggest diff
        if (xDiff > yDiff)
        {
            newDiff = 1.0f - xDiff;
        }
        else
        {
            newDiff = 1.0f - yDiff;
        }

        this.character.gameObject.transform.localScale = new Vector3(newDiff, newDiff, newDiff);
    }
}
