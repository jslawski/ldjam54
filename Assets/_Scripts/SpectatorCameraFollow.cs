using System.Collections;
using UnityEngine;

public class SpectatorCameraFollow : MonoBehaviour
{
    public static SpectatorCameraFollow instance;

    [HideInInspector]
    public Transform playerTransform;

    private Camera thisCamera;
    private Transform cameraTransform;

    private float snapSpeed = 3f;

    private Vector3 startingPosition;
    private float startingRotation;

    public float targetCameraDistance;
    public float targetRotation;

    public float isometricZOffset;

    public float spherecastRadius;

    void Awake()
    {
        SpectatorCameraFollow.instance = this;
        this.thisCamera = this.gameObject.GetComponentInChildren<Camera>();
        this.cameraTransform = this.gameObject.transform;
        this.startingPosition = new Vector3(0.0f, 29.0f, 0.0f);
        this.startingRotation = 90.0f;
    }

    private float GetDistance(Vector3 position1, Vector3 position2)
    {
        float xValue = Mathf.Pow(position2.x - position1.x, 2);
        float yValue = Mathf.Pow(position2.y - position1.y, 2);
        float zValue = Mathf.Pow(position2.z - position1.z, 2);

        return Mathf.Sqrt(xValue + yValue + zValue);
    }

    public void BeginFollow(Transform targetPlayerTransform)
    {
        this.playerTransform = targetPlayerTransform;

        StartCoroutine(ZoomIn());
        StartCoroutine(RotateIn());
        StartCoroutine(this.FollowCoroutine());
    }

    private IEnumerator ZoomIn()
    {
        while (Mathf.Abs(this.transform.position.y - this.targetCameraDistance) > 0.5f)
        {
            Vector3 targetPoint = new Vector3(this.transform.position.x, this.targetCameraDistance, this.transform.position.z);
            this.transform.position = Vector3.Lerp(this.transform.position, targetPoint, this.snapSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator RotateIn()
    {
        while (Mathf.Abs(this.thisCamera.transform.rotation.eulerAngles.x - this.targetRotation) > 0.1f)
        {
            float currentRotation = Mathf.Lerp(this.thisCamera.transform.rotation.eulerAngles.x, this.targetRotation, this.snapSpeed * Time.fixedDeltaTime);
            this.thisCamera.transform.rotation = Quaternion.Euler(currentRotation, this.thisCamera.transform.rotation.eulerAngles.y, this.thisCamera.transform.rotation.eulerAngles.z);
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator FollowCoroutine()
    {
        yield return new WaitForFixedUpdate();

        while (this.playerTransform != null)
        {
            Vector3 targetPosition = new Vector3(this.playerTransform.position.x, this.transform.position.y, this.playerTransform.position.z - this.isometricZOffset);

            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, this.snapSpeed * Time.deltaTime);

            yield return null;

            /*Vector3 diffVector = this.playerTransform.position - this.transform.position;

            Vector3 targetPosition = new Vector3(this.transform.position.x + (diffVector.x / 2.0f), this.transform.position.y, this.transform.position.z + (diffVector.z / 2.0f) - (this.isometricZOffset / 2.0f));

            this.transform.position = targetPosition;

            

            yield return null;

            targetPosition = new Vector3(this.transform.position.x + (diffVector.x / 2.0f), this.transform.position.y, this.transform.position.z + (diffVector.z / 2.0f) - (this.isometricZOffset / 2.0f));
            this.transform.position = targetPosition;
            */
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            this.LocatePlayer();
        }
        if (Input.GetMouseButtonUp(1))
        {
            this.ReturnToZoomOut();
        }
    }

    private void LocatePlayer()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] allhits = Physics.SphereCastAll(mouseRay, this.spherecastRadius);

        for (int i = 0; i < allhits.Length; i++)
        {
            if (allhits[i].transform.tag == "Player")
            {
                this.BeginFollow(allhits[i].transform);
                break;
            }
        }
    }

    private void ReturnToZoomOut()
    {
        this.playerTransform = null;

        this.StopAllCoroutines();

        StartCoroutine(this.ZoomOut());
        StartCoroutine(this.RotateOut());
    }

    private IEnumerator ZoomOut()
    {
        while (Mathf.Abs(this.transform.position.y - this.startingPosition.y) > 0.5f)
        {
            Vector3 targetPoint = this.startingPosition;
            this.transform.position = Vector3.Lerp(this.transform.position, targetPoint, this.snapSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator RotateOut()
    {
        while (Mathf.Abs(this.thisCamera.transform.rotation.eulerAngles.x - this.startingRotation) > 0.1f)
        {
            float currentRotation = Mathf.Lerp(this.thisCamera.transform.rotation.eulerAngles.x, this.startingRotation, this.snapSpeed * Time.fixedDeltaTime);
            this.thisCamera.transform.rotation = Quaternion.Euler(currentRotation, this.thisCamera.transform.rotation.eulerAngles.y, this.thisCamera.transform.rotation.eulerAngles.z);
            yield return new WaitForFixedUpdate();
        }
    }
}