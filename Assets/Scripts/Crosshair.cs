using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private RectTransform reticle;
    [SerializeField] private float restingSize;
    [SerializeField] private float maxSize;
    [SerializeField] private float speed = 15f;

    float currentReticleSize;

    Player player;

    private void Awake()
    {
        reticle = GetComponent<RectTransform>();
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if(Mathf.Abs(InputManager.instance.Movement.magnitude) > 0.01f)
        {
            currentReticleSize = maxSize;
        } else
        {
            currentReticleSize = restingSize;
        }

        Vector2 targetSize = Vector2.Lerp(reticle.sizeDelta, new Vector2(currentReticleSize, currentReticleSize), speed * Time.deltaTime);
        reticle.sizeDelta = targetSize;
    }
}
