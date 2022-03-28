using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCamController : MonoBehaviour
{
    public float mouseSpeed;
    private Vector3 camRotation;
    public Transform PlayerBody;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float temp_x = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float temp_y = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        camRotation.x -= temp_y;
        camRotation.y += temp_x;
        camRotation.x = Mathf.Clamp(camRotation.x,-90f,90f);
        //相机旋转控制上下
        transform.localRotation = Quaternion.Euler(camRotation.x, 0, 0);
        //角色旋转控制面朝方向
        PlayerBody.localRotation = Quaternion.Euler(0, camRotation.y, 0);
    }
}
