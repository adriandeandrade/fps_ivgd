using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Player Configuration")]
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float gravity = 10.0f;
    [SerializeField] private float maxVelocityChange = 10.0f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private bool canJump = true;
    [SerializeField] private Transform cam2;
    [SerializeField] private MouseLook mouseLook;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float startAnimTime = 0.3f;
    [SerializeField] private float stopAnimTime = 0.15f;

    bool grounded = false;

    Rigidbody rBody;
    Camera cam;

    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        cam = Camera.main;
        rBody.freezeRotation = true;
        rBody.useGravity = false;
    }

    private void Start()
    {
        mouseLook = new MouseLook();
        mouseLook.Init(transform, cam2);
    }

    private void Update()
    {
        RotateView();
    }

    void FixedUpdate()
    {
        if (grounded)
        {
            InputMagnitude();

            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rBody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            rBody.AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump
            if (canJump && Input.GetButton("Jump"))
            {
                rBody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }
        }

        // We apply gravity manually for more tuning control
        rBody.AddForce(new Vector3(0, -gravity * rBody.mass, 0));

        grounded = false;

        mouseLook.UpdateCursorLock();
    }

    private void InputMagnitude()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float speed = new Vector2(horizontal, vertical).sqrMagnitude;

        if (speed > 0.01f)
        {
            animator.SetFloat("InputMagnitude", speed, startAnimTime, Time.deltaTime);
        }
        else if (speed < 0.01f)
        {
            animator.SetFloat("InputMagnitude", speed, stopAnimTime, Time.deltaTime);
        }
    }

    private void RotateView()
    {
        mouseLook.LookRotation(transform, cam2);
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}
