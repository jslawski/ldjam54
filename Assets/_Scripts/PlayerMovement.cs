using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    private float maxSpeed = 10f;
    [SerializeField, Range(0, 100)]
    private float maxAcceleration = 30f;
    [SerializeField, Range(0, 100)]
    private float maxDeceleration = 10f;

    private Vector3 playerVelocity;
    private Vector3 desiredPlayerVelocity;
    private float desiredMaxSpeedChange;

    private Collider movementCollider = null;

    public Rigidbody rigidBody { get; private set; }

    private void Start()
    {
        this.rigidBody = GetComponent<Rigidbody>();
        this.movementCollider = GetComponent<Collider>();
    }

    private void AccelerateTowardsDesiredVelocity(Vector3 desiredVelocity, float maxSpeedChange)
    {
        this.playerVelocity.x = Mathf.MoveTowards(playerVelocity.x, desiredVelocity.x, maxSpeedChange);
        this.playerVelocity.z = Mathf.MoveTowards(playerVelocity.z, desiredVelocity.z, maxSpeedChange);
    }

    private void Update()
    {
        playerVelocity = this.rigidBody.velocity;
        if (Input.GetMouseButton(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                Vector3 desiredDirection = hit.point - this.transform.position;
                desiredPlayerVelocity = desiredDirection.normalized * maxSpeed;
                desiredMaxSpeedChange = this.maxAcceleration;
            }
        }
        else if (this.playerVelocity.magnitude > 0)
        {
            this.desiredPlayerVelocity = Vector3.zero;
            this.desiredMaxSpeedChange = this.maxDeceleration;                       
        }
    }

    private void FixedUpdate()
    {
        float maxSpeedChange = desiredMaxSpeedChange * Time.deltaTime;

        this.AccelerateTowardsDesiredVelocity(this.desiredPlayerVelocity, maxSpeedChange);
        if (this.playerVelocity.magnitude < 0)
        {
            this.playerVelocity = Vector3.zero;
        }

        this.rigidBody.velocity = playerVelocity;
    }

    
}
