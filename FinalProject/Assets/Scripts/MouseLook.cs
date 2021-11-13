using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private float mouseX, mouseY, xRotation=0f;
    public float mouseSensitivity = 100;
    
    public Transform PlayerCamera;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        PlayerCamera.Rotate(Vector3.up * mouseX);

        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);//clamping player rotattion to 180 deg
        this.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            

        
    }
}
