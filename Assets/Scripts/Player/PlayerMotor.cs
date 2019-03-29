using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Player Configuration")]
    [SerializeField] private float sprintSpeed = 15.0f;
    [SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private float gravity = 10.0f;
    [SerializeField] private float maxVelocityChange = 10.0f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private bool canJump = true;
    [SerializeField] private Transform cam2;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private KeyCode sprintKey;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float startAnimTime = 0.3f;
    [SerializeField] private float stopAnimTime = 0.15f;

    public bool IsSprinting { get { return isSprinting; } set { isSprinting = value; } }
    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }

    bool isGrounded = false;
    bool isSprinting;
    bool isJumping;

    float horizontal;
    float vertical;

    InputManager inputManager;
    Rigidbody rBody;
    Camera cam;

    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
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
        //InputMagnitude();
        //ControlInputs();
        HandleAnimations();
        RotateView();
    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = inputManager.Movement;
            targetVelocity = transform.TransformDirection(targetVelocity);

            if(isSprinting)
                targetVelocity *= sprintSpeed;
            else
                targetVelocity *= walkSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rBody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rBody.AddForce(velocityChange, ForceMode.VelocityChange);

            if (canJump && isJumping)
            {
                rBody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                isJumping = false;
            }
        }

        // We apply gravity manually for more tuning control
        rBody.AddForce(new Vector3(0, -gravity * rBody.mass, 0));

        isGrounded = false;

        mouseLook.UpdateCursorLock();
    }

    private void HandleAnimations()
    {
        if(inputManager.InputMag > 0.01f)
        {
            animator.SetFloat("InputMagnitude", inputManager.InputMag, startAnimTime, Time.deltaTime);
        } else if(inputManager.InputMag < 0.01f)
        {
            animator.SetFloat("InputMagnitude", inputManager.InputMag, stopAnimTime, Time.deltaTime);
        }
    }

    private void RotateView()
    {
        mouseLook.LookRotation(transform, cam2);
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}
