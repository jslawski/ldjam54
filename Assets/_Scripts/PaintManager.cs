using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : MonoBehaviour
{
    public static PaintManager instance;

    public Shader paintShader;
    private Material paintMaterial;

    public Shader deltaPaintShader;
    private Material deltaPaintMaterial;

    private CommandBuffer command;

    private int positionID = Shader.PropertyToID("_ObjPos");
    private int hardnessID = Shader.PropertyToID("_BrushHardness");
    private int strengthID = Shader.PropertyToID("_BrushStrength");
    private int radiusID = Shader.PropertyToID("_BrushSize");
    private int colorID = Shader.PropertyToID("_BrushColor");
    private int textureID = Shader.PropertyToID("_MainTex");
    private int deltaTexureID = Shader.PropertyToID("_DeltaTex");

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.paintMaterial = new Material(this.paintShader);
        this.deltaPaintMaterial = new Material(this.deltaPaintShader);

        this.command = new CommandBuffer();
        this.command.name = "CommandBuffer - " + this.gameObject.name;
    }

    public void InitializeTextures(Paintable paintableObj)
    {
        RenderTexture mask = paintableObj.GetMaskTexture();
        RenderTexture support = paintableObj.GetSupportTexture();
        Renderer rend = paintableObj.GetRenderer();

        this.command.SetRenderTarget(mask);
        this.command.SetRenderTarget(support);
        
        this.command.DrawRenderer(rend, this.paintMaterial, 0);

        Graphics.ExecuteCommandBuffer(this.command);

        this.command.Clear();
    }

    public void Paint(Paintable paintableObj, Vector3 objPos, float radius = 1.0f, float hardness = 0.5f, float strength = 0.5f, Color? color = null, bool deltaContributor = true)
    {
        RenderTexture mask = paintableObj.GetMaskTexture();
        RenderTexture support = paintableObj.GetSupportTexture();
        RenderTexture delta = paintableObj.GetDeltaTexture();
        Renderer rend = paintableObj.GetRenderer();
        
        this.paintMaterial.SetVector(this.positionID, objPos);
        this.paintMaterial.SetFloat(this.hardnessID, hardness);
        this.paintMaterial.SetFloat(this.strengthID, strength);
        this.paintMaterial.SetFloat(this.radiusID, radius);
        this.paintMaterial.SetColor(this.colorID, color ?? Color.red);
        
        this.paintMaterial.SetTexture(this.textureID, support);
                
        this.command.SetRenderTarget(mask);
        this.command.DrawRenderer(rend, this.paintMaterial, 0);
        
        this.command.SetRenderTarget(support);
        this.command.Blit(mask, support);

        if (deltaContributor == true)
        {
            this.command.SetRenderTarget(delta);
            this.command.Blit(mask, delta);
        }

        Graphics.ExecuteCommandBuffer(this.command);
        this.command.Clear();
    }

    public void OverwriteCanvas(Paintable paintableObj, Texture newCanvas)
    {
        RenderTexture mask = paintableObj.GetMaskTexture();
        RenderTexture support = paintableObj.GetSupportTexture();
        RenderTexture delta = paintableObj.GetDeltaTexture();
        RenderTexture temp = paintableObj.GetTempTexture();
        Renderer rend = paintableObj.GetRenderer();

        //Step 0:  Blit Texture2D onto temporary RenderTexture
        this.command.SetRenderTarget(temp);
        this.command.Blit(newCanvas, temp);
        Graphics.ExecuteCommandBuffer(this.command);
        this.command.Clear();
        
        //Step 1:  Blit Delta onto loaded map to make composite map
        this.deltaPaintMaterial.SetTexture(this.deltaTexureID, mask);
        this.deltaPaintMaterial.SetTexture(this.textureID, temp);
        this.command.SetRenderTarget(delta);
        this.command.Blit(delta, temp, this.deltaPaintMaterial);
        Graphics.ExecuteCommandBuffer(this.command);
        this.command.Clear();

        //Step 2:  Blit composite map onto support texture        
        //this.command.SetRenderTarget(support);
        this.command.Blit(temp, support);
        Graphics.ExecuteCommandBuffer(this.command);
        this.command.Clear();
        
        //Step 3:  DrawRenderer with paintMaterial
        this.paintMaterial.SetTexture(this.textureID, temp);
        this.command.SetRenderTarget(mask);
        this.command.DrawRenderer(rend, this.paintMaterial, 0);
        Graphics.ExecuteCommandBuffer(this.command);
        this.command.Clear();
    }

    public void DeltaPaint(Paintable paintableObj, Texture latestCanvas)
    {
        RenderTexture mask = paintableObj.GetMaskTexture();
        RenderTexture support = paintableObj.GetSupportTexture();
        RenderTexture delta = paintableObj.GetDeltaTexture();
        Renderer rend = paintableObj.GetRenderer();             

        this.deltaPaintMaterial.SetTexture(this.deltaTexureID, mask);
        this.deltaPaintMaterial.SetTexture(this.textureID, latestCanvas);

        this.command.SetRenderTarget(delta);
        this.command.DrawRenderer(rend, this.deltaPaintMaterial, 0);

        this.command.SetRenderTarget(support);
        this.command.Blit(delta, support, this.deltaPaintMaterial);

        Graphics.ExecuteCommandBuffer(this.command);
        this.command.Clear();
    }

    public void Blit(Texture source, Paintable dest, bool shouldDraw = false)
    {
        RenderTexture mask = dest.GetMaskTexture();
        RenderTexture support = dest.GetSupportTexture();
        Renderer rend = dest.GetRenderer();

        this.command.SetRenderTarget(support);
        this.command.Blit(source, support);

        this.paintMaterial.SetTexture(this.textureID, source);

        if (shouldDraw == true)
        {
            this.command.SetRenderTarget(mask);
            this.command.DrawRenderer(rend, this.paintMaterial, 0);
        }

        Graphics.ExecuteCommandBuffer(this.command);
        this.command.Clear();
    }
}

//Blit delta to loaded texture, then blit THAT composite to support texture, then draw renderer
