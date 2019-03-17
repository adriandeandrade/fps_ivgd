using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float jumpAmount;
    [SerializeField] private int maxJumps;
    [SerializeField] private float gravity;
    [SerializeField] private bool useDampening;

    Vector3 movement;
    Vector3 moveDirection;

    float inputX;
    float inputZ;

    int currentJumps;

    CharacterController characterController;
    Camera cam;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    private void Update()
    {
        //RotateToFaceCameraForward();

        Movement();
    }

    private void Movement()
    {
        if (characterController.isGrounded)
        {
            inputX = Input.GetAxis("Horizontal");
            inputZ = Input.GetAxis("Vertical");

            Vector3 movementInput = new Vector3(inputX, 0, inputZ);
            movement = transform.TransformDirection(movementInput);

            if (Input.GetButtonDown("Jump"))
            {
                currentJumps++;
                movement.y = jumpAmount;
            }
        }

        if (!characterController.isGrounded && currentJumps <= maxJumps)
        {
            if (Input.GetButtonDown("Jump"))
            {
                currentJumps++;
                movement.y = jumpAmount;
            }
        }

        movement.y = movement.y - (gravity * Time.deltaTime);
        characterController.Move(movement * moveSpeed * Time.deltaTime);
    }

    //private void RotateToFaceCameraForward()
    //{
    //    Vector3 cameraForward = cam.transform.forward;
    //    cameraForward.y = 0f;
    //    moveDirection = cameraForward.normalized;

    //    if (useDampening)
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), rotateSpeed);
    //    else
    //        transform.rotation = Quaternion.LookRotation(moveDirection);
    //}

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            currentJumps = 0;
        }
    }
}
