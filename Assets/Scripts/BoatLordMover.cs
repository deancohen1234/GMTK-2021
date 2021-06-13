using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatLordMover : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform frogTransform;

    [Header("Movement")]
    public float maxSpeed;
    public float maxAcceleration;
    public float maxRotationAcceleration = 10f;

    [Header("Animation")]
    public float waveDuration = 0.75f;

    private Vector2 input;

    private Rigidbody body;

    private Vector3 velocity;
    private Vector3 direction;

    private bool isMoving = false;

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

        UpdateAnimation();
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

    private void UpdateAnimation()
    {
        if (body.velocity.sqrMagnitude > 0.05f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        //rotate player towards move direction
        if (direction != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(-direction, Vector3.up));

            transform.rotation = Quaternion.RotateTowards(frogTransform.rotation, desiredRotation, Time.deltaTime * maxRotationAcceleration);
        }

        //set animation triggers
        if (animator)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }

    public void PlayWaveAnimation()
    {
        if (animator)
        {
            animator.SetTrigger("IsHoying");
        }
    }
}
