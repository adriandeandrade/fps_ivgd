using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Weapon currentlyEquippedGun;

    [SerializeField] private WeaponSlot primarySlot;
    [SerializeField] private WeaponSlot secondarySlot;

    // Private Variables
    bool isReloading;
    bool isAimingDownSights;
    bool triggerReleasedSinceLastShot;
    WeaponSlot currentSlot;

    // Properties
    public Weapon CurrentlyEquippedGun { get { return currentlyEquippedGun; } set { currentlyEquippedGun = value; } }
    public Animator ArmsAnimator { get { return armsAnimator; } private set { } }
    public bool IsReloading { get { return isReloading; } set { isReloading = value; } }
    public bool IsAimingDownSights { get { return isAimingDownSights; } set { isAimingDownSights = value; } }

    public bool TriggerReleasedSinceLastShot { get => triggerReleasedSinceLastShot; set => triggerReleasedSinceLastShot = value; }

    // Components
    Animator armsAnimator;
    Crosshair crosshair;


    private void Awake()
    {
        currentlyEquippedGun = primarySlot.weaponsInSlot[0].gameObject.GetComponentInChildren<Weapon>();
        armsAnimator = currentlyEquippedGun.gameObject.GetComponentInParent<Animator>();
        crosshair = FindObjectOfType<Crosshair>();
    }

    private void Start()
    {
        isReloading = false;
        isAimingDownSights = false;
        currentSlot = primarySlot;

        primarySlot.currentWeapon = primarySlot.weaponsInSlot[0];
    }

    private void Update()
    {
        if (currentlyEquippedGun != null)
        {
            HandleCurrentWeapon();
        }

        currentSlot.SwitchWeaponInSlot();
        SwitchWeapon();
    }

    private void HandleCurrentWeapon()
    {
        if (isAimingDownSights)
        {
            armsAnimator.SetBool("IsAiming", true);
            crosshair.ToggleCrosshair(false);
        }
        else if (!isAimingDownSights)
        {
            armsAnimator.SetBool("IsAiming", false);
            crosshair.ToggleCrosshair(true);
        }



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

    private void SwitchWeapon()
    {
        if (Input.GetButtonDown(InputManager.instance.switchWeaponButtonName) && !isAimingDownSights)
        {
            if (isReloading)
            {
                currentlyEquippedGun.CancelReload();
            }

            if (currentSlot == primarySlot)
            {
                primarySlot.slot.gameObject.SetActive(false);
                secondarySlot.slot.gameObject.SetActive(true);
                currentSlot = secondarySlot;
                currentlyEquippedGun = secondarySlot.weaponsInSlot[secondarySlot.weaponIndex].gameObject.GetComponentInChildren<Weapon>();
                armsAnimator = currentlyEquippedGun.gameObject.GetComponentInParent<Animator>();
            }
            else if (currentSlot == secondarySlot)
            {
                secondarySlot.slot.gameObject.SetActive(false);
                primarySlot.slot.gameObject.SetActive(true);
                currentSlot = primarySlot;
                currentlyEquippedGun = primarySlot.weaponsInSlot[primarySlot.weaponIndex].gameObject.GetComponentInChildren<Weapon>();
                armsAnimator = currentlyEquippedGun.gameObject.GetComponentInParent<Animator>();
            }
        }
    }

    private void SwitchSlot()
    {
        if (!isAimingDownSights)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (isReloading)
                {
                    currentlyEquippedGun.CancelReload();
                }

                currentSlot.SwitchWeaponInSlot();
                UpdatePlayer();
            }
        }
    }

    private void UpdatePlayer()
    {
        currentlyEquippedGun = currentSlot.currentlySelectedWeaponInSlot;
        armsAnimator = currentlyEquippedGun.gameObject.GetComponentInParent<Animator>();
    }

    public void AddWeapon(WeaponData weaponToAdd)
    {
        WeaponSlot slot = weaponToAdd.weaponType == WeaponType.PRIMARY ? primarySlot : secondarySlot;
        if (!slot.CheckWeaponExists(weaponToAdd.weaponArms))
        {
            if(slot.CanAddWeapon())
            {
                GameObject newWeapon = Instantiate(weaponToAdd.weaponArms, primarySlot.slot);
                newWeapon.SetActive(false);
                slot.AddWeaponToSlot(newWeapon);
            } else
            {
                Debug.Log("Cant add weapon.");
            }
        }
    }
}

[System.Serializable]
public class WeaponSlot
{
    [SerializeField] private Player player;
    public Transform slot;
    public GameObject currentWeapon;
    public Weapon currentlySelectedWeaponInSlot;
    public List<GameObject> weaponsInSlot;
    public int weaponIndex;
    public int maxGuns;

    public void SwitchWeaponInSlot()
    {
        if (weaponsInSlot.Count <= 1) return; // Returns if we dont have more than one weapon in the slot.

        currentWeapon.gameObject.SetActive(false);

        if (weaponIndex >= weaponsInSlot.Count - 1)
        {
            weaponIndex = 0;
        }
        else
        {
            weaponIndex++;
        }

        currentWeapon = weaponsInSlot[weaponIndex].gameObject;
        player.CurrentlyEquippedGun = currentWeapon.gameObject.GetComponentInChildren<Weapon>();
        currentWeapon.gameObject.SetActive(true);
        //currentlySelectedWeaponInSlot.gameObject.SetActive(true);

        
    }

    public bool CheckWeaponExists(GameObject weaponToCheck)
    {
        foreach (GameObject weapon in weaponsInSlot)
        {
            if (weaponToCheck == weapon) return true;
        }

        return false;
    }

    public bool CanAddWeapon()
    {
        if (weaponsInSlot.Count >= maxGuns)
        {
            return false;
        }

        return true;
    }

    public void AddWeaponToSlot(GameObject gunToAdd)
    {
        weaponsInSlot.Add(gunToAdd);
    }
}