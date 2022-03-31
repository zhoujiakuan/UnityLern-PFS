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
    FPCamController fpCam;

    protected override void Start()
    {
        base.Start();
        aimEnumerator = DoAim();
        fpCam = GameObject.FindWithTag("MainCamera").GetComponent<FPCamController>();
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
        if (Input.GetKeyDown(KeyCode.R) && currentBulltCount < clip && !isReloading)
        {
            if (currentBulltMaxCount > 0)
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
            fpCam.FireRecoil();
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

    private void CreateBullte()
    {
        //通过第一人称的相机向屏幕中间（也就是准星的方向）发射一条射线
        //如果射线碰到了物体就把目标点设为碰撞点，如果没有碰撞到就把目标点设置为摄像机前方1000米
        Vector3 targetPoint;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            targetPoint = hitInfo.point;
        }
        else
        {
            targetPoint = Camera.main.transform.forward * 10000;
        }

        GameObject bullet = ObjectPoolManager.Instance.Spawn("Bullet", BulletSpawnPoint.position, BulletSpawnPoint.rotation).gameObject;
        bullet.GetComponent<BulletBehaviour>().impactAudioData = impactAudioData;
        bullet.transform.LookAt(targetPoint);
        bullet.transform.eulerAngles += CalculateSpread();
        bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.25f);
    }
}
