using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatLordMover : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed;
    public float maxAcceleration;

    private Vector2 input;

    private Rigidbody body;

    private Vector3 velocity;
    private Vector3 direction;

    void Start()
    {
        body = GetComponent<Rigidbody>();

        input = new Vector2();
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        input.x = xInput;
        input.y = yInput;
    }

    private void FixedUpdate()
    {
        AdjustVelocity();

        body.velocity = velocity;
    }

    private void AdjustVelocity()
    {
        direction.x = input.x;
        direction.z = input.y;
        direction = direction.normalized;

        Vector3 desiredVelocity = direction * maxSpeed;

        Vector3 velocityChange = Vector3.MoveTowards(body.velocity, desiredVelocity, maxAcceleration * Time.fixedDeltaTime);

        velocity = velocityChange;
    }
}
