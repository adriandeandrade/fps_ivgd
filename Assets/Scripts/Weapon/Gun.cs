using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunData gunData;
    [SerializeField] private GameObject debugItem;

    float bulletsLeft;
    float nextFireTime;

    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (Time.time > nextFireTime)
        {
            RaycastHit hit;
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, gunData.fireRange))
            {
                Instantiate(debugItem, hit.point, Quaternion.identity);
                Debug.Log("Fire Weapon");
            }

            nextFireTime = Time.time + gunData.fireRate;
        }
    }
}
