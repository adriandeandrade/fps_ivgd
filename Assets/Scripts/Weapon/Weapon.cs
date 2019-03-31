using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //Public Variables
    [SerializeField] private WeaponData gunData;
    [SerializeField] private GameObject debugItem;
    [SerializeField] private LayerMask ignoreMask;

    enum WeaponStates { READY, RELOAD };
    WeaponStates weaponState;

    // Private Variables
    int bulletsLeft;
    int shotsRemainingInBurst;
    float nextFireTime;

    // Components
    Camera cam;
    FireTypes fireType;
    //Animator animator;
    Animator arms;
    PlayerMotor playerMotor;
    Player player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        cam = Camera.main;
        //animator = GetComponent<Animator>();
        playerMotor = GetComponentInParent<PlayerMotor>();
        arms = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
    }

    private void Start()
    {
        fireType = gunData.fireType;
        bulletsLeft = gunData.magazineCapacity;
    }

    private void FixedUpdate()
    {
        HandleAnimationSynchronization();
    }

    private void LateUpdate()
    {
        if(!player.IsReloading && bulletsLeft == 0)
        {
            Reload();
        }
    }

    public void Shoot()
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
                if (!player.TriggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            arms.CrossFadeInFixedTime("shoot", 0.01f);

            ShootRay();
            bulletsLeft--;
            nextFireTime = Time.time + gunData.fireRate;
        }
    }

    public void Reload()
    {
        if(!player.IsReloading && bulletsLeft != gunData.magazineCapacity)
        {
            StartCoroutine(AnimateReload());
        }
    }

    public bool CanShoot()
    {
        if(!player.IsReloading && Time.time > nextFireTime && bulletsLeft > 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    private void HandleAnimationSynchronization()
    {
        AnimatorStateInfo armsInfo = arms.GetCurrentAnimatorStateInfo(0);

        if (armsInfo.IsName("shoot"))
        {
            arms.SetBool("Shoot", false);
        }
    }

    IEnumerator AnimateReload()
    {
        Debug.Log("Reloading...");

        //playerMotor.IsReloading = true;
        player.IsReloading = true;
        
        yield return new WaitForSeconds(0.2f);

        //animator.SetTrigger("Reload");
        arms.SetBool("IsReloading", true);

        float reloadSpeed = 1f / gunData.reloadTime;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            yield return null;
        }

        player.IsReloading = false;
        //playerMotor.IsReloading = false;
        arms.SetBool("IsReloading", false);
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

    public void ResetBurst()
    {
        shotsRemainingInBurst = gunData.burstCount;
    }
}
