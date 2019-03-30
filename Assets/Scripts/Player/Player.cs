using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Gun currentlyEquippedGun;

    public Gun CurrentlyEquippedGun { get { return currentlyEquippedGun; } set { currentlyEquippedGun = value; } }

    private void Start()
    {
        currentlyEquippedGun = GetComponentInChildren<Gun>();
    }

    private void Update()
    {
        if (InputManager.instance.Shoot > 0)
        {
            currentlyEquippedGun.OnTriggerHold();
        }

        if (InputManager.instance.Shoot <= 0)
        {
            currentlyEquippedGun.OnTriggerReleased();
        }
    }
}
