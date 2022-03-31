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
    [Header("第一人称相机")]
    public Camera fisrtPersonCam;

    //枪械的攻击动画
    internal protected Animator GunAnim;

    //当前子弹数量
    protected int currentBulltCount;
    //最大携带数量
    protected int currentBulltMaxCount;
    //是否在换弹
    internal protected bool isReloading;

    //最后一次开枪的时间
    protected float lastFireTime;
    //瞄准相机的初始fov
    protected float originFov;
    //是否在瞄准
    protected bool isAiming;
    //子弹散射角度
    [SerializeField]protected float SpreadAngle = 60f;
    //瞄准的携程
    protected IEnumerator aimEnumerator;

    private void Awake()
    {
        GunAnim = GetComponent<Animator>();
        fisrtPersonCam = GameObject.FindWithTag("FPCam").GetComponent<Camera>();
    }

    protected virtual void Start()
    {
        originFov = fisrtPersonCam.fieldOfView;
        currentBulltCount = clip;
        currentBulltMaxCount = MaxClip;
        aimEnumerator = DoAim();
    }

    //射击的方法
    protected abstract void Shooting();

    //填装子弹的方法
    protected abstract void FillBullet();

    //瞄准的方法
    internal void Aim(bool _isAiming)
    {
        if (isReloading) return;
        isAiming = _isAiming;
        GunAnim.SetBool("Aim", isAiming);
        if (aimEnumerator == null)
        {
            aimEnumerator = DoAim();
            StartCoroutine(DoAim());
        }
        else
        {
            StopCoroutine(DoAim());
            aimEnumerator = null;

            aimEnumerator = DoAim();
            StartCoroutine(DoAim());
        }
    }

    /// <summary>
    /// 进行装弹的检测，并播放动画
    /// </summary>
    /// <param name="num">num为0时表示是打空了弹夹按鼠标左键换弹，为1表示按R换弹</param>
    internal void DoReload(int num)
    {
        //装子弹逻辑
        if (num == 0)
        {
            if (currentBulltMaxCount > 0 && !isReloading)
            {
                if (currentBulltCount <= 0 && currentBulltMaxCount > 0)
                {
                    isAiming = false;
                    GunAnim.SetBool("Aim", isAiming);
                    GunAnim.Play("ReloadOut", 0);
                    reloadAudioSource.clip = firearmAudioData.reloadoutAudio;
                    reloadAudioSource.Play();
                    isReloading = true;
                }
            }
        }
        if (num == 1)
        {
            if (currentBulltMaxCount > 0 && currentBulltCount < clip && !isReloading)
            {
                isAiming = false;
                GunAnim.SetBool("Aim", isAiming);
                if (currentBulltCount != 0)
                {
                    GunAnim.Play("Reload", 0);
                }
                else
                {
                    GunAnim.Play("ReloadOut", 0);
                }
                reloadAudioSource.clip = firearmAudioData.reloadAudio;
                reloadAudioSource.Play();
                isReloading = true;
            }
        }
    }

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
        float temp_spreadPercent = SpreadAngle / fisrtPersonCam.fieldOfView;
        //随机一下
        return (Random.value>0.5f?1:-1)*temp_spreadPercent*Random.insideUnitCircle;
    }

    /// <summary>
    /// 瞄准后调整相机的FOV值
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DoAim()
    {
        while (true)
        {
            yield return null;
            float temp_fov = 0;
            fisrtPersonCam.fieldOfView = Mathf.SmoothDamp(fisrtPersonCam.fieldOfView, isAiming ? 26 : originFov, ref temp_fov, Time.deltaTime * 5);
        }
    }
}
