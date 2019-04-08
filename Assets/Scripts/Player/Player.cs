using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform primarySlotTransform;
    [SerializeField] private Transform secondarySlotTransform;
    [SerializeField] private TextMeshProUGUI ammoText;

    [Space]

    [SerializeField] private WeaponSlot primarySlot;
    [SerializeField] private WeaponSlot secondarySlot;

    // Private Variables

    bool isReloading;
    bool isAimingDownSights;
    bool isHoldingWeapon;

    // Properties
    public Weapon CurrentWeapon { get { return currentWeapon; } set { currentWeapon = value; } }
    public bool IsReloading { get { return isReloading; } set { isReloading = value; } }
    public bool IsAimingDownSights { get { return isAimingDownSights; } set { isAimingDownSights = value; } }
    public bool IsHoldingWeapon { get => isHoldingWeapon; set => isHoldingWeapon = value; }
    public TextMeshProUGUI AmmoText { get => ammoText; set => ammoText = value; }

    // Components
    WeaponSlot currentSlot;
    Weapon currentWeapon;
    GameObject currentArms;

    private void Awake()
    {
        InitWeaponSlots();
    }

    private void Start()
    {
        isReloading = false;
        isAimingDownSights = false;
    }

    private void Update()
    {
        CycleInventory();
    }

    private void InitWeaponSlots()
    {
        primarySlot = new WeaponSlot(this, primarySlotTransform, 2);
        secondarySlot = new WeaponSlot(this, secondarySlotTransform, 1);

        primarySlot.InitSlot();
        secondarySlot.InitSlot();

        currentSlot = secondarySlot;
        currentWeapon = currentSlot.GetWeapon();
        currentArms = currentSlot.GetCurrentArms();
        primarySlotTransform.gameObject.SetActive(false);
    }

    public void AddWeapon(WeaponData newWeaponData, GameObject weaponArms)
    {
        WeaponSlot slotToAddTo = newWeaponData.weaponType == WeaponType.PRIMARY ? primarySlot : secondarySlot; // Check which slot we want to add the weapon too.

        if (slotToAddTo.weaponsInSlot.Count == 1)
        {
            if (slotToAddTo.CanAddWeapon()) // If we can add the weapon
            {
                Transform slot = slotToAddTo.GetSlotTransform();
                GameObject newWeapon = Instantiate(newWeaponData.weaponArms, slot);

                if (slotToAddTo.weaponsInSlot.Count <= 0)
                {
                    newWeapon.SetActive(true);
                }

                slotToAddTo.AddWeaponToSlot(newWeapon);
            }
        }
        else if (slotToAddTo.CanAddWeapon()) // If we can add the weapon
        {
            Transform slot = slotToAddTo.GetSlotTransform();
            GameObject newWeapon = Instantiate(newWeaponData.weaponArms, slot);

            if (slotToAddTo.weaponsInSlot.Count <= 0)
            {
                newWeapon.SetActive(true);
            }

            slotToAddTo.AddWeaponToSlot(newWeapon);
            slotToAddTo.UpdateSlot(weaponArms);
        }
    }

    public void CycleInventory()
    {

        if (Input.GetButtonDown(InputManager.instance.switchWeaponButtonName) && CanCycleInventory())
        {
            if (isReloading) currentWeapon.CancelReload();
            if (currentSlot == primarySlot)
            {
                // Switch to Secondary Guns
                currentSlot = secondarySlot;
                primarySlotTransform.gameObject.SetActive(false);
                secondarySlotTransform.gameObject.SetActive(true);
                //secondarySlot.GetSlotTransform().GetChild(0).gameObject.SetActive(true);
                UpdateInventory();
            }
            else if (currentSlot == secondarySlot)
            {
                // Switch to Primary Guns
                currentSlot = primarySlot;
                primarySlotTransform.gameObject.SetActive(true);
                secondarySlotTransform.gameObject.SetActive(false);
                //primarySlot.GetSlotTransform().GetChild(0).gameObject.SetActive(true);
                UpdateInventory();
            }

        }
        else if (Input.GetButtonDown(InputManager.instance.cycleGunsInSlotButtonName))
        {
            CycleSlot();
        }
    }

    private bool CanCycleInventory()
    {
        return primarySlot.GetSlotTransform().childCount > 0 && secondarySlot.GetSlotTransform().childCount > 0;
    }

    public void CycleSlot()
    {
        if (isReloading) currentWeapon.CancelReload();

        currentSlot.SwitchWeaponInSlot();
    }

    public void UpdateInventory()
    {
        currentWeapon = currentSlot.GetWeapon();
        currentArms = currentSlot.GetCurrentArms();
    }
}

[System.Serializable]
public class WeaponSlot
{
    public GameObject currentArms;
    public Weapon currentWeaponInSlot;
    public List<GameObject> weaponsInSlot;

    int maxGuns;
    int weaponIndex;

    Player player;
    Transform slot;

    public WeaponSlot(Player _player, Transform _slotTransform, int _maxGuns)
    {
        player = _player;
        slot = _slotTransform;
        maxGuns = _maxGuns;
        weaponsInSlot = new List<GameObject>();
    }

    public void InitSlot()
    {
        foreach (Transform child in slot) // Look through all the children of this slot.
        {
            weaponsInSlot.Add(child.gameObject); // Add them all to a list.
            child.gameObject.SetActive(false); // Turn them off by default.
        }

        if (slot.childCount > 0)
        {
            slot.GetChild(0).gameObject.SetActive(true);
            currentWeaponInSlot = slot.GetChild(0).gameObject.GetComponentInChildren<Weapon>();
            currentArms = slot.GetChild(0).gameObject;
        }
    }

    public Transform GetSlotTransform()
    {
        return slot;
    }

    public void UpdateSlotData(Weapon newWeapon, GameObject newArms)
    {
        currentArms = newArms;
        currentWeaponInSlot = newArms.GetComponentInChildren<Weapon>();
        currentArms.SetActive(true);
        Debug.Log("New Arms turned on.");

        player.UpdateInventory();
    }

    public void UpdateSlot(GameObject newArms)
    {
        if (slot.childCount > 0)
        {
            currentArms = newArms;
            currentWeaponInSlot = newArms.GetComponentInChildren<Weapon>();
        }
    }

    public void SwitchWeaponInSlot()
    {
        if (weaponsInSlot.Count <= 1) return; // Returns if we dont have more than one weapon in the slot.
        
        weaponsInSlot[weaponIndex].SetActive(false);
        Debug.Log("Old Arms turned off.: " + currentArms.name);

        if (weaponIndex >= weaponsInSlot.Count - 1)
        {
            weaponIndex = 0;
        }
        else
        {
            weaponIndex++;
        }

        Weapon newWeapon = weaponsInSlot[weaponIndex].GetComponentInChildren<Weapon>();
        GameObject newArms = weaponsInSlot[weaponIndex].gameObject;


        UpdateSlotData(newWeapon, newArms);
    }

    public GameObject GetCurrentArms()
    {
        return currentArms;
    }

    public Weapon GetWeapon()
    {
        return currentWeaponInSlot;
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
        //gunToAdd.gameObject.SetActive(false);
    }
}