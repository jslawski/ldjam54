using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPixelChecker : MonoBehaviour
{
    public static TestPixelChecker instance;

    public ComputeShader pixelCheckerShader;
    private ComputeBuffer pixelCheckerBuffer;
    private float[] result;
    private int kernalMain;
    private int kernalInit;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public float[] GetScores(RenderTexture mapTexture)
    {
        this.kernalMain = this.pixelCheckerShader.FindKernel("CSMain");
        this.kernalInit = this.pixelCheckerShader.FindKernel("CSInit");
        this.pixelCheckerBuffer = new ComputeBuffer(1, sizeof(float) * 4);
        this.result = new float[4];

        this.pixelCheckerShader.SetTexture(this.kernalMain, "InputImage", mapTexture);
        this.pixelCheckerShader.SetTexture(this.kernalInit, "InputImage", mapTexture);
        this.pixelCheckerShader.SetBuffer(this.kernalMain, "ResultBuffer", this.pixelCheckerBuffer);
        this.pixelCheckerShader.SetBuffer(this.kernalInit, "ResultBuffer", this.pixelCheckerBuffer);

        this.pixelCheckerShader.Dispatch(this.kernalInit, 1, 1, 1);
        this.pixelCheckerShader.Dispatch(this.kernalMain, mapTexture.width / 8, mapTexture.height / 8, 1);

        this.pixelCheckerBuffer.GetData(result);

        this.pixelCheckerBuffer.Release();
        this.pixelCheckerBuffer = null;

        Debug.LogError("Pixel: " + result[0] + " " + result[1] + " " + result[2] + " " + result[3]);

        return result;
    }
}
