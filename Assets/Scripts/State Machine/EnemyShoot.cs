using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private GameObject shellPrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Transform shellEjection;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform shootPoint;

    int bulletsLeft;
    float reloadTime = 1f;
    bool isReloading = false;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        bulletsLeft = 3;
    }

    public void ShootGun()
    {
        if(bulletsLeft > 0)
        {
            AudioManager.instance.PlaySFX("m1911");
            GameObject casing = Instantiate(shellPrefab, shellEjection.position, shellEjection.rotation);
            GameObject muzzleFlashInstance = Instantiate(muzzleFlash, muzzle.position, Quaternion.identity);
            Destroy(muzzleFlashInstance, 1f);
            Destroy(casing, 1f);
            bulletsLeft--;
            ShootRay();
        }
    }

    private void LateUpdate()
    {
        if(!isReloading && bulletsLeft == 0)
        {
            Reload();
        }
    }

    private void Reload()
    {
        if(!isReloading && bulletsLeft != 30)
        {
            StartCoroutine(AnimateWeapon());
        } 
    }

    private void ShootRay()
    {
        RaycastHit hit;
        Vector3 dirFromPlayer = FindObjectOfType<Player>().transform.position - transform.position;

        Debug.DrawRay(shootPoint.position, shootPoint.forward * 300, Color.green);

        if (Physics.Raycast(shootPoint.position, dirFromPlayer, out hit))
        {
            //Instantiate(debugItem, hit.point, Quaternion.identity);
            TakeDamage enemy = hit.collider.GetComponent<TakeDamage>();

            if (enemy != null)
            {
                Vector3 hitPoint = hit.point;
                enemy.ReceiveDamage(10);
            }
        }
    }

    IEnumerator AnimateWeapon()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("Reloading", true);
        float reloadSpeed = 1f / reloadTime;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            yield return null;
        }

        isReloading = false;
        anim.SetBool("Reloading", false);
        bulletsLeft = 30;
    }
}
