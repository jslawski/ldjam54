using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectLoop : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)]
    private float minDistanceBetweenPoints;

    private List<Vector2> trailPositions;

    private Transform playerTransform;

    [SerializeField]
    private GameObject trailMarker;

    [SerializeField, Range(0, 10)]
    private int minPointsBack;

    [SerializeField, Range(0.0f, 1.0f)]
    private float marginOfError;

    [SerializeField]
    private GameObject loopLine;

    private int numPointsInLoop = 0;

    private void Awake()
    {
        this.trailPositions = new List<Vector2>();
        this.playerTransform = this.transform;

        this.trailPositions.Add(new Vector2(this.playerTransform.position.x, this.playerTransform.position.z));
    }

    private void FixedUpdate()
    {
        if (this.ShouldCreateNewPoint() == true)
        {
            this.trailPositions.Add(new Vector2(this.playerTransform.position.x, this.playerTransform.position.z));            

            //Instantiate(this.trailMarker, this.playerTransform.position, new Quaternion());
        }
        
        if (this.InersectionDetected())
        {
            Debug.LogError("Loop Detected!");
            //this.DrawLoop();
        }
    }

    private void DrawLoop()
    {
        GameObject loopInstance = Instantiate(this.loopLine, this.playerTransform.position, new Quaternion());
        LineRenderer loopLine = loopInstance.GetComponent<LineRenderer>();

        PlayerTrail trailReference = this.gameObject.GetComponent<PlayerTrail>();

        

        for (int i = 0; i < this.numPointsInLoop; i++)
        {
            loopLine.positionCount++;
            loopLine.SetPosition(i, trailReference.playerTrail.GetPosition(trailReference.playerTrail.positionCount - (1+i)));
        }
    }

    private bool ShouldCreateNewPoint()
    {        
        Vector2 playerPos = new Vector2(this.playerTransform.position.x, this.playerTransform.position.z);

        return (Vector2.Distance(this.trailPositions[this.trailPositions.Count - 1], playerPos) >= this.minDistanceBetweenPoints);
    }

    private bool InersectionDetected()
    {
        if (this.trailPositions.Count < 2)
        {
            return false;
        }

        Vector2 latestStartingPoint = this.trailPositions[this.trailPositions.Count - 2];
        Vector2 latestEndingPoint = this.trailPositions[this.trailPositions.Count - 1];

        Vector2 testStartingPoint;
        Vector2 testEndingPoint;

        for (int i = 0; i < this.trailPositions.Count - 3; i++)        
        {
            testStartingPoint = this.trailPositions[i];
            testEndingPoint = this.trailPositions[i + 1];

            if (this.IsIntersecting(latestStartingPoint, latestEndingPoint, testStartingPoint, testEndingPoint) == true)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsIntersecting(Vector2 p0, Vector2 p1, Vector3 p2,  Vector3 p3)
    {
        float s1_x;
        float s1_y;
        float s2_x;
        float s2_y;

        s1_x = p1.x - p0.x;
        s1_y = p1.y - p0.y;
        s2_x = p3.x - p2.x;
        s2_y = p3.y - p2.y;

        float s;
        float t;

        s = (-s1_y * (p0.x - p2.x) + s1_x * (p0.y - p2.y)) / (-s2_x * s1_y + s1_x * s2_y);
        t = (s2_x * (p0.y - p2.y) - s2_y * (p0.x - p2.x)) / (-s2_x * s1_y + s1_x * s2_y);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            return true;
        }

        return false;
    }

    private bool LoopDetected()
    {
        if (this.trailPositions.Count < this.minPointsBack + 2)
        {
            return false;
        }

        Vector3 previousPosition = this.trailPositions[this.trailPositions.Count - 2];
        Vector3 currentPosition = this.trailPositions[this.trailPositions.Count - 1];

        int test = 0;

        for (int i = this.trailPositions.Count - (3 + this.minPointsBack); i > 0; i--)
        {
            if (this.IsPointBetween(previousPosition, currentPosition, this.trailPositions[i]))
            {
                Debug.LogError("Intersection at index: " + i);

                this.numPointsInLoop = test;
                return true;
            }

            test++;
        }

        return false;
    }

    private bool IsPointBetween(Vector3 pos1, Vector3 pos2, Vector2 testPos)
    {
        float xAlpha = ((testPos.x - pos1.x) / (pos2.x - pos1.x));
        float yAlpha = ((testPos.y - pos1.y) / (pos2.y - pos1.y));

        float minValue = 0.0f - this.marginOfError;
        float maxValue = 1.0f + this.marginOfError;


        return (xAlpha >= minValue && xAlpha <= maxValue) && (yAlpha >= minValue && yAlpha <= maxValue);        
    }
}
