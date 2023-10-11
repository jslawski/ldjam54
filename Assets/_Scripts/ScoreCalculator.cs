using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    public static ScoreCalculator instance;

    public ComputeShader scoreShader;
    private ComputeBuffer scoreBuffer;
    private int[] result;
    private int kernalMain;
    private int kernalInit;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public int[] GetScores(RenderTexture mapTexture)
    {
        this.kernalMain = this.scoreShader.FindKernel("CSMain");
        this.kernalInit = this.scoreShader.FindKernel("CSInit");
        this.scoreBuffer = new ComputeBuffer(1, sizeof(int) * 4);
        this.result = new int[4];

        this.scoreShader.SetTexture(this.kernalMain, "InputImage", mapTexture);
        this.scoreShader.SetTexture(this.kernalInit, "InputImage", mapTexture);
        this.scoreShader.SetBuffer(this.kernalMain, "ResultBuffer", this.scoreBuffer);
        this.scoreShader.SetBuffer(this.kernalInit, "ResultBuffer", this.scoreBuffer);

        this.scoreShader.Dispatch(this.kernalInit, 1, 1, 1);
        this.scoreShader.Dispatch(this.kernalMain, mapTexture.width / 8, mapTexture.height / 8, 1);

        this.scoreBuffer.GetData(result);

        this.scoreBuffer.Release();
        this.scoreBuffer = null;

        return result;
    }
}
