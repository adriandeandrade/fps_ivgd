using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Input Manager Configuration")]
    [SerializeField] private bool useRaw;
    [SerializeField] private bool handleMovement;

    [Header("Inputs")]
    [SerializeField] private string xAxisName;
    [SerializeField] private string yAxisName;
    [SerializeField] private string sprintButtonName;
    [SerializeField] private string jumpButtonName;

    float horizontal;
    float vertical;
    float inputMagnitude;

    Vector3 movement;

    PlayerMotor playerMotor;

    public Vector3 Movement { get { return movement; } private set { } }
    public float InputMag { get { return inputMagnitude; } private set { } }

    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        if (handleMovement)
        {
            if (useRaw)
            {
                movement = new Vector3(Input.GetAxisRaw(xAxisName), 0, Input.GetAxisRaw(yAxisName));
                movement = movement.normalized;
            }
            else
            {
                movement = new Vector3(Input.GetAxis(xAxisName), 0, Input.GetAxis(yAxisName));
                movement = movement.normalized;
            }

            InputMagnitude(movement);

            if (Input.GetButtonDown(jumpButtonName))
            {
                playerMotor.IsJumping = true;
            }

            if (Input.GetButton(sprintButtonName))
            {
                playerMotor.IsSprinting = true;
                inputMagnitude *= 2;
            }
            else if (Input.GetButtonUp(sprintButtonName))
            {
                playerMotor.IsSprinting = false;
            }
        }
    }

    private void InputMagnitude(Vector3 input)
    {
        inputMagnitude = new Vector2(movement.x, movement.z).sqrMagnitude;
    }
}
