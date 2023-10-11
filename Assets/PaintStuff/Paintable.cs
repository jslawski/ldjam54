using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintable : MonoBehaviour
{
    public int TEXTURE_SIZE = 1024;

    public RenderTexture maskRenderTexture;
    public RenderTexture supportRenderTexture;
    public RenderTexture deltaRenderTexture;

    private Renderer renderer;

    public RenderTexture GetMaskTexture() => this.maskRenderTexture;
    public RenderTexture GetSupportTexture() => this.supportRenderTexture;
    public RenderTexture GetDeltaTexture() => this.deltaRenderTexture;
    public Renderer GetRenderer() => this.renderer;

    public int depth = 0;

    private void Start()
    {
        this.InitializePaintable();
    }

    private void InitializePaintable()
    {
        this.maskRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, this.depth, RenderTextureFormat.ARGB32);
        this.maskRenderTexture.filterMode = FilterMode.Bilinear;

        this.supportRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, this.depth, RenderTextureFormat.ARGB32);
        this.supportRenderTexture.filterMode = FilterMode.Bilinear;

        this.deltaRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, this.depth, RenderTextureFormat.ARGB32);
        this.deltaRenderTexture.filterMode = FilterMode.Bilinear;

        this.renderer = GetComponent<Renderer>();
        renderer.material.SetTexture("_MaskTexture", this.maskRenderTexture);


        PaintManager.instance.InitializeTextures(this);
    }    

    public int[] GetScores()
    {
        return ScoreCalculator.instance.GetScores(this.supportRenderTexture);
    }    

    public void ClearDelta()
    {
        this.maskRenderTexture.Release();
        this.deltaRenderTexture.Release();        
    }

    private void OnDestroy()
    {
        this.maskRenderTexture.Release();
        this.supportRenderTexture.Release();
        this.deltaRenderTexture.Release();

        Destroy(this.maskRenderTexture);
        Destroy(this.supportRenderTexture);
        Destroy(this.deltaRenderTexture);
    }
}
