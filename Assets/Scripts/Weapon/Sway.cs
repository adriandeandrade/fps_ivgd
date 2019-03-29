using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    [SerializeField] private float amount = 0.055f;
    [SerializeField] private float maxAmount = 0.09f;

    float smooth = 3;

    Vector3 def;
    Vector3 defAth;
    Vector3 euler;

    GameObject ath;

    private void Start()
    {
        def = transform.localPosition;
        euler = transform.localEulerAngles;
    }

    float _smooth;
    private void Update()
    {
        _smooth = smooth;

        float factorX = -Input.GetAxis("Mouse X") * amount;
        float factorY = -Input.GetAxis("Mouse Y") * amount;

        if(factorX > maxAmount)
        {
            factorX = maxAmount;
        }

        if(factorX < -maxAmount)
        {
            factorX = -maxAmount;
        }

        if (factorY > maxAmount)
        {
            factorY = maxAmount;
        }

        if (factorY < -maxAmount)
        {
            factorY = -maxAmount;
        }

        Vector3 final = new Vector3(def.x + factorX, def.y + factorY, def.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, final, Time.deltaTime * _smooth);
    }
}
