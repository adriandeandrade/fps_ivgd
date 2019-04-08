using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //Public Variables
    [SerializeField] private WeaponData gunData;
    [SerializeField] private GameObject debugItem;
    [SerializeField] private GameObject shellPrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private Transform shellEjection;
    [SerializeField] private Transform muzzle;
    [SerializeField] private LayerMask ignoreMask;

    enum WeaponStates { READY, RELOAD };
    WeaponStates weaponState;

    // Private Variables
    int bulletsLeft;
    int bulletsLeftBeforeReload;
    int shotsRemainingInBurst;
    float nextFireTime;
    float startAnimTime = 0.3f;
    float stopAnimTime = 0.15f;
    bool triggerReleasedSinceLastShot;

    // Components
    Camera cam;
    FireTypes fireType;
    //Animator animator;
    Animator arms;
    PlayerMotor playerMotor;
    Player player;
    Crosshair crosshair;
    AudioSource shootSource;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        cam = Camera.main;
        crosshair = FindObjectOfType<Crosshair>();
        playerMotor = GetComponentInParent<PlayerMotor>();
        arms = GetComponentInParent<Animator>();
        shootSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        fireType = gunData.fireType;
        bulletsLeft = gunData.magazineCapacity;
        player.AmmoText.SetText(bulletsLeft + " / " + gunData.magazineCapacity);
        nextFireTime = Time.time + gunData.fireRate;
    }

    private void OnEnable()
    {
        player.AmmoText.SetText(bulletsLeft + " / " + gunData.magazineCapacity);
    }

    private void FixedUpdate()
    {
        HandleAnimationSynchronization();
    }

    private void LateUpdate()
    {
        if(Input.GetButtonDown(InputManager.instance.reloadButton))
        {
            Reload();
        }

        if (!player.IsReloading && bulletsLeft == 0)
        {
            Reload();
        }

        if(player.CurrentWeapon != null)
        {
            HandleCurrentWeapon();
        }
    }

    public void Initialize(Animator newAnimtor)
    {
        arms = newAnimtor;
        nextFireTime = Time.time + gunData.fireRate;
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
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            arms.CrossFadeInFixedTime("shoot", 0.01f);
            Instantiate(shellPrefab, shellEjection.position, shellEjection.rotation);
            GameObject muzzleFlashInstance = Instantiate(muzzleFlash, muzzle.position, Quaternion.identity);
            Destroy(muzzleFlashInstance, 1f);
            shootSource.PlayOneShot(gunData.shootSound);
            ShootRay();
            bulletsLeft--;
            player.AmmoText.SetText(bulletsLeft + " / " + gunData.magazineCapacity);
            nextFireTime = Time.time + gunData.fireRate;
        }
    }

    public void Reload()
    {
        if (!player.IsReloading && bulletsLeft != gunData.magazineCapacity)
        {
            bulletsLeftBeforeReload = bulletsLeft;
            StartCoroutine(AnimateReload());
        }
    }

    public bool CanShoot()
    {
        if (!player.IsReloading && Time.time > nextFireTime && bulletsLeft > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void HandleCurrentWeapon()
    {
        if (player.IsAimingDownSights)
        {
            arms.SetBool("IsAiming", true);
            crosshair.ToggleCrosshair(false);
        }
        else if (!player.IsAimingDownSights)
        {
            arms.SetBool("IsAiming", false);
            crosshair.ToggleCrosshair(true);
        }

        if (InputManager.instance.InputMag > 0.01f)
        {
            arms.SetFloat("InputMagnitude", InputManager.instance.InputMag, startAnimTime, Time.deltaTime);
        }
        else if (InputManager.instance.InputMag < 0.01f)
        {
            arms.SetFloat("InputMagnitude", InputManager.instance.InputMag, stopAnimTime, Time.deltaTime);
        }

        if (InputManager.instance.Shoot > 0) OnTriggerHold();
        if (InputManager.instance.Shoot <= 0) OnTriggerReleased();

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
        //Debug.Log("Reloading...");

        //playerMotor.IsReloading = true;
        player.IsReloading = true;

        yield return new WaitForSeconds(0.2f);

        //animator.SetTrigger("Reload");
        arms.SetBool("IsReloading", true);

        float reloadSpeed = 1f / gunData.reloadTime;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            yield return null;
        }

        player.IsReloading = false;
        //playerMotor.IsReloading = false;
        arms.SetBool("IsReloading", false);
        bulletsLeft = gunData.magazineCapacity;
        player.AmmoText.SetText(bulletsLeft + " / " + gunData.magazineCapacity);
        //Debug.Log("Finished Reloading!");
    }

    private void ShootRay()
    {
        RaycastHit hit;

        Vector3 lineOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(lineOrigin, Camera.main.transform.forward * 300, Color.green);

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, gunData.fireRange, ignoreMask))
        {
            //Instantiate(debugItem, hit.point, Quaternion.identity);
            TakeDamage enemy = hit.collider.GetComponent<TakeDamage>();
            Debug.Log("Hit");

            if (enemy != null)
            {
                Animator enemyAnimator = enemy.GetComponentInChildren<Animator>();
                enemyAnimator.SetTrigger("HitReaction");

                Vector3 hitPoint = hit.point;
                GameObject bloodEffect = Instantiate(bloodEffectPrefab, hitPoint, Quaternion.identity);
                Destroy(bloodEffect, 2f);
                enemy.ReceiveDamage(10);
            }
        }
    }

    public void CancelReload()
    {
        player.IsReloading = false;
        bulletsLeft = bulletsLeftBeforeReload;
    }

    public void ResetBurst()
    {
        shotsRemainingInBurst = gunData.burstCount;
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerReleased()
    {
        triggerReleasedSinceLastShot = true;
        ResetBurst();
    }
}
