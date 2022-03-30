using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽象的枪械类  需要实现武器接口
/// </summary>
public abstract class Firearms : MonoBehaviour, IWeapon
{
    [Header("枪口位置")]
    public Transform MuzzlePoint;
    [Header("抛出蛋壳位置")]
    public Transform CasingPoint;
    [Header("子弹生成位置")]
    public Transform BulletSpawnPoint;
    [Header("弹夹")]
    public int clip = 30;
    public int MaxClip = 120;
    [Header("射速")]
    public float fireRate;
    [Header("特效")]
    public ParticleSystem MuzzleParticle;
    public ParticleSystem CasingParticle;
    [Header("枪声音源")]
    public AudioSource shootingAudioSource;
    [Header("换弹音源")]
    public AudioSource reloadAudioSource;
    [Header("步枪音源文件")]
    public FirearmAudioData firearmAudioData;
    [Header("瞄准的相机")]
    public Camera eyesCam;

    //枪械的攻击动画
    protected Animator GunAnim;

    //当前子弹数量
    protected int currentBulltCount;
    //最大携带数量
    protected int currentBulltMaxCount;

    //最后一次开枪的时间
    protected float lastFireTime;
    //瞄准相机的初始fov
    protected float originFov;
    //是否在瞄准
    protected bool isAiming;

    protected virtual void Start()
    {
        GunAnim = GetComponent<Animator>();
        originFov = eyesCam.fieldOfView;
        currentBulltCount = clip;
        currentBulltMaxCount = MaxClip;

    }

    //射击的方法
    protected abstract void Shooting();

    //填装子弹的方法
    protected abstract void Reload();

    //瞄准的方法
    protected abstract void Aim();

    public void DoAttack()
    {
        Shooting();
    }

    //根据枪械的射速来判断是否可以开枪
    protected bool isAllowedShooting()
    {
        return Time.time - lastFireTime > 1/fireRate;
    }

    protected void CreateBullte()
    {
        //通过第一人称的相机向屏幕中间（也就是准星的方向）发射一条射线
        //如果射线碰到了物体就把目标点设为碰撞点，如果没有碰撞到就把目标点设置为摄像机前方1000米
        Vector3 targetPoint;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if(Physics.Raycast(ray,out RaycastHit hitInfo))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            targetPoint = Camera.main.transform.forward * 10000;
        }

        GameObject bullet = ObjectPoolManager.Instance.Spawn("Bullet", BulletSpawnPoint.position,BulletSpawnPoint.rotation).gameObject;
        bullet.transform.LookAt(targetPoint);
        bullet.transform.localScale = new Vector3(0.1f,0.1f,0.25f);
    }

}
