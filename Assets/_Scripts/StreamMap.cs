using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using CharacterCustomizer;
using System.IO;

public class StreamMap : MonoBehaviour
{
    [SerializeField]
    private GameMap gameMap;
    
    // Start is called before the first frame update
    void Start()
    {
        this.StartCoroutine(this.RefreshScore());
        this.StartCoroutine(this.RefreshMap());
    }

    private IEnumerator RefreshMap()
    {
        while (true)
        {
            this.gameMap.gameMap.ClearAllTiles();
            this.gameMap.LoadMap();

            yield return new WaitForSeconds(60.0f);
        }
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

                File.WriteAllText("C:\\Users\\jslawski\\Desktop\\streamStuff\\score1.txt", currentScore.team3Score.ToString());
                File.WriteAllText("C:\\Users\\jslawski\\Desktop\\streamStuff\\score2.txt", currentScore.team1Score.ToString());
                File.WriteAllText("C:\\Users\\jslawski\\Desktop\\streamStuff\\score3.txt", currentScore.team2Score.ToString());                
            }

            yield return new WaitForSeconds(10.0f);
        }
    }
}
