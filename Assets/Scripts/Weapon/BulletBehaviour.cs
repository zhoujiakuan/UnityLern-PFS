using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour,IPoolObjectBase
{
    public float bulletSpeed;
    public GameObject impactPrefab;//子弹击中后的效果
    public ImpactAudioData impactAudioData;
    Rigidbody rb;
    TrailRenderer trailRenderer;
    Vector3 oriPos;//子弹的上一帧位置
    Transform impactParent;
    public Vector3 fly_dir;


    public void Init()
    {
        Invoke("DestoryBullet", 10f);
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        oriPos = transform.position;
        impactParent = GameObject.Find("ImpactParent").transform;
    }

    private void Update()
    {
        //碰撞检测 从子弹的现在的位置到上一帧的位置发射一条射线来检测
        float length = (transform.position - oriPos).magnitude;//射线的长度
        Vector3 direction = transform.position - oriPos;//方向
        RaycastHit hitinfo;
        bool isCollider = Physics.Raycast(oriPos, direction, out hitinfo, length, ~(1 << 9 | 1 << 8));
        if (isCollider && hitinfo.collider.gameObject.name != "Player")
        {
            Debug.Log("2222" + hitinfo.point);
            GameObject bulletEffect = Instantiate(impactPrefab, hitinfo.point, Quaternion.LookRotation(hitinfo.normal, Vector3.up));
            //播放子弹撞击的声音
            foreach (var audio in impactAudioData.ImpactAudios)
            {
                if (hitinfo.collider.CompareTag(audio.tag))
                {
                    AudioClip audioClip = audio.AudioClips[Random.Range(0, audio.AudioClips.Count)];
                    AudioSource.PlayClipAtPoint(audioClip, hitinfo.point);
                }
            }
            //设置一下父物体 不然窗口太乱
            bulletEffect.transform.SetParent(impactParent);
            //3秒后销毁效果
            Destroy(bulletEffect, 3f);
            //回收子弹
            ObjectPoolManager.Instance.DeSpawn(gameObject);
            ClearState();
        }


        oriPos = transform.position;//记录原来的位置
    }

    public void DestoryBullet()
    {
        ObjectPoolManager.Instance.DeSpawn(gameObject);
        trailRenderer.Clear();
    }

    public void AddForce()
    {
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
        Invoke("DestoryBullet", 10f);
    }

    public void ClearState()
    {
        rb.velocity = Vector3.zero;
        trailRenderer.Clear();
    }

    public void OnInstant()
    {
        Init();
    }

    public void OnSpawnBefore()
    {
        
    }

    public void OnSpawnEnd()
    {
        
    }

    public void OnDeSpawn()
    {
        ClearState();
    }
}
