using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Weapon currentlyEquippedGun;

    // Private Variables
    bool isReloading;
    bool isAimingDownSights;
    bool triggerReleasedSinceLastShot;

    // Properties
    public Weapon CurrentlyEquippedGun { get { return currentlyEquippedGun; } set { currentlyEquippedGun = value; } }
    public Animator ArmsAnimator { get { return armsAnimator; } private set { } }
    public bool IsReloading { get { return isReloading; } set { isReloading = value; } }
    public bool IsAimingDownSights { get { return isAimingDownSights; } set { isAimingDownSights = value; } }

    public bool TriggerReleasedSinceLastShot { get => triggerReleasedSinceLastShot; set => triggerReleasedSinceLastShot = value; }

    // Components
    Animator armsAnimator;
    Animator currentWeaponAnimator;

    private void Awake()
    {
        currentlyEquippedGun = GetComponentInChildren<Weapon>();

        armsAnimator = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();

        if (currentlyEquippedGun != null)
        {
            currentWeaponAnimator = GetComponentInChildren<Weapon>().GetComponent<Animator>();
        }
    }

    private void Start()
    {
        isReloading = false;
        isAimingDownSights = false;
    }

    private void Update()
    {
        if (currentlyEquippedGun != null)
        {
            HandleCurrentWeapon();
        }
    }

    private void HandleCurrentWeapon()
    {
        armsAnimator.SetBool("IsAiming", isAimingDownSights);

        if (InputManager.instance.Shoot > 0) OnTriggerHold();
        if (InputManager.instance.Shoot <= 0) OnTriggerReleased();

    }

    public void OnTriggerHold()
    {
        currentlyEquippedGun.Shoot();
        TriggerReleasedSinceLastShot = false;
    }

    public void OnTriggerReleased()
    {
        TriggerReleasedSinceLastShot = true;
        currentlyEquippedGun.ResetBurst();
    }
}
