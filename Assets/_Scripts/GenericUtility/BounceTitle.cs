using UnityEngine;
using System.Collections;

public class BounceTitle : MonoBehaviour
{
    public float verticalAmplitude;
    public float verticalTime;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(BounceObjectCoroutine());
    }

    IEnumerator BounceObjectCoroutine()
    {
        float timeElapsed = 0;

        Vector3 startPos = this.transform.localPosition;

        while (true)
        {
            timeElapsed += Time.deltaTime;

            this.transform.localPosition = startPos + Vector3.up * verticalAmplitude * Mathf.Sin(2 * Mathf.PI * timeElapsed / verticalTime);

            yield return 0;
        }
    }
}