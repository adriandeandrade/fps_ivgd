using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    // Public Variables
    [SerializeField] private float maxInteractDistance;
    [SerializeField] private GameObject playerPickupUI;
    [SerializeField] private TextMeshProUGUI pickupText;
    [SerializeField] private LayerMask pickupMask;
    public bool doPickup = true;

    // Components
    Player player;
    Camera cam;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        cam = Camera.main;
        DisablePickupUI();
    }

    private void Update()
    {
        if (!doPickup) return;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxInteractDistance, pickupMask))
        {
            PickupItem pickup = hit.collider.GetComponent<PickupItem>();
            UpdatePickupUI(pickup.weaponData.weaponName);

            if(Input.GetKeyDown(KeyCode.G))
            {
                player.AddWeapon(pickup.weaponData, pickup.weaponData.weaponArms);
                //Destroy(pickup.gameObject);
            }

            EnablePickupUI();
        }
        else
        {
            DisablePickupUI();
        }
    }

    private void UpdatePickupUI(string hitName)
    {
        pickupText.text = "Interact With " + hitName;
    }

    private void EnablePickupUI()
    {
        playerPickupUI.gameObject.SetActive(true);
    }

    private void DisablePickupUI()
    {
        playerPickupUI.gameObject.SetActive(false);
    }
}
