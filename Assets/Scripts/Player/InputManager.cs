using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager instance;

    private void InitInputManager()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }


    }
    #endregion

    [Header("Input Manager Configuration")]
    [SerializeField] private bool useRaw;
    [SerializeField] private bool handleMovement;

    [Header("Inputs")]
    [SerializeField] private string xAxisName;
    [SerializeField] private string yAxisName;
    [SerializeField] private string sprintButtonName;
    [SerializeField] private string jumpButtonName;
    [SerializeField] public string adsButtonName;
    [SerializeField] public string primaryAttackButtonName;
    [SerializeField] public string reloadButton;

    float horizontal;
    float vertical;
    float ads;
    float shoot;
    float inputMagnitude;


    Vector3 movement;

    PlayerMotor playerMotor;
    Player player;

    public Vector3 Movement { get { return movement; } private set { } }
    public float InputMag { get { return inputMagnitude; } private set { } }
    public float ADS { get { return ads; } set { ads = value; } }
    public float Shoot { get { return shoot; } set { ads = value; } }

    private void Awake()
    {
        InitInputManager();
        playerMotor = FindObjectOfType<PlayerMotor>();
        player = playerMotor.gameObject.GetComponent<Player>();
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

            shoot = Input.GetAxis(primaryAttackButtonName);
            ads = Input.GetAxis(adsButtonName);

            if (Input.GetButtonDown(jumpButtonName))
            {
                playerMotor.IsJumping = true;
            }

            if (Input.GetButton(sprintButtonName) && playerMotor.CanSprint)
            {
                playerMotor.IsSprinting = true;
                inputMagnitude *= 2;
            }
            else if (Input.GetButtonUp(sprintButtonName))
            {
                playerMotor.IsSprinting = false;
            }

            if(ads > 0 && Input.GetButton(sprintButtonName))
            {
                player.CurrentlyEquippedGun.IsAimingDownSights = false;
                playerMotor.IsAiming = false;
                playerMotor.IsSprinting = true;
            }

            if (ads > 0 && playerMotor.IsSprinting)
            {
                player.CurrentlyEquippedGun.IsAimingDownSights = true;
                playerMotor.IsAiming = true;
                playerMotor.IsSprinting = false;
            }
            else if (ads <= 0)
            {
                player.CurrentlyEquippedGun.IsAimingDownSights = false;
                playerMotor.IsAiming = false;
            }

            if (Input.GetButtonDown(reloadButton))
            {
                player.CurrentlyEquippedGun.Reload();
            } else if(Input.GetButtonDown(reloadButton) && playerMotor.IsSprinting)
            {
                playerMotor.IsSprinting = false;
                player.CurrentlyEquippedGun.Reload();
            }

        }
    }

    private void InputMagnitude(Vector3 input)
    {
        inputMagnitude = new Vector2(movement.x, movement.z).sqrMagnitude;
    }
}
