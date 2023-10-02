using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class InitializeChunk : MonoBehaviour
{
    public int xCoord;
    public int yCoord;

    public void InitializeChunkFiles()
    {
        StartCoroutine(this.InitializeCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        string chunkLine = "";

        for (int k = 0; k < 799; k++)
        {
            chunkLine += "0 ";
        }

        chunkLine += "0";

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                string filename = j.ToString() + "_" + i.ToString() + ".txt";

                Debug.LogError("Starting " + filename);

                yield return null;

                string path = Application.dataPath + "/" + filename;

                StreamWriter writer = new StreamWriter(path, true);

                for (int k = 0; k < 800; k++)
                {
                    writer.WriteLine(chunkLine);
                }

                writer.Close();

                Debug.LogError("Done with " + filename);

                yield return null;
            }
        }
    }
}
