using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLineCollider : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineTest;
    private BoxCollider collider;

    private float minX = float.PositiveInfinity;
    private float minY = float.PositiveInfinity;
    private float maxX = float.NegativeInfinity;
    private float maxY = float.NegativeInfinity;

    private void Awake()
    {
        this.collider = this.GetComponent<BoxCollider>();
        this.UpdateCollider();
    }

    private void UpdateCollider()
    {
        for (int i = 0; i < this.lineTest.positionCount; i++)
        {
            this.UpdateColliderExtents(this.lineTest.GetPosition(i));
        }
    }

    public void UpdateColliderExtents(Vector2 newPoint)
    {
        if (newPoint.x < this.minX)
        {
            this.minX = newPoint.x;
        }
        if (newPoint.x > this.maxX)
        {
            this.maxX = newPoint.x;
        }

        if (newPoint.y < this.minY)
        {
            this.minY = newPoint.y;
        }
        if (newPoint.y > this.maxY)
        {
            this.maxY = newPoint.y;
        }

        Debug.LogError("Min Point: (" + this.minX + ", " + this.minY + ")\n" + "Max Point: (" + this.maxX + ", " + this.maxY + ")");

        Vector3 midPoint = new Vector3((this.minX + this.maxX) / 2.0f, 0.0f, (this.minY + this.maxY) / 2.0f);

        this.collider.center = midPoint;
        this.collider.size = midPoint;
    }
}
