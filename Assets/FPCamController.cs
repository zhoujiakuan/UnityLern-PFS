using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCamController : MonoBehaviour
{
    public float mouseSpeed;
    private Vector3 camRotation;
    public Transform PlayerBody;

    private void Update()
    {
        float temp_x = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float temp_y = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        camRotation.x -= temp_y;
        camRotation.x = Mathf.Clamp(camRotation.x,-90f,90f);
        transform.localRotation = Quaternion.Euler(camRotation.x,0,0);
        PlayerBody.Rotate(Vector3.up * temp_x);
    }
}
