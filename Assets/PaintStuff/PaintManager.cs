using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : MonoBehaviour
{
    public static PaintManager instance;

    public Shader paintShader;
    public Material paintMaterial;

    public Shader deltaPaintShader;
    public Material deltaPaintMaterial;

    public Renderer savedMap;

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

    public void Paint(Paintable paintableObj, Vector3 objPos, float radius = 1.0f, float hardness = 0.5f, float strength = 0.5f, Color? color = null)
    {
        RenderTexture mask = paintableObj.GetMaskTexture();
        RenderTexture support = paintableObj.GetSupportTexture();
        RenderTexture delta = paintableObj.GetDeltaTexture();
        Renderer rend = paintableObj.GetRenderer();
        
        this.paintMaterial.SetVector(this.positionID, objPos);
        this.paintMaterial.SetFloat(this.hardnessID, hardness);
        this.paintMaterial.SetFloat(this.strengthID, strength);
        this.paintMaterial.SetFloat(this.radiusID, radius);
        this.paintMaterial.SetTexture(this.textureID, support);
        this.paintMaterial.SetColor(this.colorID, color ?? Color.red);

        this.command.SetRenderTarget(mask);
        this.command.DrawRenderer(rend, this.paintMaterial, 0);

        this.command.SetRenderTarget(support);
        this.command.Blit(mask, support);
        
        this.command.SetRenderTarget(delta);
        this.command.Blit(mask, delta);

        Graphics.ExecuteCommandBuffer(this.command);
        this.command.Clear();
    }

    public void DeltaPaint(Paintable paintableObj)
    {
        RenderTexture mask = paintableObj.GetMaskTexture();
        RenderTexture support = paintableObj.GetSupportTexture();
        RenderTexture delta = paintableObj.GetDeltaTexture();
        Renderer rend = paintableObj.GetRenderer();             

        this.deltaPaintMaterial.SetTexture(this.deltaTexureID, mask);
        this.deltaPaintMaterial.SetTexture(this.textureID, support);

        this.command.SetRenderTarget(delta);
        this.command.DrawRenderer(rend, this.deltaPaintMaterial, 0);

        this.command.SetRenderTarget(support);
        this.command.Blit(delta, support, this.deltaPaintMaterial);
        Graphics.ExecuteCommandBuffer(this.command);
        this.command.Clear();
    }
}
