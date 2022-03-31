using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCamController : MonoBehaviour
{
    public float mouseSpeed;
    private Vector3 camRotation;
    public Transform PlayerBody;
    public Transform GunBody;

    [Header("后坐力相关")]
    public AnimationCurve RecoilCurve;
    public Vector2 RecoilRange;//后坐力的范围
    public float recoilBufferTime = 0.3f;//后坐力缓冲时间

    private float currentRecoilTime;
    private Vector2 currentRecoil;

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

        //增加后坐力
        CalculateRecoilOffset();
        camRotation.x -= temp_y + currentRecoil.y;
        camRotation.y += temp_x + (Random.value > 0.5f ? 1 : -1)*currentRecoil.x;

        //相机旋转控制上下
        //transform.localRotation = Quaternion.Euler(camRotation.x, 0, 0);
        GunBody.localRotation = Quaternion.Euler(camRotation.x, 0, 0);
        //角色旋转控制面朝方向
        PlayerBody.localRotation = Quaternion.Euler(0, camRotation.y, 0);
    }

    /// <summary>
    /// 计算后坐力的偏移
    /// </summary>
    private void CalculateRecoilOffset()
    {
        currentRecoilTime += Time.deltaTime/recoilBufferTime;
        //Evaluate(float time) 返回在float指定的时间点的曲线值
        float temp_recoilFractoin = RecoilCurve.Evaluate(currentRecoilTime);
        currentRecoil = Vector2.Lerp(Vector2.zero,currentRecoil,temp_recoilFractoin);
    }

    /// <summary>
    /// 每次开枪都增加偏移量
    /// </summary>
    public void FireRecoil()
    {
        currentRecoil += RecoilRange;
        currentRecoilTime = 0;
    }
}
