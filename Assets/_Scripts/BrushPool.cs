using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushPool : MonoBehaviour
{
    const int PoolSize = 20000;

    private Transform parentTransform;

    [SerializeField]
    public GameObject BrushPrefab;

    private PortMapBrush[] pool;

    private PortMapBrush firstAvailableBrush;

    public Color brushColor;

    public Paintable paintCanvas;

    // Start is called before the first frame update
    void Awake()
    {
        this.parentTransform = GetComponent<Transform>();
        this.InitializePool();
    }

    public PortMapBrush CreateBrush(Vector3 position)
    {
        if (this.firstAvailableBrush == null)
        {
            return null;
        }

        PortMapBrush newBrush = this.firstAvailableBrush;
        newBrush.LoadObject(position);

        this.firstAvailableBrush = this.firstAvailableBrush.nextBrush;

        return newBrush;
    }

    public void DestroyBrush(PortMapBrush destroyedBrush)
    {
        destroyedBrush.UnloadObject();
        this.pool[destroyedBrush.brushIndex].nextBrush = this.firstAvailableBrush;
        this.firstAvailableBrush = destroyedBrush;
    }

    public void InitializePool()
    {
        //Instantiate objects
        this.pool = new PortMapBrush[PoolSize];
        for (int i = 0; i < this.pool.Length; i++)
        {
            GameObject BrushObject = Instantiate(this.BrushPrefab, this.parentTransform);
            PortMapBrush BrushComponent = BrushObject.GetComponent<PortMapBrush>();
            BrushComponent.SetupObject(this, i);
            this.pool[i] = BrushComponent;
        }

        //Set up linked list
        this.firstAvailableBrush = this.pool[0];
        for (int i = 0; i < this.pool.Length - 1; i++)
        {
            this.pool[i].nextBrush = this.pool[i + 1];
        }

        this.pool[this.pool.Length - 1].nextBrush = null;
    }
}