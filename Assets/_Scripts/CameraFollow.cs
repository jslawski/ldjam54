using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    [HideInInspector]
    public PlayerMovement playerCharacter;

    private Camera thisCamera;
    private Transform cameraTransform;
    private float cameraDistance;

    public float verticalViewportThreshold = 0.5f;
    public float horizontalViewportThreshold = 0.5f;

    private static bool followPlayer = false;
    private static Vector3 showcasePoint;
    private static bool snapInitiated = false;
    private float snapSpeed = 0.3f;
    private float originalYValue;
    private bool returnZoomInitiated = false;
    private Coroutine SnapCoroutine;

    //Impact zoom stuff
    private float impactZoomAmount = 0.8f;
    private float impactZoomSpeed = 0.4f;
    private float impactReturnZoomSpeed = 0.01f;
    private float zoomedInYValue;
    private Coroutine ImpactZoomCoroutine;
    private Coroutine ImpactReturnCoroutine;
    private float cumulativeYZoom;

    private static float isometricZOffset;

    void Awake()
    {
        CameraFollow.instance = this;
        this.thisCamera = this.gameObject.GetComponentInChildren<Camera>();
        this.cameraTransform = this.gameObject.transform;
        this.cameraDistance = this.cameraTransform.position.y;
        this.originalYValue = this.transform.position.y;

        isometricZOffset = this.GetIsometricZOffset();
        this.cumulativeYZoom = 0;
    }

    private float GetIsometricZOffset()
    {
        return Camera.main.transform.position.y * Mathf.Tan(Camera.main.transform.rotation.x);
    }

    private float GetDistance(Vector3 position1, Vector3 position2)
    {
        float xValue = Mathf.Pow(position2.x - position1.x, 2);
        float yValue = Mathf.Pow(position2.y - position1.y, 2);
        float zValue = Mathf.Pow(position2.z - position1.z, 2);

        return Mathf.Sqrt(xValue + yValue + zValue);
    }

    private bool IsPlayerPastHorizontalThreshold(float playerViewportXPosition)
    {
        return (playerViewportXPosition >= (1.0f - this.horizontalViewportThreshold)) ||
            (playerViewportXPosition <= (0.0f + this.horizontalViewportThreshold));
    }

    private bool IsPlayerPastVerticalThreshold(float playerViewportYPosition)
    {
        return (playerViewportYPosition >= (1.0f - this.verticalViewportThreshold)) ||
            (playerViewportYPosition <= (0.0f + this.verticalViewportThreshold));
    }

    public void BeginFollow()
    {
        StartCoroutine(this.FollowCoroutine());
    }
    
    private IEnumerator FollowCoroutine()
    {
        yield return new WaitForFixedUpdate();

        this.transform.position = new Vector3(this.playerCharacter.transform.position.x, this.transform.position.y, this.playerCharacter.transform.position.z - 3.0f);

        while (followPlayer == true)
        {
            if (this.transform.position.y != this.originalYValue)
            {
                if (this.returnZoomInitiated == false)
                {
                    StopAllCoroutines();
                    this.SnapCoroutine = StartCoroutine(this.SnapToPoint(new Vector3(this.transform.position.x, this.originalYValue, this.transform.position.z)));
                    this.returnZoomInitiated = true;
                    this.cumulativeYZoom = 0;
                }                
            }

            Vector3 playerViewportPosition = thisCamera.WorldToViewportPoint(this.playerCharacter.gameObject.transform.position);

            if (this.playerCharacter != null && this.IsPlayerPastVerticalThreshold(playerViewportPosition.y))
            {
                this.UpdateCameraVerticalPosition();
            }

            if (this.IsPlayerPastHorizontalThreshold(playerViewportPosition.x))
            {
                this.UpdateCameraHorizontalPosition();
            }

            yield return new WaitForFixedUpdate();
        }        
    }

    private IEnumerator SnapToPoint(Vector3 targetPoint)
    {
        targetPoint = new Vector3(targetPoint.x, targetPoint.y, targetPoint.z);

        while (this.GetDistance(this.transform.position, targetPoint) > 0.01f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPoint, this.snapSpeed);
            yield return null;
        }

        this.transform.position = targetPoint;
        this.returnZoomInitiated = false;
        this.zoomedInYValue = this.transform.position.y;

        this.SnapCoroutine = null;
        this.ImpactZoomCoroutine = null;
        this.ImpactReturnCoroutine = null;
    }

    public static void InitiateShowcaseSnap(Vector3 targetPoint)
    {
        showcasePoint = new Vector3(targetPoint.x, targetPoint.y, targetPoint.z - (isometricZOffset / 2));
        followPlayer = false;
    }

    public static void ReturnToFollow()
    {
        showcasePoint = Vector3.zero;
        followPlayer = true;
        snapInitiated = false;        
    }

    private void UpdateCameraVerticalPosition()
    {
        Vector3 worldSpaceCenteredPosition = this.thisCamera.ViewportToWorldPoint(new Vector3(0.5f, this.cameraDistance, this.verticalViewportThreshold));

        Vector3 shiftVector = new Vector3(0, 0, this.playerCharacter.transform.position.z - worldSpaceCenteredPosition.z);

        this.cameraTransform.Translate(shiftVector.normalized * Mathf.Abs(this.playerCharacter.rigidBody.velocity.z) * Time.deltaTime);
    }

    private void UpdateCameraHorizontalPosition()
    {
        Vector3 worldSpaceCenteredPosition = this.thisCamera.ViewportToWorldPoint(new Vector3(0.5f, this.cameraDistance, this.verticalViewportThreshold));

        Vector3 shiftVector = new Vector3(this.playerCharacter.transform.position.x - worldSpaceCenteredPosition.x, 0, 0);

        this.cameraTransform.Translate(shiftVector.normalized * Mathf.Abs(this.playerCharacter.rigidBody.velocity.x) * Time.deltaTime);
    }
}