using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBrush : MonoBehaviour
{
    public Paintable paintCanvas;

    public Color brushColor;

    [SerializeField]
    public float brushHardness;

    [SerializeField]
    public float brushStrength;

    [SerializeField]
    public float brushSize;

    public bool deltaContributor = true;

    public void Setup(Paintable paintCanvas, Color brushColor, float hardness, float strength, float size)
    {
        this.paintCanvas = paintCanvas;
        this.brushColor = brushColor;
        this.brushHardness = hardness;
        this.brushStrength = strength;
        this.brushSize = size;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.paintCanvas != null)
        {
            PaintManager.instance.Paint(this.paintCanvas, this.transform.position, this.brushSize, this.brushHardness, this.brushStrength, this.brushColor, true);
        }
    }
}
