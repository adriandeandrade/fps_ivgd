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
    }

    private void Update()
    {
        if (currentlyEquippedGun != null)
        {
            HandleCurrentWeapon();
        }

        CycleInventory();
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

    public void CycleInventory()
    {
        if(Input.GetButtonDown(InputManager.instance.cyclePrimaryButtonName))
        {
            if (InputManager.instance.CheckDoubleTap(InputManager.instance.cyclePrimaryButtonName))
            {
                CycleSlot();
                return;
            }
            else
            {
                if (currentSlot == primarySlot)
                {
                    // Switch to Secondary Guns
                    currentSlot = secondarySlot;
                    primarySlot.slot.gameObject.SetActive(false);
                    secondarySlot.slot.gameObject.SetActive(true);
                    Debug.Log("Switched To Secondary");
                }
                else if (currentSlot == secondarySlot)
                {
                    // Switch to Primary Guns
                    currentSlot = primarySlot;
                    primarySlot.slot.gameObject.SetActive(true);
                    secondarySlot.slot.gameObject.SetActive(false);
                    Debug.Log("Switched To Primary");
                }
            }
        }
    }

    public void CycleSlot()
    {
        //if (currentSlot == primarySlot)
        //{
        //    primarySlot.SwitchWeaponInSlot();
        //}
        //else if (currentSlot == secondarySlot)
        //{
        //    secondarySlot.SwitchWeaponInSlot();
        //}

        Debug.Log("Cycled.");
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