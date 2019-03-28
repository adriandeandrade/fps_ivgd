using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Gun currentlyEquippedGun;

    private void Start()
    {
        currentlyEquippedGun = GetComponentInChildren<Gun>();
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            currentlyEquippedGun.OnTriggerHold();
        }

        if(Input.GetMouseButtonUp(0))
        {
            currentlyEquippedGun.OnTriggerReleased();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            currentlyEquippedGun.Reload();
        }
    }
}
