using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunData gunData;
    [SerializeField] private GameObject debugItem;

    int bulletsLeft;
    int shotsRemainingInBurst;
    float nextFireTime;

    bool isReloading;
    bool triggerReleasedSinceLastShot;

    Camera cam;
    FireTypes fireType;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        fireType = gunData.fireType;
    }

    private void LateUpdate()
    {
        if(!isReloading && bulletsLeft == 0)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        if (!isReloading && Time.time > nextFireTime && bulletsLeft > 0)
        {
            if (fireType == FireTypes.BURST)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireType == FireTypes.SINGLE)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            ShootRay();
            bulletsLeft--;
            nextFireTime = Time.time + gunData.fireRate;
        }
    }

    public void Reload()
    {
        if(!isReloading && bulletsLeft != gunData.magazineCapacity)
        {
            StartCoroutine(AnimateReload());
        }
    }

    IEnumerator AnimateReload()
    {
        Debug.Log("Reloading...");

        isReloading = true;
        yield return new WaitForSeconds(0.2f);

        float reloadSpeed = 1f / gunData.reloadTime;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            yield return null;
        }

        isReloading = false;
        bulletsLeft = gunData.magazineCapacity;
        Debug.Log("Finished Reloading!");
    }

    private void ShootRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, gunData.fireRange))
        {
            Instantiate(debugItem, hit.point, Quaternion.identity);
            Debug.Log("Fire Weapon");
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerReleased()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = gunData.burstCount;
    }
}
