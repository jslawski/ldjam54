using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutscenePlayer : MonoBehaviour
{
    [SerializeField]
    private RawImage cutsceneImage;

    [SerializeField]
    private VideoPlayer cutscenePlayer;

    [SerializeField]
    private RenderTexture videoCutsceneTexture;

    private delegate void CutsceneComplete();

    private void Start()
    {
        this.PlayVideoCutscene(TransitionToNextScene);
    }

    private void TransitionToNextScene()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    private void PlayVideoCutscene( CutsceneComplete functionAfterComplete = null)
    {
        this.cutscenePlayer.Stop();

        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "test.mp4");
        this.cutscenePlayer.url = filePath;

        this.cutsceneImage.enabled = true;
        this.cutsceneImage.texture = this.videoCutsceneTexture;

        this.cutscenePlayer.renderMode = VideoRenderMode.RenderTexture;
        this.cutscenePlayer.targetCameraAlpha = 1.0f;
        this.cutscenePlayer.Play();
        
        this.cutscenePlayer.isLooping = false;
        StartCoroutine(this.WaitForCutsceneToFinish(functionAfterComplete));
        
    }

    private IEnumerator WaitForCutsceneToFinish(CutsceneComplete functionAfterComplete)
    {
        while (this.cutscenePlayer.isPlaying == false)
        {
            yield return null;
        }

        while (this.cutscenePlayer.isPlaying)
        {
            yield return null;
        }

        if (functionAfterComplete != null)
        {
            functionAfterComplete();
        }
    }
}
