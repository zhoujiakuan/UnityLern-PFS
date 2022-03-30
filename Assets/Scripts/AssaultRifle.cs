using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 突击步枪类 实现枪械类的方法
/// </summary>
public class AssaultRifle : Firearms
{
    bool isReloading;
    IEnumerator aimEnumerator;

    protected override void Start()
    {
        base.Start();
        aimEnumerator = DoAim();
    }

    protected override void Aim()
    {
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

    protected override void Reload()
    {
        //需要的子弹数量
        int needBulltCount = clip - currentBulltCount;

        if (needBulltCount >= currentBulltMaxCount)
        {
            //子弹不够了 只能把剩下的所有子弹装填
            currentBulltCount = currentBulltMaxCount;
            currentBulltMaxCount = 0;
            isReloading = false;
        }
        else
        {
            //子弹还够 直接填满弹夹
            currentBulltCount = clip;
            currentBulltMaxCount -= needBulltCount;
            isReloading = false;
        }


    }

    protected override void Shooting()
    {
        if (currentBulltCount <= 0) return;
        if (isReloading) return;

        if (isAllowedShooting())
        {
            CreateBullte();
            if (isAiming)
            {
                GunAnim.Play("AimFire");
            }
            else
            {
                GunAnim.Play("Fire",0);
            }
            shootingAudioSource.clip = firearmAudioData.shootingAudio;
            shootingAudioSource.Play();
            MuzzleParticle.Play();
            CasingParticle.Play();
            currentBulltCount--;
            lastFireTime = Time.time;
        }
    }

    private void Update()
    {
        //射击
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            DoAttack();
        }


        //装子弹逻辑
        if (Input.GetMouseButtonDown(0))
        {
            if (currentBulltMaxCount > 0)
            {
                if (currentBulltCount <= 0 && currentBulltMaxCount > 0)
                {
                    isAiming = false;
                    GunAnim.SetBool("Aim", isAiming);
                    GunAnim.Play("ReloadOut",0);
                    reloadAudioSource.clip = firearmAudioData.reloadoutAudio;
                    reloadAudioSource.Play();
                    isReloading = true;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && currentBulltCount < clip)
        {
            if (currentBulltMaxCount > 0)
            {
                isAiming = false;
                GunAnim.SetBool("Aim", isAiming);
                if (currentBulltCount!=0)
                {
                    GunAnim.Play("Reload",0);
                }
                else
                {
                    GunAnim.Play("ReloadOut",0);
                }
                reloadAudioSource.clip = firearmAudioData.reloadAudio;
                reloadAudioSource.Play();
                isReloading = true;
            }
        }

        //瞄准逻辑
        if (Input.GetMouseButtonDown(1) && !isReloading)
        {
            isAiming = true;
            //瞄准
            Aim();
        }
        if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
            //取消瞄准
            Aim();
        }
    }

    /// <summary>
    /// 瞄准后调整相机的FOV值
    /// </summary>
    /// <returns></returns>
    IEnumerator DoAim()
    {
        while (true)
        {
            yield return null;
            float temp_fov = 0;
            eyesCam.fieldOfView = Mathf.SmoothDamp(eyesCam.fieldOfView,isAiming?26:originFov,ref temp_fov,Time.deltaTime*5);
        }
    }
}
