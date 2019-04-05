using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMotor : MonoBehaviour
{
    // Public Variables
    [Header("Speed Variables")]
    [SerializeField] private float sprintSpeed = 15.0f;
    [SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private float speedWhileAiming = 8f;

    [Header("Other Configuration")]
    [SerializeField] private float gravity = 10.0f;
    [SerializeField] private float maxVelocityChange = 10.0f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float maxDistanceFromWall = 5f;
    [SerializeField] private Transform cam2;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private LayerMask groundMask;

    

    public bool IsSprinting { get { return isSprinting; } set { isSprinting = value; } }
    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
    public bool CanSprint { get { return canSprint; } set { canSprint = value; } }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }

    bool isGrounded = false;
    bool isSprinting;
    bool isReloading;
    bool isJumping;
    bool isShooting;
    bool isAiming;
    bool canSprint;

    float horizontal;
    float vertical;
    const float groundedRadius = 0.2f;

    Vector3 originalWeaponPosition;

    Player player;
    Rigidbody rBody;
    Camera cam;

    void Awake()
    {
        player = GetComponent<Player>();
        rBody = GetComponent<Rigidbody>();
        cam = Camera.main;
        rBody.freezeRotation = true;
        rBody.useGravity = false;
    }

    private void Start()
    {
        mouseLook = new MouseLook();
        mouseLook.Init(transform, cam.transform);
    }

    private void Update()
    {
        if (player.IsAimingDownSights && !player.IsReloading) { canSprint = false; } else if (!player.IsAimingDownSights && !player.IsReloading) { canSprint = true; }

        //HandleMovementAnimations();
        RotateView();
    }

    void FixedUpdate()
    {
        Movement();
        CheckForGround();
    }

    private void Movement()
    {
        if (IsGrounded)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = InputManager.instance.Movement;
            targetVelocity = transform.TransformDirection(targetVelocity);

            if (isSprinting)
                targetVelocity *= sprintSpeed;
            else if (!isSprinting && !isAiming)
                targetVelocity *= walkSpeed;
            else
                targetVelocity *= speedWhileAiming;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rBody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rBody.AddForce(velocityChange, ForceMode.VelocityChange);

            if (isJumping)
            {
                rBody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                isJumping = false;
            }
        }

        // We apply gravity manually for more tuning control
        rBody.AddForce(new Vector3(0, -gravity * rBody.mass, 0));

        mouseLook.UpdateCursorLock();
    }

    private void CheckForGround()
    {
        IsGrounded = false;
        Collider[] groundColliders = Physics.OverlapSphere(groundCheck.position, groundedRadius, groundMask);

        for (int i = 0; i < groundColliders.Length; i++)
        {
            if(groundColliders[i].gameObject != this.gameObject)
            {
                IsGrounded = true;
            }
        }
    }

    

    private void RotateView()
    {
        if (!InputManager.instance.HandleMovement) return;
        mouseLook.LookRotation(transform, cam2);
    }

    void OnCollisionStay()
    {
        IsGrounded = true;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}
