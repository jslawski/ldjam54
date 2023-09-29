using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadePanelManager : MonoBehaviour
{
    [SerializeField]
    private Image fadePanel;
    [SerializeField]
    private float fadeDuration = 0.5f;

    private float alphaChangePerFrame;

    public delegate void FadeComplete();
    public event FadeComplete OnFadeSequenceComplete;

    private void Start()
    {
        this.alphaChangePerFrame = (1.0f / this.fadeDuration) * Time.fixedDeltaTime;
    }

    public void FadeToBlack()
    {
        StopAllCoroutines();
        this.SetAlpha(0.0f);
        StartCoroutine(this.FadeToBlackCoroutine());
    }

    public void FadeFromBlack()
    {
        StopAllCoroutines();
        this.SetAlpha(1.0f);
        StartCoroutine(this.FadeFromBlackCoroutine());
    }

    public void SetAlpha(float amount)
    {
        Color updatedColor = new Color(this.fadePanel.color.r, this.fadePanel.color.g, this.fadePanel.color.b, amount);
        fadePanel.color = updatedColor;
    }

    private IEnumerator FadeFromBlackCoroutine()
    {
        while (fadePanel.color.a > 0)
        {
            float newAlpha = this.fadePanel.color.a - this.alphaChangePerFrame;
            Color updatedColor = new Color(this.fadePanel.color.r, this.fadePanel.color.g, this.fadePanel.color.b, newAlpha);
            this.fadePanel.color = updatedColor;

            yield return new WaitForFixedUpdate();
        }

        if (this.OnFadeSequenceComplete != null)
        {
            this.OnFadeSequenceComplete();
        }
    }

    private IEnumerator FadeToBlackCoroutine()
    {
        while (fadePanel.color.a < 1)
        {
            float newAlpha = this.fadePanel.color.a + this.alphaChangePerFrame;
            Color updatedColor = new Color(this.fadePanel.color.r, this.fadePanel.color.g, this.fadePanel.color.b, newAlpha);
            this.fadePanel.color = updatedColor;

            yield return new WaitForFixedUpdate();
        }

        if (this.OnFadeSequenceComplete != null)
        {
            this.OnFadeSequenceComplete();
        }
    }    
}
