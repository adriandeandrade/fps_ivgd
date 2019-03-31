using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDownSights : MonoBehaviour
{
    Vector3 originalPosition;
    [SerializeField] private Vector3 aimPosition;
    [SerializeField] private float aimDownSightSpeed = 8f;

    [SerializeField] private Weapon currentGun;

    PlayerMotor playerMotor;
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        playerMotor = GetComponentInParent<PlayerMotor>();
    }

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        AimDownSight();
    }

    private void AimDownSight()
    {
        if (InputManager.instance.ADS > 0 && !player.IsReloading && !playerMotor.IsHittingWall)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, aimDownSightSpeed * Time.deltaTime);
            player.IsAimingDownSights = true;
        }

        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, aimDownSightSpeed * Time.deltaTime);
            player.IsAimingDownSights = false;
        }
    }
}
