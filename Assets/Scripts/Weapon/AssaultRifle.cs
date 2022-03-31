using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 突击步枪类 实现枪械类的方法
/// </summary>
public class AssaultRifle : Firearms
{
    FPCamController fpCam;

    protected override void Start()
    {
        base.Start();
        fpCam = GameObject.FindWithTag("MainCamera").GetComponent<FPCamController>();
    }

    /// <summary>
    /// 在换弹动画末尾中添加来达到动画播放完毕后增加子弹
    /// </summary>
    protected override void FillBullet()
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
