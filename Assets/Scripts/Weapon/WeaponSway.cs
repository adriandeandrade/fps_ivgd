using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Configuration")]
    [SerializeField] private float swayAmount = 0.055f;
    [SerializeField] private float maxSway = 0.09f;

    float smooth = 2;

    Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        float factorX = -Input.GetAxis("Mouse X") * swayAmount;
        float factorY = -Input.GetAxis("Mouse Y") * swayAmount;

        factorX = Mathf.Clamp(factorX, -maxSway, maxSway);
        factorY = Mathf.Clamp(factorY, -maxSway, maxSway);

        Vector3 final = new Vector3(initialPosition.x + factorX, initialPosition.y + factorY, initialPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, final, Time.deltaTime * smooth);
    }
}
