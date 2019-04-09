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

    MeshRenderer meshRenderer;

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

            if(pickup.isWeapon)
            {
                UpdatePickupUI(pickup.weaponData.weaponName);

                if (Input.GetKeyDown(KeyCode.G))
                {
                    player.AddWeapon(pickup.weaponData, pickup.weaponData.weaponArms);
                    //Destroy(pickup.gameObject);
                }
            } else
            {
                UpdatePickupUI(pickup.itemName);
            }

            EnablePickupUI();
            GetMeshRenderer(hit);
            if (meshRenderer != null)
            {
                meshRenderer.material.color = Color.red;
            }
        }
        else
        {
            DisablePickupUI();
            if (meshRenderer != null)
            {
                meshRenderer.material.color = Color.white;
            }
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

    private void GetMeshRenderer(RaycastHit hit)
    {
        MeshRenderer mr = hit.collider.GetComponent<MeshRenderer>();

        if(mr != null)
        {
            meshRenderer = mr;
            return;
        }

        mr = hit.collider.GetComponentInChildren<MeshRenderer>();

        if(mr != null)
        {
            meshRenderer = mr;
            return;
        }
    }
}
