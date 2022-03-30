using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float bulletSpeed;
    //子弹击中后的效果
    public GameObject impactPrefab;
    Rigidbody rb;
    TrailRenderer trailRenderer;
    Collider coll;
    Vector3 oriPos;

    private void Awake()
    {
        Invoke("DestoryBullet", 10f);
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        coll = GetComponent<Collider>();
    }

    private void Start()
    {
        oriPos = transform.position;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale = 1;
        }
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
        //碰撞检测 从子弹的现在的位置到上一帧的位置发射一条射线来检测
        float length = (transform.position - oriPos).magnitude;//射线的长度
        Vector3 direction = transform.position - oriPos;//方向
        RaycastHit hitinfo;
        bool isCollider = Physics.Raycast(oriPos, direction, out hitinfo, length,~(1<<9|1<<8));
        if (isCollider)
        {
            Debug.Log(hitinfo.collider.name);
            GameObject bulletEffect = Instantiate(impactPrefab,hitinfo.point,Quaternion.LookRotation(hitinfo.normal,Vector3.up));
            Destroy(bulletEffect,3f);
            ObjectPoolManager.Instance.DeSpawn(gameObject);
        }


        oriPos = transform.position;//记录原来的位置
    }

    void DestoryBullet()
    {
        ObjectPoolManager.Instance.DeSpawn(gameObject);
        trailRenderer.Clear();
    }



    private void OnEnable()
    {
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
        Invoke("DestoryBullet", 10f);
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
        trailRenderer.Clear();
    }
}
