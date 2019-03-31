using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AimDownSights : MonoBehaviour
{
    Vector3 originalPosition;
    [SerializeField] private Vector3 aimPosition;
    [SerializeField] private float aimDownSightSpeed = 8f;
    [SerializeField] private float changeFovSpeed;

    [SerializeField] private Weapon currentGun;

    float originalFov;
    float desiredFov;

    PlayerMotor playerMotor;
    Player player;

    CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        playerMotor = GetComponentInParent<PlayerMotor>();
        virtualCamera = GetComponentInParent<CinemachineVirtualCamera>();
        originalFov = virtualCamera.m_Lens.FieldOfView;
        desiredFov = originalFov - 20f;
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
            float targetFov = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, desiredFov, changeFovSpeed * Time.deltaTime);
            virtualCamera.m_Lens.FieldOfView = targetFov;
            player.IsAimingDownSights = true;
        }

        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, aimDownSightSpeed * Time.deltaTime);
            float targetFov = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, originalFov, changeFovSpeed * Time.deltaTime);
            virtualCamera.m_Lens.FieldOfView = targetFov;
            player.IsAimingDownSights = false;

        }
    }
}
