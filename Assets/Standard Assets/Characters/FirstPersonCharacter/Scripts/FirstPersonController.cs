using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using Cinemachine;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Controller Properties")]
        [SerializeField] private bool isWalking;
        [SerializeField] private bool useFovKick;
        [SerializeField] private bool useHeadBob;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] [Range(0f, 1f)] private float runStepLengthen;
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float stickToGroundForce;
        [SerializeField] private float stepInterval;
        [SerializeField] private float gravityModifier;

        [Header("Components")]
        [SerializeField] private MouseLook mouseLook;
        [SerializeField] private FOVKick fovKick = new FOVKick();
        [SerializeField] private CurveControlledBob headBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob jumpBob = new LerpControlledBob();
        [SerializeField] private AudioClip[] footStepSound;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip jumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip landSound;           // the sound played when character touches back on ground.

        Vector3 originalCameraPosition;
        Vector2 input;
        Vector3 moveDirection = Vector3.zero;

        private bool jump;
        bool isJumping;
        bool wasGrounded;
        
        float stepCycle;
        float nextStep;
        float yRotation;

        Camera cam;
        CharacterController controller;
        CinemachineBrain cinemachineBrain;
        CollisionFlags collisionFlags;
        AudioSource audioSource;

        private void Awake()
        {
            cinemachineBrain = GetComponentInChildren<CinemachineBrain>();
        }

        // Use this for initialization
        private void Start()
        {
            controller = GetComponent<CharacterController>();
            cam = Camera.main;
            originalCameraPosition = cam.transform.localPosition;
            fovKick.Setup(cam);
            headBob.Setup(cam, stepInterval);
            stepCycle = 0f;
            nextStep = stepCycle / 2f;
            isJumping = false;
            audioSource = GetComponent<AudioSource>();
            mouseLook.Init(transform, cam.transform);
            cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
        }


        // Update is called once per frame
        private void Update()
        {
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!jump)
            {
                jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!wasGrounded && controller.isGrounded)
            {
                StartCoroutine(jumpBob.DoBobCycle());
                PlayLandingSound();
                moveDirection.y = 0f;
                isJumping = false;
            }
            if (!controller.isGrounded && !isJumping && wasGrounded)
            {
                moveDirection.y = 0f;
            }

            wasGrounded = controller.isGrounded;
        }


        private void PlayLandingSound()
        {
            audioSource.clip = landSound;
            audioSource.Play();
            nextStep = stepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * input.y + transform.right * input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, controller.radius, Vector3.down, out hitInfo,
                               controller.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            moveDirection.x = desiredMove.x * speed;
            moveDirection.z = desiredMove.z * speed;


            if (controller.isGrounded)
            {
                moveDirection.y = -stickToGroundForce;

                if (jump)
                {
                    moveDirection.y = jumpSpeed;
                    PlayJumpSound();
                    jump = false;
                    isJumping = true;
                }
            }
            else
            {
                moveDirection += Physics.gravity * gravityModifier * Time.fixedDeltaTime;
            }
            collisionFlags = controller.Move(moveDirection * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            mouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            audioSource.clip = jumpSound;
            audioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (controller.velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0))
            {
                stepCycle += (controller.velocity.magnitude + (speed * (isWalking ? 1f : runStepLengthen))) *
                             Time.fixedDeltaTime;
            }

            if (!(stepCycle > nextStep))
            {
                return;
            }

            nextStep = stepCycle + stepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!controller.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, footStepSound.Length);
            audioSource.clip = footStepSound[n];
            audioSource.PlayOneShot(audioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            footStepSound[n] = footStepSound[0];
            footStepSound[0] = audioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!useHeadBob)
            {
                return;
            }
            if (controller.velocity.magnitude > 0 && controller.isGrounded)
            {
                cam.transform.localPosition =
                    headBob.DoHeadBob(controller.velocity.magnitude +
                                      (speed * (isWalking ? 1f : runStepLengthen)));
                newCameraPosition = cam.transform.localPosition;
                newCameraPosition.y = cam.transform.localPosition.y - jumpBob.Offset();
            }
            else
            {
                newCameraPosition = cam.transform.localPosition;
                newCameraPosition.y = originalCameraPosition.y - jumpBob.Offset();
            }
            cam.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = isWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            isWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = isWalking ? walkSpeed : runSpeed;
            input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (input.sqrMagnitude > 1)
            {
                input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (isWalking != waswalking && useFovKick && controller.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!isWalking ? fovKick.FOVKickUp() : fovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            mouseLook.LookRotation(transform, cam.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (collisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(controller.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
