using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBrush : MonoBehaviour
{
    public Paintable paintCanvas;

    [HideInInspector]
    public Color brushColor;

    [SerializeField]
    private float brushHardness;

    [SerializeField]
    private float brushStrength;

    [SerializeField]
    private float brushSize;

    // Update is called once per frame
    void Update()
    {
        if (this.paintCanvas != null)
        {
            PaintManager.instance.Paint(this.paintCanvas, this.transform.position, this.brushSize, this.brushHardness, this.brushStrength, this.brushColor);
        }
    }
    
}
