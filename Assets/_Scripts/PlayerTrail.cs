using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public LineRenderer playerTrail;
    public Transform playerTransform;

    [SerializeField, Range(0.0f, 1.0f)]
    private float minDistanceBetweenTrailPoints = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        this.playerTrail.positionCount = 1;
        this.AddNewPointAtPlayerPosition();
    }

    private void AddNewPointAtPlayerPosition()
    {
        this.playerTrail.positionCount++;
        this.playerTrail.SetPosition(this.playerTrail.positionCount - 1, this.playerTransform.position);
    }

    private float PlayerDistanceFromPreviousPoint()
    {
        Vector3 currentPosition = this.playerTrail.GetPosition(this.playerTrail.positionCount - 1);
        float xValues = Mathf.Pow(this.playerTransform.position.x - currentPosition.x, 2.0f);
        float yValues = Mathf.Pow(this.playerTransform.position.y - currentPosition.y, 2.0f);

        return Mathf.Abs(Mathf.Sqrt(xValues + yValues));
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (this.PlayerDistanceFromPreviousPoint() > this.minDistanceBetweenTrailPoints)
        {
            this.AddNewPointAtPlayerPosition();
        }
    }
}
