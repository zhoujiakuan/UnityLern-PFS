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
    [Header("子弹撞击音源文件")]
    public ImpactAudioData impactAudioData;
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
    //子弹散射角度
    protected float SpreadAngle = 60f;

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

    /// <summary>
    /// 根据枪械的射速来判断是否可以开枪
    /// </summary>
    /// <returns></returns>
    protected bool isAllowedShooting()
    {
        return Time.time - lastFireTime > 1/fireRate;
    }

    /// <summary>
    /// 计算子弹的散射
    /// </summary>
    protected Vector3 CalculateSpread()
    {
        //计算子弹散射的百分比
        float temp_spreadPercent = SpreadAngle / eyesCam.fieldOfView;
        //随机一下
        return temp_spreadPercent*Random.insideUnitCircle;
    }

}
