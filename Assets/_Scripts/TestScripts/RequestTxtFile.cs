using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterCustomizer;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using TMPro;

public class RequestTxtFile : MonoBehaviour
{
    public TextMeshProUGUI testText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.RequestFile());        
    }

    private IEnumerator RequestFile()
    {
        string fullURL = TwitchSecrets.ServerName + "/0_0.txt";

        using (UnityWebRequest www = UnityWebRequest.Get(fullURL))
        {
            this.testText.text = "Requesting...";

            yield return www.SendWebRequest();

            this.testText.text = "Done!  Press I to barf...";

            while (Input.GetKeyDown(KeyCode.I) == false)
            {
                yield return null;
            }

            string[] chunkArray = Encoding.ASCII.GetString(www.downloadHandler.data).Split('\n');

            this.testText.text = chunkArray[0] + "\n";
            this.testText.text += chunkArray[100] + "\n";
            this.testText.text += chunkArray[5000];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            StartCoroutine(this.RequestFile());
        }
    }
}
