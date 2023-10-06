using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintable : MonoBehaviour
{
    const int TEXTURE_SIZE = 800;

    private RenderTexture maskRenderTexture;
    private RenderTexture supportRenderTexture;

    private Renderer renderer;

    public RenderTexture GetMaskTexture() => this.maskRenderTexture;
    public RenderTexture GetSupportTexture() => this.supportRenderTexture;
    public Renderer GetRenderer() => this.renderer;

    private void Start()
    {
        this.maskRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        this.maskRenderTexture.filterMode = FilterMode.Bilinear;

        this.supportRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        this.supportRenderTexture.filterMode = FilterMode.Bilinear;

        this.renderer = GetComponent<Renderer>();
        renderer.material.SetTexture("_MaskTexture", this.maskRenderTexture);

        PaintManager.instance.InitializeTextures(this);
    }

    public int[] GetScores()
    {
        return ScoreCalculator.instance.GetScores(this.supportRenderTexture);
    }

    private void OnDisable()
    {
        this.maskRenderTexture.Release();
        this.supportRenderTexture.Release();
    }
}
