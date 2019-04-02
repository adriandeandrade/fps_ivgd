using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveTest : MonoBehaviour
{
    Animator anim;

    public float movespeed;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", movespeed);
    }
}
