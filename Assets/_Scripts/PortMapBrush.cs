using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortMapBrush : MonoBehaviour
{
    public PaintBrush brush;

    public Transform brushTransform;

    public PortMapBrush nextBrush;

    [HideInInspector]
    public int brushIndex;

    private BrushPool brushPool;



    public void SetupObject(BrushPool pool, int index)
    {
        this.brushIndex = index;
        this.brushPool = pool;

        this.brush.Setup(pool.paintCanvas, pool.brushColor, brush.brushHardness, brush.brushStrength, brush.brushSize);

        this.UnloadObject();
    }

    public void LoadObject(Vector3 position)
    {
        this.gameObject.SetActive(true);
        this.brushTransform.position = position;
        PaintManager.instance.Paint(this.brush.paintCanvas, this.transform.position, this.brush.brushSize, this.brush.brushHardness, this.brush.brushStrength, this.brush.brushColor);

        StartCoroutine(this.AutoDestroy());
    }

    public void UnloadObject()
    {
        this.brushTransform.position = new Vector3(300, 300, 300);
        this.gameObject.SetActive(false);
    }

    private IEnumerator AutoDestroy()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        this.brushPool.DestroyBrush(this);
    }
}
