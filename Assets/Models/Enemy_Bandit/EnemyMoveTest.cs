using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveTest : MonoBehaviour
{
    Animator anim;

    public float movespeed;
    public bool isShoot;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShoot)
        {
            anim.SetBool("IsShooting", false);
            anim.SetFloat("Speed", movespeed);
        }

        if (isShoot)
            anim.SetBool("IsShooting", true);

        if (movespeed > 0.01f)
        {
            StartCoroutine(playFootStep());
        }
    }

    IEnumerator playFootStep()
    {
        FindObjectOfType<AudioManager>().PlaySFX("footstep");
        yield return new WaitForSeconds(1);
    }
}
