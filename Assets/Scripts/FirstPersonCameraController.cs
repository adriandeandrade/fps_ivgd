using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    public enum RotationAxis { MouseX = 1, MouseY = 2, Both = 3 };
    [SerializeField] private RotationAxis axes = RotationAxis.Both;
    [SerializeField] private float horizontalSensitivity = 10.0f;
    [SerializeField] private float verticalSensitivity = 10.0f;
    [SerializeField] private float minYAngle = -45.0f;
    [SerializeField] private float maxYAngle = 45.0f;

    float inputX;
    float inputY;

    float rotationX;
    float rotationY;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        inputX = Input.GetAxis("Mouse X");
        inputY = Input.GetAxis("Mouse Y");

        if (axes == RotationAxis.Both)
        {
            transform.Rotate(0f, inputX * horizontalSensitivity * Time.deltaTime, 0f);
            rotationX -= Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, minYAngle, maxYAngle);

            rotationY = transform.localEulerAngles.y;
            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
        }
        else if (axes == RotationAxis.MouseX)
        {
            transform.Rotate(0f, inputX * horizontalSensitivity * Time.deltaTime, 0f);
        }
        else if (axes == RotationAxis.MouseY)
        {
            rotationX -= Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, minYAngle, maxYAngle);

            rotationY = transform.localEulerAngles.y;
            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
        }

    }
}
