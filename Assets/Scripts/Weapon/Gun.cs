using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GunData gunData;
    [SerializeField] private GameObject debugItem;
    [SerializeField] private LayerMask ignoreMask;

    public bool IsReloading { get { return isReloading; } private set { } }
    public bool IsAimingDownSights { get { return isAimingDownSights; } set { isAimingDownSights = value; } }

    int bulletsLeft;
    int shotsRemainingInBurst;
    float nextFireTime;

    bool isReloading;
    bool isAimingDownSights;
    bool triggerReleasedSinceLastShot;

    Camera cam;
    FireTypes fireType;
    Animator animator;
    Animator arms;
    PlayerMotor playerMotor;

    private void Awake()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        playerMotor = GetComponentInParent<PlayerMotor>();
        arms = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
    }

    private void Start()
    {
        fireType = gunData.fireType;
        bulletsLeft = gunData.magazineCapacity;
    }

    private void Update()
    {
        arms.SetBool("IsAiming", isAimingDownSights);
    }

    private void FixedUpdate()
    {
        HandleAnimationSynchronization();
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
        if (CanShoot())
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

            animator.CrossFadeInFixedTime("shoot", gunData.fireRate);
            arms.CrossFadeInFixedTime("shoot", gunData.fireRate);

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

    public bool CanShoot()
    {
        if(!isReloading && Time.time > nextFireTime && bulletsLeft > 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    private void HandleAnimationSynchronization()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo armsInfo = arms.GetCurrentAnimatorStateInfo(0);

        if (info.IsName("shoot"))
        {
            animator.SetBool("Shoot", false);
        }

        if (armsInfo.IsName("shoot"))
        {
            arms.SetBool("Shoot", false);
        }
    }

    IEnumerator AnimateReload()
    {
        Debug.Log("Reloading...");

        playerMotor.IsReloading = true;
        isReloading = true;
        yield return new WaitForSeconds(0.2f);

        animator.SetTrigger("Reload");

        float reloadSpeed = 1f / gunData.reloadTime;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            yield return null;
        }

        isReloading = false;
        playerMotor.IsReloading = false;
        bulletsLeft = gunData.magazineCapacity;
        Debug.Log("Finished Reloading!");
    }

    private void ShootRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, gunData.fireRange, ignoreMask))
        {
            Instantiate(debugItem, hit.point, Quaternion.identity);
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
