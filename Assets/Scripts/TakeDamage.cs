using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeDamage : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private Image healthBar;
    [SerializeField] private bool useHealthbar;
    [SerializeField] private AudioClip bulletImpactSound;

    float currentHealth;
    bool isDead = false;

    AudioSource aSource;

    private void Awake()
    {
        aSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(useHealthbar)
        {
            float ratio = currentHealth / maxHealth;
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, ratio, 0.1f);

            if(currentHealth <= 0)
            {
                // Do gameover stuff here.
            }
        } else
        {
            if (currentHealth <= 0)
            {
                Unit unit = GetComponent<Unit>();

                if (unit != null)
                {
                    unit.agent.SetDestination(transform.position);
                    unit.enabled = false;
                }

                Animator anim = GetComponentInChildren<Animator>();

                if(anim != null)
                {
                    Debug.Log("Playing death");
                    //anim.SetTrigger("Die");
                }
                Destroy(gameObject);
            }
        }
    }

    public void ReceiveDamage(int amount)
    {
        currentHealth -= amount;
        aSource.PlayOneShot(bulletImpactSound);
    }
}
