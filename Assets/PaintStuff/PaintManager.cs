using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : MonoBehaviour
{
    public static PaintManager instance;

    public Shader paintShader;

    private Material paintMaterial;

    private CommandBuffer command;

    private int positionID = Shader.PropertyToID("_ObjPos");
    private int hardnessID = Shader.PropertyToID("_BrushHardness");
    private int strengthID = Shader.PropertyToID("_BrushStrength");
    private int radiusID = Shader.PropertyToID("_BrushSize");
    private int colorID = Shader.PropertyToID("_BrushColor");
    private int textureID = Shader.PropertyToID("_MainTex");

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.paintMaterial = new Material(paintShader);

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
        
        /*
        int[] scores = ScoreCalculator.instance.GetScores(support);

        Debug.LogError("Team1: " + scores[0] + "\n" +
                "Team2: " + scores[1] + "\n" +
                "Team3: " + scores[2]);
                */

        Graphics.ExecuteCommandBuffer(this.command);
        command.Clear();
    }
}
